namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseComparison(
            in Signal signal,
            in MemScope scope,
            in ExecutionStack exec_stack
        )
        {
            if (TryParseAddSub(signal, scope, null, exec_stack))
            {
                if (!signal.reader.TryReadChar_matches_out(out char op_char, true, "!<>="))
                    return true;
                else
                {
                    OP_FLAGS code = op_char switch
                    {
                        '<' => OP_FLAGS.LESSER_THAN,
                        '>' => OP_FLAGS.GREATER_THAN,
                        '=' when signal.reader.TryReadChar_match('=', lint: signal.reader.lint_theme.operators, skippables: null) => OP_FLAGS.EQUAL,
                        '!' when signal.reader.TryReadChar_match('=', lint: signal.reader.lint_theme.operators, skippables: null) => OP_FLAGS.NOT_EQUAL,
                        _ => 0,
                    };

                    if (code == 0)
                        return false;

                    signal.reader.LintToThisPosition(signal.reader.lint_theme.operators, true);

                    Executor addsub1 = exec_stack.Peek();
                    if (TryParseAddSub(signal, scope, T_object, exec_stack))
                    {
                        Executor addsub2 = exec_stack.Peek();
                        if (TryParsePair(signal, T_object, code, addsub1, addsub2, exec_stack))
                            return true;
                    }
                    else
                        signal.reader.Stderr($"expected expression after '{op_char}' operator.");
                }
            }
            return false;
        }
    }
}