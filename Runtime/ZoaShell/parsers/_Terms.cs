using System;

namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseTerm(
            in Signal signal,
            in MemScope scope,
            in TypeStack type_stack,
            in ValueStack value_stack,
            out Executor executor
        )
        {
            if (!TryParseUnary(signal, scope, type_stack, value_stack, out executor))
                return false;

            if (!signal.reader.TryReadChar_matches_out(out char op_char, true, "*/%"))
                return true;
            else
            {
                Type type_a = type_stack.Pop();

                OP_FLAGS code = op_char switch
                {
                    '*' => OP_FLAGS.MULTIPLY,
                    '/' => OP_FLAGS.DIVIDE,
                    '%' => OP_FLAGS.MODULO,
                    _ => 0,
                };

                if (TryParseTerm(signal, scope, type_stack, value_stack, out var expr2))
                {
                    Type type_b = type_stack.Pop();
                    var expr1 = executor;
                    if (TryParsePair(signal, type_stack, value_stack, code, expr1, type_a, expr2, type_b, out executor))
                        return true;
                }
                else
                    signal.reader.Stderr($"expected expression after '{op_char}' operator.");
            }

            return false;
        }
    }
}