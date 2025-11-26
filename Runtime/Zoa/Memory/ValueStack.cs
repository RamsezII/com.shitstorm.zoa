using System.Collections.Generic;

namespace _ZOA_
{
    public sealed partial class ValueStack
    {
        internal readonly List<object> _stack = new();

        //----------------------------------------------------------------------------------------------------------

        public void Push<T>(in T value) => _stack.Add(value);
        public object Pop() => Pop<object>();
        public object Peek() => _stack[^1];
        public T Peek<T>() => (T)_stack[^1];
        public T Pop<T>()
        {
            T value = (T)_stack[^1];
            _stack.RemoveAt(_stack.Count - 1);
            return value;
        }
    }
}