using _COBRA_;
using System.Collections.Generic;

namespace _ZOA_
{
    sealed partial class ZoaShell : Shell
    {
        readonly List<Executor> background_executors = new();
        Executor front_executor;

        //----------------------------------------------------------------------------------------------------------

        protected override void OnTick()
        {
            CodeReader tick_reader = new(null, false, null, null);
            Signal tick_sig = new(SIG_FLAGS.EXEC, tick_reader);

            if (background_executors.Count > 0)
                for (int i = 0; i < background_executors.Count; ++i)
                {
                    Executor exe = background_executors[i];
                    ExecutionOutput output = exe.OnSignal(tick_sig);
                    if (!exe.isDone)
                    {
                        background_executors.RemoveAt(i--);
                        if (background_executors.Count == 0)
                            on_output?.Invoke(output.data, output.lint);
                        exe.Dispose();
                    }
                }

            if (front_executor != null)
            {
                ExecutionOutput output = front_executor.OnSignal(tick_sig);
                status.Value = output.status;
                if (front_executor.isDone)
                {
                    on_output?.Invoke(output.data, output.lint);
                    front_executor.Dispose();
                    front_executor = null;
                }
            }
            else
                status.Value = CMD_STATUS.WAIT_FOR_STDIN;
        }
    }
}