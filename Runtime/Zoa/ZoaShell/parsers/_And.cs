using System;

namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseAnd(
            in Signal signal,
            in MemScope scope,
            in Type expected_type,
            in ExecutionStack exec_stack
        )
        {
            if (TryParseComparison(signal, scope, expected_type, exec_stack))
                if (!signal.reader.TryReadString_match_out(out string op_name, as_function_argument: false, lint: signal.reader.lint_theme.keywords, match: "and"))
                    return true;
                else
                {
                    Executor and1 = exec_stack.Peek();
                    if (TryParseAnd(signal, scope, expected_type, exec_stack))
                    {
                        Executor and2 = exec_stack.Peek();
                        if (TryParsePair(signal, T_bool, OP_FLAGS.AND, and1, and2, exec_stack))
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