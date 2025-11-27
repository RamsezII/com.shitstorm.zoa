using System;
using System.Linq;

namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseVariable(
            in Signal signal,
            MemScope scope,
            in Type expected_type,
            out string var_name,
            in ExecutionStack exec_stack
        )
        {
            if (signal.reader.TryReadString_matches_out(out var_name, as_function_argument: false, lint: signal.reader.lint_theme.variables, matches: scope.EVarNames().ToArray()))
                if (!scope.TryGetCell(var_name, out MemCell var_cell))
                {
                    signal.reader.sig_error ??= $"no variable named '{var_name}'.";
                    goto failure;
                }
                else if (expected_type != null && !var_cell.type.IsOfType(expected_type))
                {
                    signal.reader.sig_error ??= $"expted variable of type {expected_type}, got {var_cell.type}";
                    goto failure;
                }
                else
                {
                    signal.reader.LintToThisPosition(signal.reader.lint_theme.variables, true);

                    if (signal.is_exec)
                        exec_stack.Push(new("variable", var_cell.type)
                        {
                            action_SIG_EXE = exe => exe.output = var_cell.value,
                        });

                    return true;
                }

            failure:
            return false;
        }
    }
}