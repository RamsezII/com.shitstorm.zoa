using System.Collections.Generic;
using UnityEngine;

namespace _ZOA_.Ast.compilation
{
    internal class AstString : AstExpression
    {
        readonly List<AstExpression> asts = new();

        //----------------------------------------------------------------------------------------------------------

        AstString(in List<AstExpression> asts) : base(typeof(string))
        {
            this.asts = asts;
        }

        //----------------------------------------------------------------------------------------------------------

        public static bool TryParseString(in Signal signal, in TScope tscope, out AstString ast_string)
        {
            int read_old = signal.reader.read_i;

            char sep = default;

            if (signal.reader.TryReadChar_match('\'', lint: signal.reader.lint_theme.quotes))
                sep = '\'';
            else if (signal.reader.TryReadChar_match('"', lint: signal.reader.lint_theme.quotes))
                sep = '"';

            if (sep == default)
            {
                ast_string = null;
                return false;
            }

            signal.reader.cpl_start = Mathf.Min(read_old + 1, signal.reader.read_i - 1);
            signal.reader.cpl_end = signal.reader.read_i - 1;

            List<AstExpression> asts = new();
            string current_fragment = string.Empty;
            int start_i = signal.reader.read_i;
            bool flag_escape = false;

            while (signal.reader.TryReadChar_out(out char c, skippables: null))
                switch (c)
                {
                    // escape character
                    case '\\':
                        signal.reader.LintToThisPosition(signal.reader.lint_theme.strings, false, signal.reader.read_i - 1);
                        flag_escape = true;
                        signal.reader.LintToThisPosition(signal.reader.lint_theme.quotes, false);
                        break;

                    // expression
                    case '{' when !flag_escape:
                        {
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.strings, false, signal.reader.read_i - 1);
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.quotes, false);

                            if (current_fragment.Length > 0)
                                asts.Add(new AstLiteral<string>(current_fragment));

                            if (TryExpr(signal, tscope, false, typeof(object), out AstExpression expression))
                                asts.Add(expression);
                            else
                            {
                                signal.reader.Error($"expected expression after '{{'.");
                                goto failure;
                            }

                            if (!signal.reader.TryReadChar_match('}'))
                            {
                                signal.reader.Error($"expected closing braquet '}}'.");
                                goto failure;
                            }

                            signal.reader.LintToThisPosition(signal.reader.lint_theme.strings, false, signal.reader.read_i - 1);
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.quotes, false);
                        }
                        break;

                    // end of string
                    case '\'' or '"' when !flag_escape && c == sep:
                        {
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.strings, false, signal.reader.read_i - 1);
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.quotes, false);

                            signal.reader.last_arg = current_fragment;
                            signal.reader.cpl_end = signal.reader.read_i - 1;

                            if (current_fragment.Length > 0)
                                asts.Add(new AstLiteral<string>(current_fragment));

                            ast_string = new AstString(asts);
                        }
                        return true;

                    // validate char
                    default:
                        flag_escape = false;
                        current_fragment += c;
                        break;
                }

            if (current_fragment.TryIndexOf_min(out int err_index, 0, true, ' ', '\t', '\n', '\r'))
                signal.reader.read_i = start_i + err_index;
            else
                signal.reader.read_i = read_old;

            signal.reader.LintToThisPosition(signal.reader.lint_theme.quotes, false);

        failure:
            signal.reader.Error($"string error: expected closing quote '{sep}'.");
            ast_string = null;
            return false;
        }
    }
}