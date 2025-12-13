using _UTIL_;
using System.Collections.Generic;

namespace _ZOA_.Ast
{
    public sealed class Janitor : Disposable
    {
        readonly IEnumerator<ExecutionOutput> routine;
        internal Signal signal;

        //----------------------------------------------------------------------------------------------------------

        internal Janitor(in AstAsbtract ast)
        {
            routine = EExecution(ast);
        }

        //----------------------------------------------------------------------------------------------------------

        IEnumerator<ExecutionOutput> EExecution(AstAsbtract ast)
        {
            MemStack mem_stack = new();
            yield break;
        }

        public bool OnSignal(in Signal signal, out ExecutionOutput output)
        {
            if (!Disposed)
            {
                this.signal = signal;
                if (routine.MoveNext())
                {
                    output = routine.Current;
                    this.signal = null;
                    return true;
                }
                this.signal = null;
                Dispose();
            }
            output = default;
            return false;
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();
            routine?.Dispose();
        }
    }
}