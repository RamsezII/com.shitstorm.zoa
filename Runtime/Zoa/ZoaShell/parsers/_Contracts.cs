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
                    Dictionary<string, Executor> options1 = null;

                    if (contract.options != null)
                    {
                        if (signal.is_exec)
                            options1 = new();

                        foreach (var pair in contract.options)
                            if (pair.Value != null)
                                if (!TryParseExpression(signal, scope, false, pair.Value, exec_stack))
                                {
                                    signal.reader.sig_error ??= $"could not parse expression for option {(pair.Key.short_name != '\0' ? $"\"-{pair.Key.short_name}\"" : string.Empty)}/\"--{pair.Key.long_name}\"";
                                    goto failure;
                                }
                                else if (signal.is_exec)
                                    options1.Add(pair.Key.long_name, exec_stack.Peek());
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

                    List<Executor> params1 = null;
                    if (contract.parameters != null)
                    {
                        if (signal.is_exec)
                            params1 = new();

                        for (int i = 0; i < contract.parameters._list.Count; i++)
                        {
                            Type arg_type = contract.parameters._list[i];
                            if (!TryParseExpression(signal, scope, true, arg_type, exec_stack))
                            {
                                signal.reader.sig_error ??= $"could not parse argument[{i}]";
                                goto failure;
                            }
                            else if (signal.is_exec)
                                params1.Add(exec_stack.Peek());
                        }
                    }

                    if (signal.reader.sig_error != null)
                        goto failure;

                    if ((expects_parenthesis || found_parenthesis) && !signal.reader.TryReadChar_match(')', lint: signal.reader.CloseBraquetLint()))
                    {
                        signal.reader.Stderr($"'{contract.name}' expected closing parenthesis ')'");
                        goto failure;
                    }

                    if (signal.flags.HasFlag(SIG_FLAGS.EXEC))
                    {
                        Dictionary<string, object> options2 = null;
                        if (options1 != null)
                        {
                            options2 = new(options1.Count);
                            foreach (var pair in options1)
                                if (pair.Value == null)
                                    options2.Add(pair.Key, null);
                                else
                                    options2.Add(pair.Key, pair.Value.output_data);
                        }

                        List<object> params2 = null;
                        if (params1 != null)
                        {
                            params2 = new(params1.Count);
                            for (int i = 0; i < params1.Count; i++)
                                params2[i] = params1[i].output_data;
                        }

                        var executor = new Executor(contract.output_type);

                        if (contract.action_SIG_EXE != null)
                            executor.action_SIG_EXE = exe => contract.action_SIG_EXE(exe, scope, options2, params2);

                        if (contract.routine_SIG_EXE != null)
                            executor.routine_SIG_EXE = contract.routine_SIG_EXE(executor, scope, options2, params2);

                        if (contract.routine_SIG_ALL != null)
                            executor.routine_SIG_ALL = contract.routine_SIG_ALL(executor, scope, options2, params2);

                        exec_stack.Push(executor);
                    }

                    return true;
                }

            failure:
            return false;
        }
    }
}