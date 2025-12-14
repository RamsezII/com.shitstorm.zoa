using _ZOA_.Ast;

namespace _ZOA_
{
    public sealed class VScope : MScope<MemCell>
    {

        //----------------------------------------------------------------------------------------------------------

        internal VScope(in VScope parent) : base(parent)
        {
        }
    }
}