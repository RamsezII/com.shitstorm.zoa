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
            in TypeStack type_stack,
            ValueStack value_stack,
            in Type expected_type,
            out ZoaExecutor executor
        )
        {
            if (signal.reader.TryReadString_matches_out(out string cont_name, as_function_argument: false, lint: signal.reader.lint_theme.contracts, matches: Contract.contracts.Keys.ToArray()))
                if (!Contract.contracts.TryGetValue(cont_name, out Contract contract))
                    signal.reader.Stderr($"no contract named '{cont_name}'.");
                else
                {
                    bool is_exe = signal.flags.HasFlag(SIG_FLAGS.EXEC);

                    executor = new();
                    Dictionary<string, ZoaExecutor> exe_opts = null;

                    if (contract.options != null)
                    {
                        if (is_exe)
                            exe_opts = new();

                        foreach (var pair in contract.options)
                            if (pair.Value != null)
                                if (!TryParseExpression(signal, scope, type_stack, value_stack, false, pair.Value, out ZoaExecutor exe_opt))
                                {
                                    signal.reader.sig_error ??= $"could not parse expression for option {(pair.Key.short_name != '\0' ? $"\"-{pair.Key.short_name}\"" : string.Empty)}/\"--{pair.Key.long_name}\"";
                                    goto failure;
                                }
                                else if (is_exe)
                                    exe_opts.Add(pair.Key.long_name, exe_opt);
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

                    List<ZoaExecutor> exe_prms = null;
                    if (contract.parameters != null)
                    {
                        if (is_exe)
                            exe_prms = new();

                        for (int i = 0; i < contract.parameters._list.Count; i++)
                        {
                            Type arg_type = contract.parameters._list[i];
                            if (!TryParseExpression(signal, scope, type_stack, value_stack, true, arg_type, out ZoaExecutor exe_prm))
                            {
                                signal.reader.sig_error ??= $"could not parse argument[{i}]";
                                goto failure;
                            }
                            else if (is_exe)
                                exe_prms.Add(exe_prm);
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
                        var exe = executor;
                        executor.routine_SIG_ALL = EExecute_SIG_ALL();
                        IEnumerator<ExecutionOutput> EExecute_SIG_ALL()
                        {
                            int stack_count_expected = value_stack._stack.Count;

                            Dictionary<string, object> opts = null;
                            if (exe_opts != null)
                            {
                                opts = new(exe_opts.Count);
                                foreach (var pair in exe_opts)
                                    if (pair.Value == null)
                                        opts.Add(pair.Key, null);
                                    else
                                    {
                                        ZoaExecutor ex = pair.Value;
                                        while (!ex.isDone)
                                            yield return ex.OnSignal(exe.signal);
                                        opts.Add(pair.Key, value_stack.Pop());
                                    }
                            }

                            List<object> prms = null;
                            if (exe_prms != null)
                            {
                                prms = new(exe_prms.Count);
                                for (int i = 0; i < exe_prms.Count; i++)
                                {
                                    ZoaExecutor ex = exe_prms[i];
                                    while (!ex.isDone)
                                        yield return ex.OnSignal(exe.signal);
                                    prms[i] = value_stack.Pop();
                                }
                            }

                            if (contract.action_SIG_EXE != null)
                                if (!exe.signal.flags.HasFlag(SIG_FLAGS.EXEC))
                                    yield return new(CMD_STATUS.BLOCKED);
                                else
                                {
                                    contract.action_SIG_EXE(exe, scope, opts, prms);
                                    yield return new(CMD_STATUS.RETURN);
                                }

                            if (contract.routine_SIG_EXE != null)
                                if (!exe.signal.flags.HasFlag(SIG_FLAGS.EXEC))
                                    yield return new(CMD_STATUS.BLOCKED);
                                else
                                {
                                    using var routine = contract.routine_SIG_EXE(exe, scope, opts, prms);
                                    bool go = true;
                                    while (go)
                                        if (exe.signal.flags.HasFlag(SIG_FLAGS.EXEC))
                                            if (routine.MoveNext())
                                                yield return routine.Current;
                                            else
                                                go = false;
                                        else
                                            yield return new(CMD_STATUS.BLOCKED);
                                }

                            if (contract.routine_SIG_ALL != null)
                            {
                                using var routine = contract.routine_SIG_ALL(exe, scope, opts, prms);
                                while (routine.MoveNext())
                                    yield return routine.Current;
                            }

                            exe.isDone = true;

                            if (contract.output_type != null)
                                ++stack_count_expected;

                            if (stack_count_expected != value_stack._stack.Count)
                                exe.signal.exe_error ??= $"expected stack size: {stack_count_expected}, got: {value_stack._stack.Count}";
                            else
                            {
                                Type actual_output_type = value_stack.Peek().GetType();
                                if (contract.output_type != null && contract.output_type != actual_output_type)
                                    exe.signal.exe_error ??= $"expected output type: {contract.output_type}, got: {actual_output_type}";
                            }
                        }
                    }

                    return true;
                }

            failure:
            signal.reader.sig_error ??= $"could not parse contract";
            executor = null;
            return false;
        }
    }
}