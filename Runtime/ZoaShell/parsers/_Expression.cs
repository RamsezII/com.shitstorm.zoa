namespace _ZOA_
{
    partial class ZoaShell
    {
        public bool TryParseExpression(in Signal signal, in MemScope scope, in bool read_as_argument, out ExpressionExecutor executor, in bool type_check = true)
        {
            executor = null;

            if (!TryParseAssignation(signal, scope, out AssignationExecutor exe_assign) && signal.reader.sig_error != null)
                return false;

            if (exe_assign == null && !TryParseOr(signal, scope, out executor))
                return false;

            if (read_as_argument)
                if (!signal.reader.TryReadChar_match(',', lint: signal.reader.lint_theme.argument_coma) && !signal.reader.TryPeekChar_match(')', out _))
                    if (signal.reader.strict_syntax)
                    {
                        signal.reader.Stderr($"expected ',' or ')' after expression.");
                        return false;
                    }

            if (signal.reader.TryReadChar_match('?', lint: signal.reader.lint_theme.operators))
            {
                var cond = executor;

                if (!cond.output_type.IsSubclassOf(typeof(bool)))
                {
                    signal.reader.Stderr($"expected bool expression.");
                    return false;
                }

                if (!TryParseExpression(signal, scope, false, out var _if, type_check: false))
                    signal.reader.Stderr($"expected expression after ternary operator '?'.");
                else if (!signal.reader.TryReadChar_match(':', lint: signal.reader.lint_theme.operators))
                    signal.reader.Stderr($"expected ternary operator delimiter ':'.");
                else if (!TryParseExpression(signal, scope, false, out var _else, type_check: false))
                    signal.reader.Stderr($"expected second expression after ternary operator ':'.");
                else
                {
                    executor = new TernaryOpExecutor(signal, scope, cond, _if, _else);
                    if (signal.reader.sig_error != null)
                        return false;
                }
            }

            return true;
        }
    }
}