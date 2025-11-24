using System.Collections.Generic;
using UnityEngine;

namespace _ZOA_
{
    partial class ZoaShell
    {
        public bool TryParseString(in Signal signal, in MemScope scope, out StringExecutor executor)
        {
            executor = null;
            int read_old = signal.reader.read_i;

            char sep = default;

            if (signal.reader.TryReadChar_match('\'', lint: signal.reader.lint_theme.quotes))
                sep = '\'';
            else if (signal.reader.TryReadChar_match('"', lint: signal.reader.lint_theme.quotes))
                sep = '"';

            if (sep == default)
                return false;

            signal.reader.cpl_start = Mathf.Min(read_old + 1, signal.reader.read_i - 1);
            signal.reader.cpl_end = signal.reader.read_i - 1;

            List<Executor> stack = new();
            string value = string.Empty;
            int start_i = signal.reader.read_i;
            bool flag_escape = false;

            while (signal.reader.TryReadChar_out(out char c, skippables: null))
                switch (c)
                {
                    case '\\':
                        signal.reader.LintToThisPosition(signal.reader.lint_theme.strings, false, signal.reader.read_i - 1);
                        flag_escape = true;
                        signal.reader.LintToThisPosition(signal.reader.lint_theme.quotes, false);
                        break;

                    case '\'' or '"' when !flag_escape && c == sep:
                        {
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.strings, false, signal.reader.read_i - 1);
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.quotes, false);

                            signal.reader.last_arg = value;
                            signal.reader.cpl_end = signal.reader.read_i - 1;

                            if (value.Length > 0 || stack.Count == 0)
                                stack.Add(new LiteralExecutor(signal, scope, value));

                            executor = new StringExecutor(signal, scope, stack);
                        }
                        return true;

                    case '{' when !flag_escape:
                        {
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.strings, false, signal.reader.read_i - 1);
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.quotes, false);

                            if (value.Length > 0)
                            {
                                stack.Add(new LiteralExecutor(signal, scope, value));
                                value = string.Empty;
                            }

                            if (!TryParseExpression(signal, scope, false, out ExpressionExecutor expr, type_check: false))
                            {
                                signal.reader.Stderr($"expected expression after '{{'.");
                                return false;
                            }

                            if (!signal.reader.TryReadChar_match('}'))
                            {
                                signal.reader.Stderr($"expected closing braquet '}}'.");
                                return false;
                            }

                            signal.reader.LintToThisPosition(signal.reader.lint_theme.strings, false, signal.reader.read_i - 1);
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.quotes, false);

                            stack.Add(expr);
                        }
                        break;

                    default:
                        flag_escape = false;
                        value += c;
                        break;
                }

            if (value.TryIndexOf_min(out int err_index, 0, true, ' ', '\t', '\n', '\r'))
                signal.reader.read_i = start_i + err_index;
            else
                signal.reader.read_i = read_old;

            signal.reader.LintToThisPosition(signal.reader.lint_theme.quotes, false);

            signal.reader.Stderr($"string error: expected closing quote '{sep}'.");
            return false;
        }
    }
}