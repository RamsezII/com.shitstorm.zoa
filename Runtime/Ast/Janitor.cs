using System.Collections.Generic;

namespace _ZOA_
{
    public sealed class Janitor
    {
        internal readonly List<AstContract> contract_stack = new();
        internal readonly MemStack mem_stack = new();
        internal int pointer;

        Executor current_executor;
        IEnumerator<ExecutionOutput> routine_SIG_EXE, routine_SIG_ALL;

        //----------------------------------------------------------------------------------------------------------

        public void OnSignal(in Signal signal)
        {
            if (routine_SIG_EXE != null)
            {
                current_executor.signal = signal;
                if (signal.flags.HasFlag(SIG_FLAGS.TICK))
                    if (!routine_SIG_EXE.MoveNext())
                        routine_SIG_EXE = null;
                current_executor.signal = null;
            }

            if (routine_SIG_ALL != null)
            {
                current_executor.signal = signal;
                if (!routine_SIG_ALL.MoveNext())
                    routine_SIG_ALL = null;
                current_executor.signal = null;
            }

            if (pointer < contract_stack.Count)
            {
                AstContract contract = contract_stack[pointer];
            }
        }
    }
}