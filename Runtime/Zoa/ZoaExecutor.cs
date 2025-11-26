using _UTIL_;
using System;
using System.Collections.Generic;

namespace _ZOA_
{
    public sealed class ZoaExecutor : Disposable
    {
        internal Action<ZoaExecutor> action_SIG_EXE;
        internal IEnumerator<ExecutionOutput> routine_SIG_EXE, routine_SIG_ALL;

        internal bool isDone;
        public Signal signal;

        //----------------------------------------------------------------------------------------------------------

        internal ZoaExecutor()
        {
        }

        internal ZoaExecutor(in TypeStack type_stack, ValueStack value_stack, object literal)
        {
            type_stack.Push(literal.GetType());
            action_SIG_EXE = exe => value_stack.Push(literal);
        }

        //----------------------------------------------------------------------------------------------------------

        internal static IEnumerator<ExecutionOutput> EExecute_SIG_ALL(ZoaExecutor executor, List<ZoaExecutor> exe_stack)
        {
            for (int i = 0; i < exe_stack.Count; ++i)
                yield return exe_stack[i].OnSignal(executor.signal);
        }

        internal ExecutionOutput OnSignal(in Signal signal)
        {
            if (action_SIG_EXE == null && routine_SIG_EXE == null && routine_SIG_ALL == null)
                throw new Exception($"nothing assigned to executor");

            if (action_SIG_EXE != null)
            {
                this.signal = signal;
                action_SIG_EXE(this);
                this.signal = null;
                return new(CMD_STATUS.RETURN);
            }

            if (routine_SIG_EXE != null)
                if (!signal.flags.HasFlag(SIG_FLAGS.EXEC))
                    return new(CMD_STATUS.BLOCKED);
                else
                {
                    this.signal = signal;
                    if (!routine_SIG_EXE.MoveNext())
                        isDone = true;
                    this.signal = null;
                    return routine_SIG_EXE.Current;
                }

            if (routine_SIG_ALL != null)
            {
                this.signal = signal;
                if (!routine_SIG_ALL.MoveNext())
                    isDone = true;
                this.signal = null;
                return routine_SIG_ALL.Current;
            }

            throw new Exception($"should not reach here (nothing assigned to executor");
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();
            routine_SIG_EXE?.Dispose();
            routine_SIG_ALL?.Dispose();
        }
    }
}