using _COBRA_;
using System;
using System.Collections.Generic;

namespace _ZOA_.Ast.compilation
{
    internal class AstBinaryOperation : AstExpression
    {
        enum Depths : byte
        {
            Or,
            And,
            Equality,
            Comparison,
            Addition,
            Term,
            _last_,
        }

        enum Codes : byte
        {
            Or,
            And,
            Equal,
            NotEqual,
            Lesser,
            LesserOrEqual,
            Greater,
            GreaterOrEqual,
            Add,
            Sub,
            Multiply,
            Divide,
            Modulus,
        }

        static readonly Dictionary<string, Codes>[] codes = new Dictionary<string, Codes>[(int)Depths._last_]
        {
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "or", Codes.Or },
            },
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "and", Codes.And },
            },
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "==", Codes.Equal },
                { "!=", Codes.NotEqual },
            },
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "<", Codes.Lesser },
                { "<=", Codes.LesserOrEqual },
                { ">", Codes.Greater },
                { ">=", Codes.GreaterOrEqual },
            },
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "+", Codes.Add },
                { "-", Codes.Sub },
            },
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "*", Codes.Multiply },
                { "/", Codes.Divide },
                { "%", Codes.Modulus },
            },
        };

        readonly Depths depth;
        readonly Codes code;
        readonly AstExpression astL, astR;

        //----------------------------------------------------------------------------------------------------------

        AstBinaryOperation(in Depths depth, in Codes code, in AstExpression astL, in AstExpression astR) : base(typeof(bool))
        {
            this.depth = depth;
            this.code = code;
            this.astL = astL;
            this.astR = astR;
        }

        //----------------------------------------------------------------------------------------------------------

        public static bool TryOr(in Signal signal, in TScope tscope, out AstExpression ast_expr) => TryBinOp(signal, tscope, Depths.Or, out ast_expr);
        static bool TryBinOp(in Signal signal, in TScope tscope, in Depths depth, out AstExpression ast_expr)
        {
            if (depth == Depths.Term)
            {
                if (AstUnary.TryUnary(signal, tscope, typeof(CobraNumber), out ast_expr))
                    while (signal.reader.TryReadString_matches_out(out string match, false, signal.reader.lint_theme.operators, codes[(int)depth].Keys))
                        if (AstUnary.TryUnary(signal, tscope, typeof(CobraNumber), out var astR))
                        {
                            Codes code = codes[(int)depth][match];
                            ast_expr = new AstBinaryOperation(depth, code, ast_expr, astR);
                            return true;
                        }
                        else
                        {
                            signal.reader.Error($"expected expression after '{match}' operator");
                            goto failure;
                        }
                else
                    return true;
            }
            else
            {
                if (TryBinOp(signal, tscope, depth + 1, out ast_expr))
                    while (signal.reader.TryReadString_matches_out(out string match, false, signal.reader.lint_theme.operators, codes[(int)depth].Keys))
                        if (TryBinOp(signal, tscope, depth + 1, out var astR))
                        {
                            Codes code = codes[(int)depth][match];
                            ast_expr = new AstBinaryOperation(depth, code, ast_expr, astR);
                            return true;
                        }
                        else
                        {
                            signal.reader.Error($"expected expression after '{match}' operator");
                            goto failure;
                        }
                else
                    return true;
            }

        failure:
            ast_expr = null;
            return false;
        }
    }
}