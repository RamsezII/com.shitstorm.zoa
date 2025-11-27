namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseInstruction(
            in Signal signal,
            in MemScope scope,
            in ExecutionStack exec_stack
        )
        {
        skipped_comments:
            if (signal.reader.TryReadChar_match(';', lint: signal.reader.lint_theme.command_separators))
                return true;
            else if (signal.reader.TryReadChar_match('#', lint: signal.reader.lint_theme.comments))
            {
                signal.reader.SkipUntil('\n');
                goto skipped_comments;
            }
            else if (TryParseExpression(signal, scope, false, null, exec_stack))
                return true;

            return false;
        }
    }
}