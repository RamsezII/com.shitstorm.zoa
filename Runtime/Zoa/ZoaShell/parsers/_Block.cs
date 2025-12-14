namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseBlock(
            in Signal signal,
            in MemScope scope,
            in ExecutionStack exec_stack
        )
        {
            if (signal.reader.TryReadChar_match('{'))
            {
                signal.reader.LintOpeningBraquet();

                var sub_scope = new MemScope(scope);

                while (TryParseBlock(signal, sub_scope, exec_stack)) ;

                if (signal.reader.sig_error != null)
                    return false;

                if (signal.reader.TryReadChar_match('}', lint: signal.reader.CloseBraquetLint()))
                    return true;
                else
                    signal.reader.Error($"expected closing bracket '}}'.");
            }
            else if (TryParseInstruction(signal, scope, exec_stack))
                return true;

            return false;
        }
    }
}