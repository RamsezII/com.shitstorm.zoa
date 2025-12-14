using _ZOA_.Ast.compilation;

namespace _ZOA_
{
    internal class AstLiteral<T> : AstFactor
    {
        public T value;
        public AstLiteral(in T value) : base(typeof(T))
        {
            this.value = value;
        }
    }
}