namespace _ZOA_
{
    internal abstract class AST_If_Else : AST_Abstract
    {
        AST_expression ast_cond;
        AST_Block ast_if_block, ast_else_block;

        //----------------------------------------------------------------------------------------------------------

        public override void OnExecution(in ExecutionQueue execution)
        {
            throw new System.NotImplementedException();
        }
    }
}