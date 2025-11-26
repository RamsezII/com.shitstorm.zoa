using System;

namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseComparison(
            in Signal signal,
            in MemScope scope,
            in TypeStack type_stack,
            in ValueStack value_stack,
            out ZoaExecutor executor
        )
        {
            if (TryParseAddSub(signal, scope, type_stack, value_stack, out executor))
            {
                if (!signal.reader.TryReadChar_matches_out(out char op_char, true, "!<>="))
                    return true;
                else
                {
                    Type type_a = type_stack.Pop();

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

                    if (TryParseAddSub(signal, scope, type_stack, value_stack, out var addsub2))
                    {
                        Type type_b = type_stack.Pop();
                        var addsub1 = executor;
                        if (TryParsePair(signal, type_stack, value_stack, code, addsub1, type_a, addsub2, type_b, out executor))
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