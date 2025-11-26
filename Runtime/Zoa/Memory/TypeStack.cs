using System;
using System.Collections.Generic;

namespace _ZOA_
{
    public sealed partial class TypeStack
    {
        readonly List<Type> _stack = new();

        //----------------------------------------------------------------------------------------------------------

        public void Push(in Type type) => _stack.Add(type);

        public Type Pop()
        {
            Type type = _stack[^1];
            _stack.RemoveAt(_stack.Count - 1);
            return type;
        }

        public bool TryPop(out Type type)
        {
            if (_stack.Count == 0)
            {
                type = null;
                return false;
            }
            type = Pop();
            return true;
        }
    }
}