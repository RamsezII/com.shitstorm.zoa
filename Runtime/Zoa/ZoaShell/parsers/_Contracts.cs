using System;
using System.Collections.Generic;
using System.Linq;

namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseContract(
            in Signal signal,
            MemScope scope,
            in Type expected_type,
            in ExecutionStack exec_stack
        )
        {
            if (signal.reader.TryReadString_matches_out(out string cont_name, as_function_argument: false, lint: signal.reader.lint_theme.contracts, matches: Contract.contracts.Keys.ToArray()))
                if (!Contract.contracts.TryGetValue(cont_name, out Contract contract))
                {
                    signal.reader.sig_error ??= $"no contract named '{cont_name}'.";
                    goto failure;
                }
                else if (expected_type != null && contract.output_type == null && !contract.output_type.IsOfType(expected_type))
                {
                    signal.reader.sig_error ??= $"expected contract of type {expected_type}, got {contract.output_type}";
                    goto failure;
                }
                else
                {
                    Dictionary<string, Executor> exe_opts = null;
                    if (contract.options != null)
                    {
                        exe_opts = new();
                        foreach (var pair in contract.options)
                            if (pair.Value != null)
                                if (TryParseExpression(signal, scope, false, pair.Value, exec_stack))
                                    exe_opts.Add(pair.Key.long_name, exec_stack.Peek());
                                else
                                {
                                    signal.reader.sig_error ??= $"could not parse expression for option {(pair.Key.short_name != '\0' ? $"\"-{pair.Key.short_name}\"" : string.Empty)}/\"--{pair.Key.long_name}\"";
                                    goto failure;
                                }
                    }

                    bool expects_parenthesis = signal.reader.strict_syntax;
                    bool found_parenthesis = signal.reader.TryReadChar_match('(');

                    if (found_parenthesis)
                        signal.reader.LintOpeningBraquet();

                    if (expects_parenthesis && !found_parenthesis)
                    {
                        signal.reader.Stderr($"'{contract.name}' expected opening parenthesis '('");
                        goto failure;
                    }

                    List<Executor> exe_prms = null;
                    if (contract.parameters != null)
                    {
                        exe_prms = new();
                        for (int i = 0; i < contract.parameters._list.Count; i++)
                        {
                            Type arg_type = contract.parameters._list[i];
                            if (TryParseExpression(signal, scope, true, arg_type, exec_stack))
                                exe_prms.Add(exec_stack.Peek());
                            else
                            {
                                signal.reader.sig_error ??= $"could not parse argument[{i}]";
                                goto failure;
                            }
                        }
                    }

                    if (signal.reader.sig_error != null)
                        goto failure;

                    if ((expects_parenthesis || found_parenthesis) && !signal.reader.TryReadChar_match(')', lint: signal.reader.CloseBraquetLint()))
                    {
                        signal.reader.Stderr($"'{contract.name}' expected closing parenthesis ')'");
                        goto failure;
                    }

                    var executor = new Executor(contract.name, contract.output_type);
                    if (signal.is_exec)
                    {
                        Dictionary<string, object> vals_opts = null;
                        List<object> vals_prms = null;

                        executor.action_SIG_EXE = exe =>
                        {
                            if (exe_opts != null)
                            {
                                vals_opts = new(exe_opts.Count);
                                foreach (var pair in exe_opts)
                                    if (pair.Value == null)
                                        vals_opts.Add(pair.Key, null);
                                    else
                                        vals_opts.Add(pair.Key, pair.Value.output);
                            }

                            if (exe_prms != null)
                            {
                                vals_prms = new(exe_prms.Count);
                                for (int i = 0; i < exe_prms.Count; i++)
                                    vals_prms.Add(exe_prms[i].output);
                            }
                        };

                        if (contract.action_SIG_EXE != null)
                            executor.action_SIG_EXE += exe => contract.action_SIG_EXE(exe, scope, vals_opts, vals_prms);

                        if (contract.routine_SIG_EXE != null)
                            executor.routine_SIG_EXE = contract.routine_SIG_EXE(executor, scope, vals_opts, vals_prms);

                        if (contract.routine_SIG_ALL != null)
                            executor.routine_SIG_ALL = contract.routine_SIG_ALL(executor, scope, vals_opts, vals_prms);
                    }
                    exec_stack.Push(executor);

                    return true;
                }

            failure:
            return false;
        }
    }
}