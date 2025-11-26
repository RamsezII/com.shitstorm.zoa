using System.Linq;

namespace _ZOA_
{
    partial class ZoaShell
    {
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