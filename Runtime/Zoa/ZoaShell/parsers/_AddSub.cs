using System;

namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseAddSub(
            in Signal signal,
            in MemScope scope,
            in TypeStack type_stack,
            in ValueStack value_stack,
            in Type expected_type,
            out ZoaExecutor executor
        )
        {
            if (TryParseTerm(signal, scope, type_stack, value_stack, expected_type, out executor))
            {
                int read_old = signal.reader.read_i;
                if (!signal.reader.TryReadChar_matches_out(out char op_symbol, true, "+-"))
                {
                    signal.reader.read_i = read_old;
                    return true;
                }
                else
                {
                    signal.reader.LintToThisPosition(signal.reader.lint_theme.operators, true);

                    Type type_a = type_stack.Pop();

                    OP_FLAGS code = op_symbol switch
                    {
                        '+' => OP_FLAGS.ADD,
                        '-' => OP_FLAGS.SUBSTRACT,
                        _ => 0,
                    };

                    if (TryParseAddSub(signal, scope, type_stack, value_stack, expected_type, out var term2))
                    {
                        Type type_b = type_stack.Pop();
                        var term1 = executor;
                        if (TryParsePair(signal, type_stack, value_stack, expected_type, code, term1, type_a, term2, type_b, out executor))
                            return true;
                    }
                    else
                        signal.reader.Stderr($"expected expression after '{op_symbol}' operator.");
                }
            }
            return false;
        }
    }
}