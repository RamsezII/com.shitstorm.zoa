using System;
using System.Collections.Generic;

namespace _ZOA_
{
    partial class ZoaShell
    {
        enum IncrementOperators : byte
        {
            AddBefore,
            SubBefore,
            AddAfter,
            SubAfter,
        }

        //----------------------------------------------------------------------------------------------------------

        public bool TryParseExpression(
            in Signal signal,
            MemScope scope,
            in TypeStack type_stack,
            ValueStack value_stack,
            in bool read_as_argument,
            in Type expected_type,
            out ZoaExecutor executor
        )
        {
            if (!TryParseAssignation(signal, scope, type_stack, value_stack, out executor) && signal.reader.sig_error != null)
                return false;

            if (executor == null && !TryParseOr(signal, scope, type_stack, value_stack, out executor))
                return false;

            if (read_as_argument)
                if (!signal.reader.TryReadChar_match(',', lint: signal.reader.lint_theme.argument_coma) && !signal.reader.TryPeekChar_match(')', out _))
                    if (signal.reader.strict_syntax)
                    {
                        signal.reader.Stderr($"expected ',' or ')' after expression");
                        return false;
                    }

            if (signal.reader.TryReadChar_match('?', lint: signal.reader.lint_theme.operators))
            {
                // cond output
                Type type_predicat = type_stack.Pop();
                // check if is bool

                if (!TryParseExpression(signal, scope, type_stack, value_stack, false, typeof(object), out ZoaExecutor exe_yes))
                    signal.reader.Stderr($"expected expression after ternary operator '?'");
                else
                {
                    Type type_yes = type_stack.Pop();

                    if (!signal.reader.TryReadChar_match(':', lint: signal.reader.lint_theme.operators))
                        signal.reader.Stderr($"expected ternary operator delimiter ':'");
                    else if (!TryParseExpression(signal, scope, type_stack, value_stack, false, typeof(object), out ZoaExecutor exe_no))
                        signal.reader.Stderr($"expected second expression after ternary operator ':'");
                    else
                    {
                        Type type_no = type_stack.Pop();

                        if (type_yes.IsOfType(type_no))
                            type_stack.Push(type_no);
                        else if (type_no.IsOfType(type_yes))
                            type_stack.Push(type_yes);
                        else
                            signal.reader.sig_error ??= $"type inconsistancy between {type_yes} and {type_no}";

                        ZoaExecutor exe_cond = executor;
                        ZoaExecutor exe = executor = new();

                        if (signal.flags.HasFlag(SIG_FLAGS.EXEC))
                        {
                            executor.routine_SIG_ALL = ERoutine();
                            IEnumerator<ExecutionOutput> ERoutine()
                            {
                                bool cond_b = false;
                                while (!exe_cond.isDone)
                                {
                                    ExecutionOutput output = exe_cond.OnSignal(exe.signal);
                                    if (exe_cond.isDone)
                                        cond_b = value_stack.Pop().ToBool();
                                    else
                                        yield return output;
                                }

                                ZoaExecutor block = cond_b ? exe_yes : exe_no;
                                if (block != null)
                                    while (!block.isDone)
                                    {
                                        ExecutionOutput output = block.OnSignal(exe.signal);
                                        if (block.isDone)
                                            yield return new(CMD_STATUS.RETURN, progress: 1, error: output.error);
                                        else
                                            yield return output;
                                    }
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}