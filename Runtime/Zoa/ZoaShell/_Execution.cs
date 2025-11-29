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
                    Signal sig1 = new(this, SIG_FLAGS.TICK, null, on_output);
                    if (!stack.OnSignal(sig1, out _))
                        background_executions.RemoveAt(i--);
                }

            if (front_execution != null)
                FrontTick(null);
            else
                status.Value = CMD_STATUS.WAIT_FOR_STDIN;
        }

        void FrontTick(in Signal signal)
        {
            Signal sig2 = signal ?? new(this, SIG_FLAGS.TICK, null, on_output);
            if (front_execution.OnSignal(sig2, out var output))
            {
                status.Value = output.status;
                prefixe.Value = output.prefixe;
            }
            else
            {
                front_execution = null;
                status.Value = CMD_STATUS.WAIT_FOR_STDIN;
                prefixe.Value = RegularPrefixe();
            }
        }
    }
}