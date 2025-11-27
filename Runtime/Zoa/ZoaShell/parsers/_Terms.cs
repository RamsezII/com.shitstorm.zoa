using System;

namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseTerm(
            in Signal signal,
            in MemScope scope,
            in Type expected_type,
            in ExecutionStack exec_stack
        )
        {
            if (!TryParseUnary(signal, scope, expected_type, exec_stack))
                return false;

            if (!signal.reader.TryReadChar_matches_out(out char op_char, true, "*/%"))
                return true;
            else
            {
                OP_FLAGS code = op_char switch
                {
                    '*' => OP_FLAGS.MULTIPLY,
                    '/' => OP_FLAGS.DIVIDE,
                    '%' => OP_FLAGS.MODULO,
                    _ => 0,
                };

                Executor term1 = exec_stack.Peek();

                if (TryParseTerm(signal, scope, expected_type, exec_stack))
                {
                    Executor term2 = exec_stack.Peek();
                    if (TryParsePair(signal, expected_type, code, term1, term2, exec_stack))
                        return true;
                }
                else
                    signal.reader.Stderr($"expected expression after '{op_char}' operator.");
            }

            return false;
        }
    }
}