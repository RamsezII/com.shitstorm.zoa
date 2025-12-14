using _ZOA_.Ast.compilation;
using _ZOA_.Ast.execution;
using System.Collections.Generic;
using UnityEngine;

namespace _ZOA_
{
    internal sealed class AstShell : Shell
    {
        /*

            Instruction
            │
            ├── Assignation (ex: x = ...)
            │     └── Expression
            │           └── ...
            │
            └── Expression
                └── Or
                    └── And
                        └── Comparison
                            └── Addition (addition, subtraction)
                                └── Term (multiplication, division, modulo)
                                    └── Facteur
                                        ├── Littéral (nombre)
                                        ├── Variable
                                        ├── Parenthèse
                                        └── Appel de fonction

        */

        readonly List<Janitor> janitors = new();
        Janitor front_janitor;

        //----------------------------------------------------------------------------------------------------------

        public override void OnSignal(in Signal signal)
        {
            if (front_janitor != null)
            {
                front_janitor.OnSignal(signal, out ExecutionOutput output);

                if (output.status == CMD_STATUS.ERROR)
                {
                    Debug.LogError($"{this} SIG_ERROR['{signal.flags}']: \"{output.error}\"");
                    front_janitor.Dispose();
                }

                if (!front_janitor.Disposed)
                    status.Value = output.status;
                else
                {
                    front_janitor = null;
                    status.Value = CMD_STATUS.WAIT_FOR_STDIN;
                }
            }
            else
            {
                AstProgram program = new(signal);
                if (signal.flags.HasFlag(SIG_FLAGS.SUBMIT))
                    if (signal.reader.sig_error != null)
                    {
                        signal.reader.LocalizeError();
                        Debug.LogError(signal.reader.sig_long_error);
                        status.Value = CMD_STATUS.WAIT_FOR_STDIN;
                    }
                    else
                    {
                        front_janitor = new(program);
                        status.Value = CMD_STATUS.BLOCKED;
                    }
            }
        }

        protected override void OnTick()
        {
            for (int i = 0; i < janitors.Count; i++)
            {
                Signal signal = new(this, SIG_FLAGS.TICK, null, on_output);

                Janitor janitor = janitors[i];
                janitor.OnSignal(signal, out ExecutionOutput output);

                if (output.status == CMD_STATUS.ERROR)
                {
                    Debug.LogError($"{this} TICK_ERROR_bg: \"{output.error}\"");
                    janitor.Dispose();
                }

                if (janitor.Disposed)
                    janitors.RemoveAt(i--);
            }

            if (front_janitor != null && !front_janitor.Disposed)
            {
                Signal signal = new(this, SIG_FLAGS.TICK, null, on_output);
                front_janitor.OnSignal(signal, out ExecutionOutput output);

                if (output.status == CMD_STATUS.ERROR)
                {
                    Debug.LogError($"{this} TICK_ERROR_bg: \"{output.error}\"");
                    front_janitor.Dispose();
                }

                if (!front_janitor.Disposed)
                    status.Value = output.status;
                else
                {
                    front_janitor = null;
                    status.Value = CMD_STATUS.WAIT_FOR_STDIN;
                }
            }
        }
    }
}