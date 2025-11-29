namespace _ZOA_
{
    sealed partial class ZoaShell : Shell
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

        //----------------------------------------------------------------------------------------------------------

        public override void OnSignal(in Signal signal)
        {
            if (front_execution != null)
                FrontTick(signal);
            else
            {
                ExecutionStack exec_stack = new();
                if (TryParseProgram(signal, new MemScope(mem_scope), exec_stack, out bool background))
                    if (signal.flags.HasFlag(SIG_FLAGS.SUBMIT))
                        if (background)
                            background_executions.Add(exec_stack);
                        else
                        {
                            front_execution = exec_stack;
                            status.Value = CMD_STATUS.BLOCKED;
                        }
            }
        }
    }
}