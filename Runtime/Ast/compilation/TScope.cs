using System;

namespace _ZOA_.Ast.compilation
{
    public sealed class TScope : MScope<Type>
    {

        //----------------------------------------------------------------------------------------------------------

        internal TScope(in TScope parent) : base(parent)
        {
        }
    }
}