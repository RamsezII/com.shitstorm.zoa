namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseComparison(in Signal signal, in MemScope scope, out ExpressionExecutor executor)
        {
            if (TryParseAddSub(signal, scope, out executor))
            {
                if (!signal.reader.TryReadChar_matches_out(out char op_char, true, "!<>="))
                    return true;
                else
                {
                    OP_FLAGS code = op_char switch
                    {
                        '<' => OP_FLAGS.lt,
                        '>' => OP_FLAGS.gt,
                        '=' when signal.reader.TryReadChar_match('=', lint: signal.reader.lint_theme.operators, skippables: null) => OP_FLAGS.eq,
                        '!' when signal.reader.TryReadChar_match('=', lint: signal.reader.lint_theme.operators, skippables: null) => OP_FLAGS.neq,
                        _ => 0,
                    };

                    if (code == 0)
                        return false;

                    signal.reader.LintToThisPosition(signal.reader.lint_theme.operators, true);

                    if (TryParseAddSub(signal, scope, out var addsub2))
                    {
                        executor = new PairExecutor(signal, scope, code, executor, addsub2);
                        return true;
                    }
                    else
                    {
                        signal.reader.Stderr($"expected expression after '{op_char}' operator.");
                        return false;
                    }
                }
            }
            return false;
        }
    }
}