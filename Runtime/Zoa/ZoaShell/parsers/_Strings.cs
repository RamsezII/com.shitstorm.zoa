using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace _ZOA_
{
    partial class ZoaShell
    {
        public bool TryParseString(
            in Signal signal,
            MemScope scope,
            in TypeStack type_stack,
            ValueStack value_stack,
            out ZoaExecutor executor
        )
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

            List<ZoaExecutor> exe_stack = new();
            string current_fragment = string.Empty;
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

                    // the end
                    case '\'' or '"' when !flag_escape && c == sep:
                        {
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.strings, false, signal.reader.read_i - 1);
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.quotes, false);

                            signal.reader.last_arg = current_fragment;
                            signal.reader.cpl_end = signal.reader.read_i - 1;

                            // grab buffer
                            if (current_fragment.Length > 0 || exe_stack.Count == 0)
                                exe_stack.Add(new(type_stack, value_stack, current_fragment));

                            var exe = executor;
                            exe = executor = new()
                            {
                                routine_SIG_ALL = EExecute_SIG_ALL(),
                            };

                            // execute stack of fragments and expressions
                            IEnumerator<ExecutionOutput> EExecute_SIG_ALL()
                            {
                                StringBuilder sb = new();
                                for (int i = 0; i < exe_stack.Count; ++i)
                                {
                                    ZoaExecutor ex = exe_stack[i];
                                    while (!ex.isDone)
                                    {
                                        ExecutionOutput output = ex.OnSignal(exe.signal);
                                        yield return output;

                                        if (ex.isDone)
                                        {
                                            object fragment = value_stack.Pop();
                                            sb.Append(fragment);
                                        }
                                    }
                                }
                                value_stack.Push(sb.ToString());
                            }
                        }
                        return true;

                    case '{' when !flag_escape:
                        {
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.strings, false, signal.reader.read_i - 1);
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.quotes, false);

                            // pull current fragment
                            if (current_fragment.Length > 0)
                            {
                                exe_stack.Add(new(type_stack, value_stack, current_fragment));
                                current_fragment = string.Empty;
                            }

                            if (!TryParseExpression(signal, scope, type_stack, value_stack, false, T_object, out ZoaExecutor inside_expr))
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

                            exe_stack.Add(inside_expr);
                        }
                        break;

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

            signal.reader.Stderr($"string error: expected closing quote '{sep}'.");
            return false;
        }
    }
}