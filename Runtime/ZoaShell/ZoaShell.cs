using System.Collections.Generic;
using UnityEngine;

namespace _ZOA_
{
    public sealed class ZoaShell : Shell
    {
        public readonly Queue<Executor> executors = new();

        //----------------------------------------------------------------------------------------------------------

        public override void OnSignal(in Signal signal)
        {
            if (signal.reader.TryReadArgument(out string arg0, false, Color.white))
                if (Contract.contracts.TryGetValue(arg0, out Contract contract))
                {
                    Executor exe = new(signal, contract);
                    if (signal.flags.HasFlag(SIG_FLAGS.EXEC))
                        if (signal.reader.sig_error == null)
                        {
                            status.Value = STATUS.BLOCKED;
                            executors.Enqueue(exe);
                        }
                }
                else
                    signal.reader.sig_error ??= $"no method or namespace named \"{arg0}\"";
            else
                signal.reader.sig_error ??= $"could not parse argument \"{arg0}\"";
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnTick()
        {
            if (executors.Count == 0)
                return;

            const int MAX_LOOPS = 100;
            int loops = 0;
            bool run = true;

            while (run && executors.Count > 0 && loops < MAX_LOOPS)
            {
                Executor exe = executors.Peek();
                if (exe.contract.function != null)
                {
                    ZoaData output = exe.contract.function(exe);

                    if (output.data != null)
                    {
                        string text = output.data.ToString();
                        on_output?.Invoke(text, text);
                    }

                    exe.Dispose();
                    executors.Dequeue();
                }
                else if (exe.routine.MoveNext())
                    run = false;
                else
                {
                    if (exe.routine.Current.data != null)
                    {
                        string text = exe.routine.Current.data.ToString();
                        on_output?.Invoke(text, text);
                    }
                    exe.Dispose();
                    executors.Dequeue();
                }

                if (executors.Count == 0)
                    status.Value = STATUS.WAIT_FOR_STDIN;
            }
        }
    }
}