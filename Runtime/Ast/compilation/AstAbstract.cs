using _ZOA_.Ast.execution;
using System;

namespace _ZOA_.Ast.compilation
{
    internal abstract class AstAbstract
    {
        public readonly Type output_type;

        //----------------------------------------------------------------------------------------------------------

        protected AstAbstract(in Type output_type)
        {
            this.output_type = output_type;
        }

        //----------------------------------------------------------------------------------------------------------

        internal virtual void OnExecution(in Janitor janitor)
        {
        }
    }
}