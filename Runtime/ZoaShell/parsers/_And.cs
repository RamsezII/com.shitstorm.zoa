namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseAnd(in Signal signal, in MemScope scope, out ExpressionExecutor executor)
        {
            if (TryParseComparison(signal, scope, out executor))
                if (signal.reader.TryReadString_match_out(out string op_name, as_function_argument: false, lint: signal.reader.lint_theme.keywords, match: "and"))
                {
                    if (TryParseAnd(signal, scope, out var and2))
                    {
                        executor = new PairExecutor(signal, scope, OP_FLAGS.and, executor, and2);
                        return true;
                    }
                    else
                    {
                        signal.reader.Stderr($"expected expression after '{op_name}' operator.");
                        return false;
                    }
                }
                else
                    return true;
            return false;
        }
    }
}