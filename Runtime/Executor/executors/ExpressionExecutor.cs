namespace _ZOA_
{
    public abstract class ExpressionExecutor : Executor
    {
        protected ExpressionExecutor(in Signal signal, in MemScope scope) : base(signal, scope)
        {
        }
    }
}