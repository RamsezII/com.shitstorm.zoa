namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseOr(
            in Signal signal,
            in MemScope scope,
            in ExecutionStack exec_stack
        )
        {
            if (TryParseAnd(signal, scope, exec_stack))
            {
                Executor or1 = exec_stack.Peek();
                if (signal.reader.TryReadString_match_out(out string op_name, as_function_argument: false, lint: signal.reader.lint_theme.keywords, match: "or"))
                {
                    if (TryParseOr(signal, scope, exec_stack))
                    {
                        Executor or2 = exec_stack.Peek();
                        if (TryParsePair(signal, T_bool, OP_FLAGS.OR, or1, or2, exec_stack))
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