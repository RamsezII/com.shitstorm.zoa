using System.Collections.Generic;

namespace _ZOA_.Ast
{
    public sealed class MemStack
    {
        internal readonly List<MemCell> _stack = new();

        //----------------------------------------------------------------------------------------------------------

        public MemCell Pop()
        {
            MemCell cell = _stack[^1];
            _stack.RemoveAt(_stack.Count - 1);
            return cell;
        }
    }
}