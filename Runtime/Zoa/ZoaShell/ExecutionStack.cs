using System.Collections.Generic;

namespace _ZOA_
{
    sealed class ExecutionStack
    {
        internal readonly List<Executor> _stack = new();
        internal bool is_empty;

        //----------------------------------------------------------------------------------------------------------

        internal void Push(in Executor executor) => _stack.Add(executor);

        internal Executor Peek() => _stack[^1];

        internal bool OnSignal(in Signal signal, out ExecutionOutput output)
        {
            if (!is_empty)
                for (int i = _stack.Count - 1; i >= 0; i--)
                    if (!_stack[i].isDone)
                    {
                        output = _stack[i].OnSignal(signal);
                        return true;
                    }

            is_empty = true;
            output = default;
            return false;
        }
    }
}