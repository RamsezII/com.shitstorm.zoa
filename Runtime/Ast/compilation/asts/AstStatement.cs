using _ZOA_.Ast.execution;

namespace _ZOA_.Ast.compilation
{
    internal abstract class AstStatement : AstAbstract
    {
        public static bool TryStatement(in Signal signal, in TScope tscope, out AstStatement ast_statement)
        {
        skipped_comments:
            if (signal.reader.TryReadChar_match('#', lint: signal.reader.lint_theme.comments))
            {
                signal.reader.SkipUntil('\n');
                goto skipped_comments;
            }

            if (signal.reader.TryReadChar_match(';', lint: signal.reader.lint_theme.command_separators))
            {
                ast_statement = null;
                return true;
            }

            if (AstBlock.TryBlock(signal, tscope, out var ast_block))
            {
                ast_statement = ast_block;
                return true;
            }
            else if (AstExprStatement.TryExprStatement(signal, tscope, out var ast_expr))
            {
                ast_statement = ast_expr;
                return true;
            }

            ast_statement = null;
            return false;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override void OnExecutionStack(in Janitor janitor)
        {
            base.OnExecutionStack(janitor);
        }
    }

    internal sealed class AstExprStatement : AstStatement
    {
        readonly AstExpression expression;

        //----------------------------------------------------------------------------------------------------------

        AstExprStatement(in AstExpression expression)
        {
            this.expression = expression;
        }

        //----------------------------------------------------------------------------------------------------------

        public static bool TryExprStatement(in Signal signal, in TScope tscope, out AstExprStatement ast_statement)
        {
            if (AstExpression.TryExpr(signal, tscope, false, null, out var expression))
            {
                ast_statement = new AstExprStatement(expression);
                return true;
            }

            if (!signal.reader.TryReadChar_match(';', lint: signal.reader.lint_theme.command_separators))
                if (signal.reader.strict_syntax)
                {
                    signal.reader.Error($"Expected ';' at the end of statement");
                    goto failure;
                }

            failure:
            ast_statement = null;
            return false;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override void OnExecutionStack(in Janitor janitor)
        {
            base.OnExecutionStack(janitor);
        }
    }
}