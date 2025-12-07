using System.Collections.Generic;

namespace _ZOA_
{
    public sealed class ExecutionQueue
    {
        public Signal signal;
        internal readonly AST_Abstract ast;
        internal readonly MemStack mem_stack = new();
        internal readonly Queue<Executor> _queue = new();

        //----------------------------------------------------------------------------------------------------------

        public IEnumerator<ExecutionOutput> EExecution()
        {
            while (true)
                if (signal == null)
                    yield return new(CMD_STATUS.ERROR, error: $"please arm the signal before each tick.");
                else
                {
                    if (_queue.TryDequeue(out Executor executor))
                        ;
                    signal = null;
                }
        }
    }
}