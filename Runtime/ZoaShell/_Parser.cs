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
            if (TryParseBlock(signal, new MemScope(mem_scope), new TypeStack(), new ValueStack(), out Executor executor) && signal.reader.sig_error == null)
            {
                if (signal.flags.HasFlag(SIG_FLAGS.EXEC))
                {
                    front_executor = executor;
                    OnFrontSignal(signal);
                }
            }
            else
                signal.reader.sig_error ??= $"could not parse {nameof(signal)}";
        }
    }
}