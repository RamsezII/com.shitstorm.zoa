using System;

namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseAddSub(
            in Signal signal,
            in MemScope scope,
            in Type expected_type,
            in ExecutionStack exec_stack
        )
        {
            if (TryParseTerm(signal, scope, expected_type, exec_stack))
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

                    OP_FLAGS code = op_symbol switch
                    {
                        '+' => OP_FLAGS.ADD,
                        '-' => OP_FLAGS.SUBSTRACT,
                        _ => 0,
                    };

                    var term1 = exec_stack._stack[^1];

                    if (TryParseAddSub(signal, scope, expected_type ?? typeof(object), exec_stack))
                    {
                        var term2 = exec_stack._stack[^1];
                        if (TryParsePair(signal, expected_type, code, term1, term2, exec_stack))
                            return true;
                    }
                    else
                        signal.reader.Error($"expected expression after '{op_symbol}' operator.");
                }
            }

        failure:
            return false;
        }
    }
}