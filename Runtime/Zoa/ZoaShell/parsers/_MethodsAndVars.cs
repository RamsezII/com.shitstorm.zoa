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
            out ZoaExecutor executor
        )
        {
            executor = null;
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
                                if (TryParseExpression(signal, scope, type_stack, value_stack, false, pair.Value, out ZoaExecutor exe_opt))
                                    if (is_exe)
                                        exe_opts.Add(pair.Key.long_name, exe_opt);
                    }

                    bool expects_parenthesis = signal.reader.strict_syntax;
                    bool found_parenthesis = signal.reader.TryReadChar_match('(');

                    if (found_parenthesis)
                        signal.reader.LintOpeningBraquet();

                    if (expects_parenthesis && !found_parenthesis)
                    {
                        signal.reader.Stderr($"'{contract.name}' expected opening parenthesis '('");
                        return false;
                    }

                    List<ZoaExecutor> exe_prms = null;
                    if (contract.parameters != null)
                    {
                        if (is_exe)
                            exe_prms = new();

                        foreach (var prm in contract.parameters)
                            if (TryParseExpression(signal, scope, type_stack, value_stack, true, prm, out ZoaExecutor exe_prm))
                                if (is_exe)
                                    exe_prms.Add(exe_prm);
                    }

                    if (signal.reader.sig_error != null)
                        return false;

                    if ((expects_parenthesis || found_parenthesis) && !signal.reader.TryReadChar_match(')', lint: signal.reader.CloseBraquetLint()))
                    {
                        signal.reader.Stderr($"'{contract.name}' expected closing parenthesis ')'");
                        return false;
                    }

                    if (signal.flags.HasFlag(SIG_FLAGS.EXEC))
                    {
                        var exe = executor;
                        executor.routine_SIG_ALL = EExecute_SIG_ALL();
                        IEnumerator<ExecutionOutput> EExecute_SIG_ALL()
                        {
                            Dictionary<string, object> opts = null;
                            if (exe_opts != null)
                            {
                                opts = new(exe_opts.Count);
                                foreach (var pair in exe_opts)
                                {
                                    ZoaExecutor ex = pair.Value;
                                    while (!ex.isDone)
                                    {
                                        ExecutionOutput output = ex.OnSignal(exe.signal);
                                        yield return output;
                                    }
                                    opts.Add(pair.Key, ex.output);
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
                                    {
                                        ExecutionOutput output = ex.OnSignal(exe.signal);
                                        yield return output;
                                    }
                                    prms[i] = ex.output;
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
                        }
                    }

                    return true;
                }

            executor = null;
            return false;
        }

        internal bool TryParseVariable(
            in Signal signal,
            MemScope scope,
            in TypeStack type_stack,
            ValueStack value_stack,
            out string var_name,
            out ZoaExecutor executor
        )
        {
            if (signal.reader.TryReadString_matches_out(out var_name, as_function_argument: false, lint: signal.reader.lint_theme.variables, matches: scope.EVarNames().ToArray()))
                if (!scope.TryGetCell(var_name, out MemCell cell))
                    signal.reader.Stderr($"no variable named '{var_name}'.");
                else
                {
                    signal.reader.LintToThisPosition(signal.reader.lint_theme.variables, true);

                    type_stack.Push(cell.type);
                    executor = new()
                    {
                        action_SIG_EXE = exe => value_stack.Push(cell.value),
                    };

                    if (signal.reader.sig_error == null)
                        return true;
                }

            executor = null;
            return false;
        }
    }
}