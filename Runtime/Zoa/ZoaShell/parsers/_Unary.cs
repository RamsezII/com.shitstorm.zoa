using System;

namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseUnary(
            in Signal signal,
            MemScope scope,
            in TypeStack type_stack,
            ValueStack value_stack,
            out ZoaExecutor executor
        )
        {
            int read_old = signal.reader.read_i;

            if (!signal.reader.TryReadChar_matches_out(out char unary_operator, true, "+-!"))
            {
                if (TryParseFactor(signal, scope, type_stack, value_stack, out executor))
                    return true;
            }
            else
            {
                OP_CODES code = unary_operator switch
                {
                    '+' => OP_CODES.ADD,
                    '-' => OP_CODES.SUBSTRACT,
                    '!' => OP_CODES.NOT,
                    _ => 0,
                };

                if ((code == OP_CODES.ADD || code == OP_CODES.SUBSTRACT) && signal.reader.TryReadChar_match(unary_operator, signal.reader.lint_theme.operators, skippables: null))
                    ;
                else
                {
                    if (!signal.reader.TryReadArgument(out string var_name, false, signal.reader.lint_theme.variables, skippables: null))
                        signal.reader.Stderr($"expected variable after increment operator '{unary_operator}{unary_operator}'.");
                    else if (!scope.TryGetCell(var_name, out MemCell cell))
                        signal.reader.Stderr($"no variable named '{var_name}'.");
                    else
                    {
                        type_stack.Push(cell.type);

                        executor = new();

                        if (signal.flags.HasFlag(SIG_FLAGS.EXEC))
                            executor.action_SIG_EXE = exe =>
                            {
                                switch (cell.value)
                                {
                                    case bool b:
                                        value_stack.Push(code switch
                                        {
                                            OP_CODES.NOT => !b,
                                            _ => throw new Exception(),
                                        });
                                        break;

                                    case int i:
                                        value_stack.Push(code switch
                                        {
                                            OP_CODES.ADD => i,
                                            OP_CODES.SUBSTRACT => -i,
                                            _ => throw new Exception(),
                                        });
                                        break;

                                    case float f:
                                        value_stack.Push(code switch
                                        {
                                            OP_CODES.ADD => f,
                                            OP_CODES.SUBSTRACT => -f,
                                            _ => throw new Exception(),
                                        });
                                        break;

                                    default:
                                        throw new NotImplementedException();
                                }
                            };

                        return true;
                    }
                    goto failure;
                }

                if (TryParseFactor(signal, scope, type_stack, value_stack, out executor))
                    return true;
                else
                {
                    signal.reader.Stderr($"expected factor after '{unary_operator}'.");
                    return false;
                }
            }

        failure:
            signal.reader.read_i = read_old;
            executor = null;
            return false;
        }
    }
}