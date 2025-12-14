using _ZOA_.Ast.compilation;
using System;

namespace _ZOA_
{
    internal class AstContract : AstExpression
    {

        //----------------------------------------------------------------------------------------------------------

        public AstContract(in Type output_type) : base(output_type)
        {
        }

        //----------------------------------------------------------------------------------------------------------

        public static bool TryParseContract(in Signal signal, in TScope tscope, in TStack tstack, in Type expected_type, out AstContract ast_contract)
        {
            ast_contract = null;
            return false;
        }
    }
}