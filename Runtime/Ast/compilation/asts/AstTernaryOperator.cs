using System;

namespace _ZOA_.Ast.compilation
{
    internal class AstTernaryOperator : AstExpression
    {
        readonly AstExpression ast_cond, ast_yes, ast_no;

        //----------------------------------------------------------------------------------------------------------

        AstTernaryOperator(in AstExpression ast_cond, in AstExpression ast_yes, in AstExpression ast_no, in Type output_type) : base(output_type)
        {
            this.ast_cond = ast_cond;
            this.ast_yes = ast_yes;
            this.ast_no = ast_no;
        }

        //----------------------------------------------------------------------------------------------------------

        public static bool TryParseTernary(in Signal signal, in TScope tscope, in TStack tstack, in Type expected_type, out AstExpression ast)
        {
            if (!AstBinaryOperation.TryParseOr(signal, tscope, tstack, out ast))
                if (signal.reader.sig_error == null)
                    return false;
                else
                    goto failure;

            if (!signal.reader.TryReadChar_match('?', lint: signal.reader.lint_theme.operators))
                return true;
            else
            {
                if (ast.output_type != typeof(bool))
                {
                    signal.reader.Error($"expected {typeof(bool)} after ternary operator '?', got {ast.output_type}");
                    goto failure;
                }

                if (!TryParseExpression(signal, tscope, tstack, false, expected_type ?? typeof(object), out var expr_yes))
                    signal.reader.Error($"expected expression after ternary operator '?'");
                else
                {
                    if (!signal.reader.TryReadChar_match(':', lint: signal.reader.lint_theme.operators))
                        signal.reader.Error($"expected ternary operator delimiter ':'");
                    else
                    {
                        if (!TryParseExpression(signal, tscope, tstack, false, expected_type ?? typeof(object), out var expr_no))
                            signal.reader.Error($"expected second expression after ternary operator ':'");
                        else
                        {
                            Type output_type = Util_cobra.EnglobingType(expr_yes.output_type, expr_no.output_type);
                            if (output_type == null)
                            {
                                signal.reader.Error($"both expression after '?' operator must return something");
                                goto failure;
                            }

                            ast = new AstTernaryOperator(ast, expr_yes, expr_no, output_type);
                            return true;
                        }
                    }
                }
            }

        failure:
            signal.reader.Error("could not parse expression");
            ast = null;
            return false;
        }
    }
}