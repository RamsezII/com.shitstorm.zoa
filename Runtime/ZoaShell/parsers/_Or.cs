namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseOr(in Signal signal, in MemScope scope, out ExpressionExecutor executor)
        {
            if (TryParseAnd(signal, scope, out executor))
                if (signal.reader.TryReadString_match_out(out string op_name, as_function_argument: false, lint: signal.reader.lint_theme.keywords, match: "or"))
                {
                    if (TryParseOr(signal, scope, out var or2))
                    {
                        executor = new PairExecutor(signal, scope, OP_FLAGS.or, executor, or2);
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