namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseBlock(in Signal signal, in MemScope scope, out Executor executor)
        {
            if (signal.reader.TryReadChar_match('{'))
            {
                signal.reader.LintOpeningBraquet();

                var sub_scope = new MemScope(scope);
                var body = new BlockExecutor(signal, sub_scope);

                while (TryParseBlock(signal, sub_scope, out Executor exe))
                    if (exe != null)
                        body.stack.Add(exe);

                if (signal.reader.sig_error != null)
                {
                    executor = null;
                    return false;
                }

                if (signal.reader.TryReadChar_match('}', lint: signal.reader.CloseBraquetLint()))
                {
                    executor = body;
                    return true;
                }
                else
                    signal.reader.Stderr($"expected closing bracket '}}'.");
            }
            else if (TryParseInstruction(signal, scope, true, out executor))
                return true;

            executor = null;
            return false;
        }
    }
}