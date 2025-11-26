using System;

namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseAnd(
            in Signal signal,
            in MemScope scope,
            in TypeStack type_stack,
            in ValueStack value_stack,
            out Executor executor
        )
        {
            if (TryParseComparison(signal, scope, type_stack, value_stack, out executor))
                if (!signal.reader.TryReadString_match_out(out string op_name, as_function_argument: false, lint: signal.reader.lint_theme.keywords, match: "and"))
                    return true;
                else
                {
                    Type type_a = type_stack.Pop();
                    if (TryParseAnd(signal, scope, type_stack, value_stack, out Executor and2))
                    {
                        Type type_b = type_stack.Pop();
                        var and1 = executor;
                        if (TryParsePair(signal, type_stack, value_stack, OP_FLAGS.AND, and1, type_a, and2, type_b, out executor))
                            return true;
                    }
                    else
                    {
                        signal.reader.Stderr($"expected expression after '{op_name}' operator.");
                        return false;
                    }
                }
            return false;
        }
    }
}