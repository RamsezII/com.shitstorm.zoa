using _UTIL_;
using System;
using System.Collections.Generic;

namespace _ZOA_
{
    public sealed class Executor : Disposable
    {
        internal Action<Executor> action_SIG_EXE;
        internal IEnumerator<ExecutionOutput> routine_SIG_EXE, routine_SIG_ALL;

        public readonly string name;
        internal readonly Type type;
        internal object output;
        internal bool isDone;
        public Signal signal;

        public string DisplayName => $"ex {{ {nameof(name)}: {name}, {nameof(type)}: {type}, {nameof(output)}: {output} }}";
        public override string ToString() => DisplayName;

        //----------------------------------------------------------------------------------------------------------

        internal Executor(in string name, in Type type)
        {
            this.name = name;
            this.type = type;
        }

        internal static Executor Literal(object value) => new($"lit[{value}]", value.GetType())
        {
            action_SIG_EXE = exe => exe.output = value,
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
                if (type != null)
                    if (this.output == null)
                        output = new(CMD_STATUS.ERROR, error: $"no output, expected {type}");
                    else if (type != this.output.GetType())
                        output = new(CMD_STATUS.ERROR, error: $"wrong output, expected {type}, got {this.output} ({this.output.GetType()})");

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