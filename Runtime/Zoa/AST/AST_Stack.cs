using System.Collections.Generic;

namespace _ZOA_
{
    internal sealed class AST_Stack
    {
        public readonly List<AST_Abstract> elements = new();
        public int pointer;

        //----------------------------------------------------------------------------------------------------------

        public IEnumerator<ExecutionOutput> EExecution(MemScope scope)
        {
            while (pointer < elements.Count)
            {
                yield break;

                ++pointer;
            }
        }
    }
}