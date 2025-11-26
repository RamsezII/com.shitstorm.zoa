using System;

namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseOr(
            in Signal signal,
            in MemScope scope,
            in TypeStack type_stack,
            in ValueStack value_stack,
            out ZoaExecutor executor
        )
        {
            if (TryParseAnd(signal, scope, type_stack, value_stack, out executor))
            {
                Type type_a = type_stack.Pop();
                if (signal.reader.TryReadString_match_out(out string op_name, as_function_argument: false, lint: signal.reader.lint_theme.keywords, match: "or"))
                {
                    if (TryParseOr(signal, scope, type_stack, value_stack, out var or2))
                    {
                        Type type_b = type_stack.Pop();
                        var or1 = executor;
                        if (TryParsePair(signal, type_stack, value_stack, OP_FLAGS.OR, or1, type_a, or2, type_b, out executor))
                            return true;
                    }
                    else
                        signal.reader.Stderr($"expected expression after '{op_name}' operator.");
                }
                else
                    return true;
            }

            return false;
        }
    }
}