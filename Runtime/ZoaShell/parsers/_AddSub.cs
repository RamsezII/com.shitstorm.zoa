namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseAddSub(in Signal signal, in MemScope scope, out ExpressionExecutor executor)
        {
            if (TryParseTerm(signal, scope, out executor))
            {
                int read_old = signal.reader.read_i;
                if (signal.reader.TryReadChar_matches_out(out char op_symbol, true, "+-"))
                {
                    signal.reader.LintToThisPosition(signal.reader.lint_theme.operators, true);

                    OP_FLAGS code = op_symbol switch
                    {
                        '+' => OP_FLAGS.add,
                        '-' => OP_FLAGS.sub,
                        _ => 0,
                    };

                    if (TryParseAddSub(signal, scope, out var term2))
                    {
                        executor = new PairExecutor(signal, scope, code, executor, term2);
                        return true;
                    }
                    else
                    {
                        signal.reader.Stderr($"expected expression after '{op_symbol}' operator.");
                        return false;
                    }
                }
                else
                {
                    signal.reader.read_i = read_old;
                    return true;
                }
            }
            return false;
        }
    }
}