using System.Collections.Generic;
using System.Linq;

namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseMethod(in Signal signal, in MemScope scope, out ContractExecutor executor)
        {
            if (signal.reader.TryReadString_matches_out(out string cont_name, as_function_argument: false, lint: signal.reader.lint_theme.contracts, matches: Contract.contracts.Keys.ToArray()))
                if (!Contract.contracts.TryGetValue(cont_name, out Contract contract))
                    signal.reader.Stderr($"no contract named '{cont_name}'.");
                else
                {
                    executor = new(signal, scope, contract);
                    if (signal.reader.sig_error == null)
                        return true;
                }

            executor = null;
            return false;
        }

        internal bool TryParseVariable(in Signal signal, in MemScope scope, out string var_name, out VariableExecutor executor)
        {
            if (signal.reader.TryReadString_matches_out(out var_name, as_function_argument: false, lint: signal.reader.lint_theme.variables, matches: scope.EVarNames().ToArray()))
                if (!scope.TryGetCell(var_name, out _))
                    signal.reader.Stderr($"no variable named '{var_name}'.");
                else
                {
                    signal.reader.LintToThisPosition(signal.reader.lint_theme.variables, true);
                    executor = new VariableExecutor(signal, scope, var_name);

                    if (signal.reader.sig_error == null)
                        return true;
                }

            executor = null;
            return false;
        }
    }
}