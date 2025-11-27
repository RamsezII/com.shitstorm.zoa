using _UTIL_;
using System;
using System.Collections.Generic;

namespace _ZOA_
{
    public sealed class Executor : Disposable
    {
        internal Action<Executor> action_SIG_EXE;
        internal IEnumerator<ExecutionOutput> routine_SIG_EXE, routine_SIG_ALL;

        internal readonly Type output_type;
        internal object output_data;
        internal bool isDone;
        public Signal signal;

        //----------------------------------------------------------------------------------------------------------

        internal Executor(in Type output_type)
        {
            this.output_type = output_type;
        }

        internal static Executor Literal(object value) => new(value.GetType())
        {
            action_SIG_EXE = exe => exe.output_data = value,
        };

        //----------------------------------------------------------------------------------------------------------

        internal ExecutionOutput OnSignal(in Signal signal)
        {
            ExecutionOutput output = new(
                status: CMD_STATUS.ERROR,
                error: $"nothing assigned to executor"
            );

            if (action_SIG_EXE != null)
            {
                this.signal = signal;
                action_SIG_EXE(this);
                this.signal = null;
                isDone = true;
                output = new(CMD_STATUS.RETURN);
            }

            if (routine_SIG_EXE != null)
                if (!signal.flags.HasFlag(SIG_FLAGS.EXEC))
                    output = new(CMD_STATUS.BLOCKED);
                else
                {
                    this.signal = signal;
                    if (!routine_SIG_EXE.MoveNext())
                        isDone = true;
                    this.signal = null;
                    output = routine_SIG_EXE.Current;
                }

            if (routine_SIG_ALL != null)
            {
                this.signal = signal;
                if (!routine_SIG_ALL.MoveNext())
                    isDone = true;
                this.signal = null;
                output = routine_SIG_ALL.Current;
            }

            if (isDone)
                if (output_type != null)
                    if (output_data == null)
                        output = new(CMD_STATUS.ERROR, error: $"no output, expected {output_type}");
                    else if (output_type != output_data.GetType())
                        output = new(CMD_STATUS.ERROR, error: $"wrong output, expected {output_type}, got {output_data} ({output_data.GetType()})");

            return output;
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