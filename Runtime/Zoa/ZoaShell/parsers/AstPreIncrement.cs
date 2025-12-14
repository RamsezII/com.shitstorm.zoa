namespace _ZOA_.Ast.compilation
{
    internal class AstPreIncrement : AstExpression
    {
        readonly AstVariable ast_var;

        //----------------------------------------------------------------------------------------------------------

        internal AstPreIncrement(in AstUnary.Codes code, in AstVariable ast_var) : base(ast_var.output_type)
        {
            this.ast_var = ast_var;
        }

        //----------------------------------------------------------------------------------------------------------


    }
}