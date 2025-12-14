using System;
using System.Collections.Generic;

namespace _ZOA_.Ast.compilation
{
    internal class AstComparison : AstBinaryOperation
    {
        public enum Codes : byte
        {
            Greater,
            GreaterOrEqual,
            Lesser,
            LesserOrEqual,
            Equals,
            NotEquals,
        }

        static readonly Dictionary<string, Codes> codes = new(StringComparer.OrdinalIgnoreCase)
        {
            { ">", Codes.Greater },
            { "<", Codes.Lesser },
            { "==", Codes.Equals },
            { "!=", Codes.NotEquals },
            { ">=", Codes.GreaterOrEqual },
            { "<=", Codes.LesserOrEqual },
        };

        readonly Codes code;

        //----------------------------------------------------------------------------------------------------------

        AstComparison(in Codes code, in AstExpression astL, in AstExpression astR) : base(OpDepth.Comp, astL, astR)
        {
            this.code = code;
        }

        //----------------------------------------------------------------------------------------------------------

        public static bool TryParseComparison(in Signal signal, in TScope tscope, in TStack tstack, out AstExpression ast_comparison)
        {
            if (!TryParseExpression(signal, tscope, tstack, false, typeof(object), out ast_comparison))
                return false;
            else if (!signal.reader.TryReadString_matches_out(out string op_name, false, signal.reader.lint_theme.operators, codes.Keys))
                return true;
            else
            {
                Codes code = codes[op_name];

                if (AstAddition.TryParseAddition(signal, tscope, tstack, out var astR))
                {
                    ast_comparison = new AstComparison(code, ast_comparison, astR);
                    return true;
                }
                else
                    signal.reader.Error($"expected expression after '{op_name}' operator.");
            }

            signal.reader.Error($"could not parse comparison");
            ast_comparison = null;
            return false;
        }
    }
}