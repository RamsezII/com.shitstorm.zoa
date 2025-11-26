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
            Signal signal = new(SIG_FLAGS.EXEC, null, on_output);
            OnBackgroundSignal(signal);
            OnFrontSignal(signal);
        }

        void OnBackgroundSignal(in Signal signal)
        {
            if (background_executors.Count > 0)
                for (int i = 0; i < background_executors.Count; ++i)
                {
                    Executor exe = background_executors[i];
                    exe.OnSignal(signal);

                    if (!exe.isDone)
                    {
                        background_executors.RemoveAt(i--);
                        exe.Dispose();
                        return;
                    }
                }
        }

        void OnFrontSignal(in Signal signal)
        {
            if (front_executor != null)
            {
                ExecutionOutput output = front_executor.OnSignal(signal);
                status.Value = output.status;

                if (front_executor.isDone)
                {
                    front_executor.Dispose();
                    front_executor = null;
                }
            }
            else
                status.Value = CMD_STATUS.WAIT_FOR_STDIN;
        }
    }
}