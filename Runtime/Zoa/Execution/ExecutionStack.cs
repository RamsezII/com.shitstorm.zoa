using System.Collections.Generic;

namespace _ZOA_
{
    public sealed class ExecutionStack
    {
        internal readonly List<Executor> _stack = new();

        //----------------------------------------------------------------------------------------------------------

        internal bool OnSignal(in Signal signal, out ExecutionOutput output)
        {
            for (int i = 0; i < _stack.Count; ++i)
                if (!_stack[i].isDone)
                {
                    output = _stack[i].OnSignal(signal);
                    return true;
                }
            output = default;
            return false;
        }
    }
}