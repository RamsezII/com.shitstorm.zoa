using System;

namespace _ZOA_.Ast.compilation
{
    internal abstract class AstExpression : AstAbstract
    {

        //----------------------------------------------------------------------------------------------------------

        public AstExpression(in Type output_type) : base(output_type)
        {
        }

        //----------------------------------------------------------------------------------------------------------

        public static bool TryParseExpression(in Signal signal, in TScope tscope, in TStack tstack, in bool read_as_argument, in Type expected_type, out AstExpression expression)
        {
            expression = null;

            if (!AstAssignation.TryParseAssignation(signal, tscope, tstack, expected_type, out AstAssignation assignation) && signal.reader.sig_error != null)
                return false;

            if (assignation == null)
                if (AstTernaryOperator.TryParseTernary(signal, tscope, tstack, expected_type, out expression))
                {
                    if (read_as_argument)
                        if (!signal.reader.TryReadChar_match(',', lint: signal.reader.lint_theme.argument_coma) && !signal.reader.TryPeekChar_match(')', out _))
                            if (signal.reader.strict_syntax)
                            {
                                signal.reader.Error($"expected ',' or ')' after expression");
                                return false;
                            }
                    return true;
                }

            if (signal.reader.sig_error != null)
                goto failure;

            failure:
            expression = null;
            signal.reader.Error($"could not parse expression");
            return false;
        }
    }
}