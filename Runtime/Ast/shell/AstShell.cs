using _ZOA_.Ast;
using System.Collections.Generic;
using UnityEngine;

namespace _ZOA_
{
    internal sealed partial class AstShell : Shell
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
                AstBlock ast = new();
                if (ast.TryParse(signal))
                    if (signal.flags.HasFlag(SIG_FLAGS.SUBMIT))
                    {
                        if (signal.reader.sig_error == null)
                        {
                            front_janitor = new(ast);
                            status.Value = CMD_STATUS.BLOCKED;
                        }
                        else
                        {
                            signal.reader.LocalizeError();
                            Debug.LogError(signal.reader.sig_long_error);
                            status.Value = CMD_STATUS.WAIT_FOR_STDIN;
                        }
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