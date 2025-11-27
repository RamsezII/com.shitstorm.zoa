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
            in bool read_as_argument,
            Type expected_type,
            ExecutionStack exec_stack
        )
        {
            int exe_count_bf_assign = exec_stack._stack.Count;
            if (!TryParseAssignation(signal, scope, expected_type, exec_stack) && signal.reader.sig_error != null)
                return false;

            if (exe_count_bf_assign == exec_stack._stack.Count && !TryParseOr(signal, scope, exec_stack))
                return false;

            if (read_as_argument)
                if (!signal.reader.TryReadChar_match(',', lint: signal.reader.lint_theme.argument_coma) && !signal.reader.TryPeekChar_match(')', out _))
                    if (signal.reader.strict_syntax)
                    {
                        signal.reader.Stderr($"expected ',' or ')' after expression");
                        return false;
                    }

            if (!signal.reader.TryReadChar_match('?', lint: signal.reader.lint_theme.operators))
                return true;
            else
            {
                // cond output
                Executor exe_cond = exec_stack.Peek();
                // check if is bool

                ExecutionStack exec_stack_yes = new();

                if (!TryParseExpression(signal, scope, false, typeof(object), exec_stack_yes))
                    signal.reader.Stderr($"expected expression after ternary operator '?'");
                else
                {
                    Executor exe_yes = exec_stack.Peek();

                    if (!signal.reader.TryReadChar_match(':', lint: signal.reader.lint_theme.operators))
                        signal.reader.Stderr($"expected ternary operator delimiter ':'");
                    else
                    {
                        ExecutionStack exec_stack_no = new();

                        if (!TryParseExpression(signal, scope, false, typeof(object), exec_stack_no))
                            signal.reader.Stderr($"expected second expression after ternary operator ':'");
                        else if (signal.is_exec)
                        {
                            Executor exe_no = exec_stack.Peek();
                            Executor exe_tern_eval = null;

                            exe_tern_eval = new("ternary evaluator", expected_type)
                            {
                                action_SIG_EXE = exe =>
                                {
                                    bool cond_b = exe_cond.output.ToBool();

                                    var exec_stack_final = cond_b ? exec_stack_yes : exec_stack_no;

                                    int insert_index = exec_stack._stack.IndexOf(exe_tern_eval);

                                    exec_stack._stack.InsertRange(insert_index, exec_stack_final._stack);

                                    Executor exe_tern_output = new("ternary output", expected_type)
                                    {
                                        action_SIG_EXE = exe =>
                                        {
                                            exe.output = exec_stack_final.Peek().output;
                                        },
                                    };
                                    exec_stack._stack.Insert(insert_index, exe_tern_output);
                                }
                            };
                            exec_stack.Push(exe_tern_eval);
                        }
                    }
                }
            }

            return false;
        }
    }
}