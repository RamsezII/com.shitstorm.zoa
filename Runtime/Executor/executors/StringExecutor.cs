using System.Collections.Generic;
using System.Text;

namespace _ZOA_
{
    internal sealed class StringExecutor : ExpressionExecutor
    {
        readonly List<Executor> stack;

        //----------------------------------------------------------------------------------------------------------

        public StringExecutor(in Signal signal, in MemScope scope, in List<Executor> stack) : base(signal ,scope)
        {
            this.stack = stack;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<ExecutionOutput> EExecution()
        {
            StringBuilder sb = new();

            for (int i = 0; i < stack.Count; i++)
            {
                Executor exe = stack[i];
                while (!exe.isDone)
                {
                    ExecutionOutput output = exe.OnSignal(signal);
                    if (exe.isDone)
                        sb.Append(output.data.ToString());
                    else
                        yield return new(status: output.status, progress: output.progress);
                }
            }

            yield return new(CMD_STATUS.RETURN, data: sb.ToString(), progress: 1);
        }
    }
}