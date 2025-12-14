using System;
using System.Collections.Generic;

namespace _ZOA_.Ast.compilation
{
    internal class AstTerm : AstExpression
    {
        enum Codes : byte
        {
            Mult,
            Div,
            Mod,
        }

        static readonly Dictionary<string, Codes> codes = new(StringComparer.OrdinalIgnoreCase)
        {
            { "*", Codes.Mult },
            { "/", Codes.Div },
            { "%", Codes.Mod },
        };

        readonly Codes code;
        readonly AstExpression astL, astR;

        //----------------------------------------------------------------------------------------------------------

        AstTerm(in Codes code, in AstExpression astL, in AstExpression astR, in Type output_type) : base(output_type)
        {
            this.code = code;
            this.astL = astL;
            this.astR = astR;
        }

        //----------------------------------------------------------------------------------------------------------

        public static bool TryParseTerm(in Signal signal, in TScope tscope, in TStack tstack, in Type expected_type, out AstExpression term)
        {
            if (!AstUnary.TryParseUnary(signal, tscope, tstack, expected_type, out term))
                if (signal.reader.sig_error != null)
                    return false;

            if (!signal.reader.TryReadString_matches_out(out string op_name, false, signal.reader.lint_theme.operators, codes.Keys))
                return term != null;
            else
            {
                Codes code = codes[op_name];

                if (TryParseTerm(signal, tscope, tstack, expected_type, out var termR))
                {
                    term = new AstTerm(code, term, termR, Util_cobra.EnglobingType(term.output_type, termR.output_type));
                    return true;
                }
                else
                    signal.reader.Error($"expected expression after '{op_name}' operator.");
            }

            return false;
        }
    }
}