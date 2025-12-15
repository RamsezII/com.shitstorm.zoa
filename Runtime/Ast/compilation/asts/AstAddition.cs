using _COBRA_;
using _ZOA_.Ast.execution;
using System.Collections.Generic;

namespace _ZOA_.Ast.compilation
{
    internal class AstAddition : AstBinaryOperation
    {
        enum Codes : byte
        {
            _none_,
            Add,
            Sub,
        }

        readonly Codes code;

        //----------------------------------------------------------------------------------------------------------

        AstAddition(in Codes code, in AstExpression astL, in AstExpression astR) : base(OpDepth.Add, astL, astR)
        {
            this.code = code;
        }

        //----------------------------------------------------------------------------------------------------------

        public static bool TryParseAddition(in Signal signal, in TScope tscope, out AstExpression ast_addition)
        {
            if (AstTerm.TryParseTerm(signal, tscope, typeof(CobraNumber), out var term))
            {
                int read_old = signal.reader.read_i;
                if (!signal.reader.TryReadChar_matches_out(out char op_symbol, true, "+-"))
                {
                    signal.reader.read_i = read_old;
                    ast_addition = term;
                    return true;
                }
                else
                {
                    signal.reader.LintToThisPosition(signal.reader.lint_theme.operators, true);

                    Codes code = op_symbol switch
                    {
                        '+' => Codes.Add,
                        '-' => Codes.Sub,
                        _ => throw new System.NotImplementedException(),
                    };

                    if (TryParseAddition(signal, tscope, out var addR))
                    {
                        ast_addition = new AstAddition(code, term, addR);
                        return true;
                    }
                    else
                        signal.reader.Error($"expected expression after '{op_symbol}' operator.");
                }
            }

            signal.reader.Error($"could not parse comparison");
            ast_addition = null;
            return false;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override void OnExecutionStack(in Janitor janitor)
        {
            base.OnExecutionStack(janitor);
        }
    }
}