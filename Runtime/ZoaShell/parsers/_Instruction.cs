namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseInstruction(in Signal signal, in MemScope scope, in bool check_semicolon, out Executor executor)
        {
        skipped_comments:
            if (signal.reader.TryReadChar_match(';', lint: signal.reader.lint_theme.command_separators))
            {
                executor = null;
                return true;
            }
            else if (signal.reader.TryReadChar_match('#', lint: signal.reader.lint_theme.comments))
            {
                signal.reader.SkipUntil('\n');
                goto skipped_comments;
            }
            else if (TryParseExpression(signal, scope, false, out var exe_expr))
            {
                executor = exe_expr;
                return true;
            }

            executor = null;
            return false;
        }
    }
}