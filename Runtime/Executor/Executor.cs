using _UTIL_;
using System;
using System.Collections.Generic;

namespace _ZOA_
{
    public abstract class Executor : Disposable
    {
        public readonly MemScope mem_scope;

        protected Signal signal;
        internal readonly Type output_type;
        internal bool isDone;

        readonly IEnumerator<ExecutionOutput> routine;

        public virtual string ToLog()
        {
            return $"EXE[{disposable_id}]";
        }

        //----------------------------------------------------------------------------------------------------------

        internal Executor(in Signal signal, in MemScope mem_scope)
        {
            this.mem_scope = mem_scope;
            if (signal.flags.HasFlag(SIG_FLAGS.EXEC))
                routine = EExecution();
        }

        //----------------------------------------------------------------------------------------------------------

        internal ExecutionOutput OnSignal(in Signal signal)
        {
            this.signal = signal;
            if (!routine.MoveNext())
                isDone = true;
            this.signal = null;
            return routine.Current;
        }

        internal abstract IEnumerator<ExecutionOutput> EExecution();
    }
}