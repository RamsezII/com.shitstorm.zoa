using System;

namespace _ZOA_.Ast.compilation
{
    internal class AstUnary : AstExpression
    {
        internal enum Codes : byte
        {
            Add,
            Sub,
            Not,
            PreIncrement,
            PreDecrement,
        }

        readonly Codes code;
        readonly AstExpression ast_factor;

        //----------------------------------------------------------------------------------------------------------

        protected AstUnary(in Codes code, in AstExpression ast_factor) : base(ast_factor.output_type)
        {
            this.code = code;
            this.ast_factor = ast_factor;
        }

        //----------------------------------------------------------------------------------------------------------

        public static bool TryParseUnary(in Signal signal, in TScope tscope, in TStack tstack, in Type expected_type, out AstExpression ast_unary)
        {
            int read_old = signal.reader.read_i;

            if (signal.reader.TryReadChar_matches_out(out char op_char, true, "+-!"))
            {
                Codes code = op_char switch
                {
                    '+' => Codes.Add,
                    '-' => Codes.Sub,
                    '!' => Codes.Not,
                    _ => 0,
                };

                if ((code == Codes.Add || code == Codes.Sub) && signal.reader.TryReadChar_match(op_char, signal.reader.lint_theme.operators, skippables: null))
                {
                    read_old = signal.reader.read_i;

                    if (!signal.reader.TryReadArgument(out string var_name, false, signal.reader.lint_theme.variables, skippables: null))
                        if (AstVariable.TryParseVariable(signal, tscope, tstack, expected_type, out var ast_var))
                        {
                            ast_unary = new AstPreIncrement(code, ast_var);
                            return true;
                        }

                    signal.reader.Error($"expected variable after increment operator '{op_char}{op_char}'.");
                    goto failure;
                }
                else if (AstFactor.TryParseFactor(signal, tscope, tstack, expected_type, out var ast_factor))
                {
                    ast_unary = new AstUnary(code, ast_factor);
                    return true;
                }
                else
                {
                    signal.reader.Error($"expected factor after '{op_char}'.");
                    goto failure;
                }
            }
            else if (AstFactor.TryParseFactor(signal, tscope, tstack, expected_type, out var ast_factor))
            {
                ast_unary = ast_factor;
                return true;
            }
            else
            {
                signal.reader.Error($"could not parse factor");
                read_old = signal.reader.read_i;
                goto failure;
            }

        failure:
            signal.reader.read_i = read_old;
            ast_unary = null;
            return false;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override void OnExecution(in execution.Janitor janitor)
        {
            base.OnExecution(janitor);

            //var factor = exec_stack._stack[^1];

            //Executor executor = new("unary", expected_type);
            //exec_stack._stack.Add(executor);

            //if (signal.arm_executors)
            //    executor.action_SIG_EXE = exe =>
            //    {
            //        exe.output = factor.output switch
            //        {
            //            bool b => code switch
            //            {
            //                OP_CODES.NOT => !b,
            //                _ => throw new Exception(),
            //            },
            //            int i => code switch
            //            {
            //                OP_CODES.ADD => i,
            //                OP_CODES.SUBSTRACT => -i,
            //                _ => throw new Exception(),
            //            },
            //            float f => code switch
            //            {
            //                OP_CODES.ADD => f,
            //                OP_CODES.SUBSTRACT => -f,
            //                _ => throw new Exception(),
            //            },
            //            _ => throw new NotImplementedException(),
            //        };
            //    };
        }
    }
}