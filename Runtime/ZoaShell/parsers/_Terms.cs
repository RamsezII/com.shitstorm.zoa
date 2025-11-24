namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseTerm(in Signal signal, in MemScope scope, out ExpressionExecutor executor)
        {
            if (!TryParseUnary(signal, scope, out executor))
                return false;

            if (signal.reader.TryReadChar_matches_out(out char op_char, true, "*/%"))
            {
                OP_FLAGS code = op_char switch
                {
                    '*' => OP_FLAGS.mul,
                    '/' => OP_FLAGS.div,
                    '%' => OP_FLAGS.mod,
                    _ => 0,
                };

                if (TryParseTerm(signal, scope, out var expr2))
                {
                    executor = new PairExecutor(signal, scope, code, executor, expr2);
                    return true;
                }
                else
                {
                    signal.reader.Stderr($"expected expression after '{op_char}' operator.");
                    return false;
                }
            }
            else
                return true;
        }
    }
}