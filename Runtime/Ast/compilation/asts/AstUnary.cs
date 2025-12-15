using _ZOA_.Ast.execution;
using System;
using System.Collections.Generic;

namespace _ZOA_.Ast.compilation
{
    internal class AstUnary : AstExpression
    {
        internal enum Codes : byte
        {
            Positive,
            Negative,
            Not,
            Anti,
        }

        static readonly Dictionary<string, Codes> codes = new(StringComparer.OrdinalIgnoreCase)
        {
            { "+", Codes.Positive },
            { "-", Codes.Negative },
            { "!", Codes.Not },
            { "~", Codes.Anti },
        };

        readonly Codes code;
        readonly AstExpression ast_factor;

        //----------------------------------------------------------------------------------------------------------

        protected AstUnary(in Codes code, in AstExpression ast_factor) : base(ast_factor.output_type)
        {
            this.code = code;
            this.ast_factor = ast_factor;
        }

        //----------------------------------------------------------------------------------------------------------

        public static bool TryUnary(in Signal signal, in TScope tscope, in Type expected_type, out AstExpression ast_unary)
        {
            int read_old = signal.reader.read_i;

            if (signal.reader.TryReadString_matches_out(out string match, false, signal.reader.lint_theme.operators, codes.Keys))
            {
                Codes code = codes[match];
                if (TryUnary(signal, tscope, expected_type, out ast_unary))
                {
                    ast_unary = new AstUnary(code, ast_unary);
                    return true;
                }
                else
                {
                    signal.reader.Error($"expected expression after unary operator '{match}'.");
                    goto failure;
                }
            }
            // postfix
            else if (AstPrimary.TryPrimary(signal, tscope, expected_type, out ast_unary))
            {
                if (signal.reader.TryReadChar_match('['))
                {
                    signal.reader.LintOpeningBraquet();
                    if (TryExpr(signal, tscope, false, typeof(int), out var ast_index))
                        if (signal.reader.TryReadChar_match(']'))
                        {
                            signal.reader.LintClosingBraquet();
                            ast_unary = new AstIndexer(ast_unary, ast_index, ast_unary.output_type);
                        }
                        else
                        {
                            signal.reader.Error($"expected ']' after indexer");
                            goto failure;
                        }
                    else
                    {
                        signal.reader.Error($"expected expression after '['");
                        goto failure;
                    }
                }

                if (AstAccessor.TryAccessor(signal, ast_unary, out var ast_accessor))
                    ast_unary = ast_accessor;
                else if (signal.reader.sig_error != null)
                    goto failure;

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

        internal override void OnExecutionStack(in Janitor janitor)
        {
            base.OnExecutionStack(janitor);

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