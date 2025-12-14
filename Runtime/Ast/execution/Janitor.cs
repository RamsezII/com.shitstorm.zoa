using _UTIL_;
using _ZOA_.Ast.compilation;
using System.Collections.Generic;

namespace _ZOA_.Ast.execution
{
    public sealed class Janitor : Disposable
    {
        readonly IEnumerator<ExecutionOutput> routine;
        internal Signal signal;
        internal VScope vscope;

        //----------------------------------------------------------------------------------------------------------

        internal Janitor(in AstProgram program)
        {
            routine = EExecution(program);
        }

        //----------------------------------------------------------------------------------------------------------

        IEnumerator<ExecutionOutput> EExecution(AstProgram program)
        {
            VStack vstack = new();
            VScope vscope = new(parent: null);

            for (int i = 0; i < program.asts.Count; ++i)
            {
                var block = program.asts[i];
            }

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