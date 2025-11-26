namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseInstruction(
            in Signal signal,
            in MemScope scope,
            in TypeStack type_stack,
            in ValueStack value_stack,
            out Executor executor
        )
        {
            executor = null;
        skipped_comments:
            if (signal.reader.TryReadChar_match(';', lint: signal.reader.lint_theme.command_separators))
                return true;
            else if (signal.reader.TryReadChar_match('#', lint: signal.reader.lint_theme.comments))
            {
                signal.reader.SkipUntil('\n');
                goto skipped_comments;
            }
            else if (TryParseExpression(signal, scope, type_stack, value_stack, false, out executor))
                return true;

            executor = null;
            return false;
        }
    }
}