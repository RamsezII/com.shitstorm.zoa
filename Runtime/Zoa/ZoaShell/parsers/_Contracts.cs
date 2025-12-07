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
            if (signal.reader.TryReadString_matches_out(out string cont_name, as_function_argument: false, lint: signal.reader.lint_theme.contracts, matches: ZoaContract.contracts.Keys.ToArray()))
                if (!ZoaContract.contracts.TryGetValue(cont_name, out ZoaContract contract))
                {
                    signal.reader.Stderr($"no contract named '{cont_name}'.");
                    goto failure;
                }
                else if (expected_type != null && (contract.output_type == null || !contract.output_type.CanBeAssignedTo(expected_type)))
                {
                    signal.reader.Stderr($"expected contract of type {expected_type}, got {contract.output_type}");
                    goto failure;
                }
                else
                {
                    Dictionary<string, Executor> opts_exe = null;
                    if (contract.options != null)
                    {
                        opts_exe = new();
                        foreach (var pair in contract.options)
                            if (pair.Value != null)
                                if (TryParseExpression(signal, scope, false, pair.Value, exec_stack))
                                    opts_exe.Add(pair.Key.long_name, exec_stack._stack[^1]);
                                else
                                {
                                    signal.reader.Stderr($"could not parse expression for option {(pair.Key.short_name != '\0' ? $"\"-{pair.Key.short_name}\"" : string.Empty)}/\"--{pair.Key.long_name}\"");
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

                    List<Executor> prms_exe = null;
                    if (contract.parameters != null)
                    {
                        prms_exe = new();
                        for (int i = 0; i < contract.parameters._list.Count; i++)
                        {
                            Type arg_type = contract.parameters._list[i];
                            if (TryParseExpression(signal, scope, true, arg_type, exec_stack))
                                prms_exe.Add(exec_stack._stack[^1]);
                            else
                            {
                                signal.reader.Stderr($"could not parse argument[{i}]");
                                goto failure;
                            }
                        }
                    }

                    bool opts_b = opts_exe != null;
                    bool prms_b = prms_exe != null;
                    bool args_b = opts_b || prms_b;

                    if (signal.reader.sig_error != null)
                        goto failure;

                    if ((expects_parenthesis || found_parenthesis) && !signal.reader.TryReadChar_match(')', lint: signal.reader.CloseBraquetLint()))
                    {
                        signal.reader.Stderr($"'{contract.name}' expected closing parenthesis ')'");
                        goto failure;
                    }


                    var args_exe = args_b ? new Executor("compute arguments", null) : null;
                    Dictionary<string, object> opts_vals = opts_b ? new(opts_exe.Count) : null;
                    List<object> prms_vals = prms_b ? new(prms_exe.Count) : null;

                    if (args_b)
                    {
                        exec_stack._stack.Add(args_exe);
                        if (signal.arm_executors)
                            args_exe.action_SIG_EXE = exe =>
                            {
                                if (opts_b)
                                    foreach (var pair in opts_exe)
                                        if (pair.Value == null)
                                            opts_vals.Add(pair.Key, null);
                                        else
                                            opts_vals.Add(pair.Key, pair.Value.output);

                                if (prms_b)
                                    for (int i = 0; i < prms_exe.Count; i++)
                                        prms_vals.Add(prms_exe[i].output);
                            };
                    }

                    var cont_exe = new Executor(contract.name, contract.output_type);
                    exec_stack._stack.Add(cont_exe);

                    if (signal.arm_executors)
                    {
                        if (contract.action_SIG_EXE != null)
                            cont_exe.action_SIG_EXE = exe => contract.action_SIG_EXE(exe, scope, opts_vals, prms_vals);

                        if (contract.routine_SIG_EXE != null)
                            cont_exe.routine_SIG_EXE = contract.routine_SIG_EXE(cont_exe, scope, opts_vals, prms_vals);

                        if (contract.routine_SIG_ALL != null)
                            cont_exe.routine_SIG_ALL = contract.routine_SIG_ALL(cont_exe, scope, opts_vals, prms_vals);
                    }

                    return true;
                }

            failure:
            return false;
        }
    }
}