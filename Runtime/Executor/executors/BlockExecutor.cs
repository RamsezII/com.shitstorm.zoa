using System.Collections.Generic;

namespace _ZOA_
{
    public sealed class BlockExecutor : Executor
    {
        internal readonly List<Executor> stack = new();

        //----------------------------------------------------------------------------------------------------------

        internal BlockExecutor(in Signal signal, in MemScope scope) : base(signal, scope)
        {
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<ExecutionOutput> EExecution()
        {
            for (int i = 0; i < stack.Count; i++)
            {
                Executor exe = stack[i];
                while (!exe.isDone)
                    yield return exe.OnSignal(signal);
            }
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();
            for (int i = 0; i < stack.Count; i++)
                stack[i].Dispose();
            stack.Clear();
        }
    }
}