using System.Collections.Generic;

namespace _ZOA_
{
    public sealed class ExecutionStack
    {
        internal readonly List<Executor> _stack = new();

        //----------------------------------------------------------------------------------------------------------

        internal void Push(in Executor executor) => _stack.Add(executor);
        internal void Insert(in Executor executor) => _stack.Insert(0, executor);
        internal void Push(in ExecutionStack stack) => _stack.AddRange(stack._stack);
        internal void Insert(in ExecutionStack stack) => _stack.InsertRange(0, stack._stack);

        internal Executor Peek() => _stack[^1];

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