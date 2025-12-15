using System;

namespace _ZOA_.Ast.compilation
{
    internal abstract class AstExpression : AstAbstract
    {
        public readonly Type output_type;

        //----------------------------------------------------------------------------------------------------------

        public AstExpression(in Type output_type)
        {
            this.output_type = output_type;
        }

        //----------------------------------------------------------------------------------------------------------

        public static bool TryExpr(in Signal signal, in TScope tscope, in bool read_as_argument, in Type expected_type, out AstExpression ast_expression)
        {
            if (AstAssignment.TryParseAssignment(signal, tscope, expected_type, out var ast_assignation))
            {
                ast_expression = ast_assignation;
                return true;
            }
            else if (signal.reader.sig_error != null)
                goto failure;

            if (AstConditional.TryConditional(signal, tscope, expected_type, out ast_expression))
            {
                if (read_as_argument)
                    if (!signal.reader.TryReadChar_match(',', lint: signal.reader.lint_theme.argument_coma) && !signal.reader.TryPeekChar_match(')', out _))
                        if (signal.reader.strict_syntax)
                        {
                            signal.reader.Error($"expected ',' or ')' after expression");
                            goto failure;
                        }
                return true;
            }

            failure:
            ast_expression = null;
            return false;
        }
    }
}