using System.Linq;

namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseContract(
            in Signal signal,
            MemScope scope,
            TypeStack type_stack,
            ValueStack value_stack,
            out Executor executor
        )
        {
            executor = null;
            if (signal.reader.TryReadString_matches_out(out string cont_name, as_function_argument: false, lint: signal.reader.lint_theme.contracts, matches: Contract.contracts.Keys.ToArray()))
                if (!Contract.contracts.TryGetValue(cont_name, out Contract contract))
                    signal.reader.Stderr($"no contract named '{cont_name}'.");
                else
                {
                    executor = new();

                    contract.options?.Invoke(executor, signal, type_stack);

                    bool expects_parenthesis = signal.reader.strict_syntax;
                    bool found_parenthesis = signal.reader.TryReadChar_match('(');

                    if (found_parenthesis)
                        signal.reader.LintOpeningBraquet();

                    if (expects_parenthesis && !found_parenthesis)
                    {
                        signal.reader.Stderr($"'{contract.name}' expected opening parenthesis '('");
                        return false;
                    }

                    contract.parameters?.Invoke(executor, signal, type_stack);

                    if (signal.reader.sig_error != null)
                        return false;

                    if ((expects_parenthesis || found_parenthesis) && !signal.reader.TryReadChar_match(')', lint: signal.reader.CloseBraquetLint()))
                    {
                        signal.reader.Stderr($"'{contract.name}' expected closing parenthesis ')'");
                        return false;
                    }

                    if (signal.flags.HasFlag(SIG_FLAGS.EXEC))
                    {
                        executor.action_SIG_EXE = exe => contract.action_SIG_EXE(exe, scope, value_stack);
                        executor.routine_SIG_EXE = contract.routine_SIG_EXE(executor, scope, value_stack);
                        executor.routine_SIG_ALL = contract.routine_SIG_ALL(executor, scope, value_stack);
                    }

                    return true;
                }

            executor = null;
            return false;
        }

        internal bool TryParseVariable(
            in Signal signal,
            in MemScope scope,
            in TypeStack type_stack,
            ValueStack value_stack,
            out string var_name,
            out Executor executor
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