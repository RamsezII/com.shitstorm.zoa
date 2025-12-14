using System;
using UnityEngine;

namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseUnary(
            in Signal signal,
            MemScope scope,
            in Type expected_type,
            in ExecutionStack exec_stack
        )
        {
            int read_old = signal.reader.read_i;

            if (!signal.reader.TryReadChar_matches_out(out char unary_operator, true, "+-!"))
            {
                if (TryParseFactor(signal, scope, expected_type, exec_stack))
                    return true;
                else
                {
                    signal.reader.Error($"could not parse factor");
                    read_old = signal.reader.read_i;
                    goto failure;
                }
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
                {
                    read_old = signal.reader.read_i;
                    if (!signal.reader.TryReadArgument(out string var_name, false, signal.reader.lint_theme.variables, skippables: null))
                    {
                        signal.reader.Error($"expected variable after increment operator '{unary_operator}{unary_operator}'.");
                        goto failure;
                    }
                    else if (!scope.TryGetCell(var_name, out MemCell var_cell))
                    {
                        signal.reader.Error($"no variable named '{var_name}'.");
                        goto failure;
                    }

                    Debug.LogError("TODO ++i");
                    signal.reader.Error($"++i TODO");
                    goto failure;
                }
                else if (!TryParseFactor(signal, scope, expected_type, exec_stack))
                {
                    signal.reader.Error($"expected factor after '{unary_operator}'.");
                    return false;
                }
                else
                {
                    var factor = exec_stack._stack[^1];

                    Executor executor = new("unary", expected_type);
                    exec_stack._stack.Add(executor);

                    if (signal.arm_executors)
                        executor.action_SIG_EXE = exe =>
                        {
                            exe.output = factor.output switch
                            {
                                bool b => code switch
                                {
                                    OP_CODES.NOT => !b,
                                    _ => throw new Exception(),
                                },
                                int i => code switch
                                {
                                    OP_CODES.ADD => i,
                                    OP_CODES.SUBSTRACT => -i,
                                    _ => throw new Exception(),
                                },
                                float f => code switch
                                {
                                    OP_CODES.ADD => f,
                                    OP_CODES.SUBSTRACT => -f,
                                    _ => throw new Exception(),
                                },
                                _ => throw new NotImplementedException(),
                            };
                        };

                    return true;
                }
            }

        failure:
            signal.reader.read_i = read_old;
            return false;
        }
    }
}