using System.Collections.Generic;

namespace _ZOA_
{
    sealed partial class ZoaShell : Shell
    {
        readonly List<ExecutionStack> background_executions = new();
        ExecutionStack front_execution = new();

        //----------------------------------------------------------------------------------------------------------

        protected override void OnTick()
        {
            if (background_executions.Count > 0)
                for (int i = 0; i < background_executions.Count; i++)
                {
                    var stack = background_executions[i];
                    Signal sig1 = new(SIG_FLAGS.EXEC, null, on_output);
                    if (!stack.OnSignal(sig1, out _))
                        background_executions.RemoveAt(i--);
                }

            if (front_execution._stack.Count > 0)
            {
                Signal sig2 = new(SIG_FLAGS.EXEC, null, on_output);
                if (front_execution.OnSignal(sig2, out var output))
                    status.Value = output.status;
                else
                    status.Value = CMD_STATUS.WAIT_FOR_STDIN;
            }
            else
                status.Value = CMD_STATUS.WAIT_FOR_STDIN;
        }
    }
}