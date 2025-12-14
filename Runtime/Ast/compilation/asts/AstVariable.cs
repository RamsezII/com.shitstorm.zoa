using System;

namespace _ZOA_.Ast.compilation
{
    internal class AstVariable : AstExpression
    {
        readonly string var_name;

        //----------------------------------------------------------------------------------------------------------

        public AstVariable(in string var_name, in Type output_type) : base(output_type)
        {
        }

        //----------------------------------------------------------------------------------------------------------

        public static bool TryParseVariable(in Signal signal, in TScope tscope, in TStack tstack, in Type expected_type, out AstVariable ast_variable)
        {
            ast_variable = null;
            return false;
        }
    }
}