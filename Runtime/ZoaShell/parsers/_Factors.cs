using _COBRA_;

namespace _ZOA_
{
    partial class ZoaShell
    {
        public bool TryParseFactor(in Signal signal, in MemScope scope, out ExpressionExecutor executor, in bool no_type_check = false)
        {
            if (signal.reader.sig_error == null)
                if (signal.reader.TryReadChar_match('('))
                {
                    signal.reader.LintOpeningBraquet();
                    if (!TryParseExpression(signal, scope, false, out executor, type_check: false))
                    {
                        signal.reader.Stderr("expected expression inside factor parenthesis.");
                        goto failure;
                    }
                    else if (!signal.reader.TryReadChar_match(')', lint: signal.reader.CloseBraquetLint()))
                    {
                        signal.reader.Stderr($"expected closing parenthesis ')' after factor {executor.ToLog()}.");
                        --signal.reader.read_i;
                        goto failure;
                    }
                    else
                        return true;
                }

            if (signal.reader.sig_error == null)
                if (TryParseString(signal, scope, out StringExecutor exe_str))
                {
                    executor = exe_str;
                    return true;
                }
                else if (signal.reader.sig_error != null)
                    goto failure;

            if (signal.reader.sig_error == null)
                if (TryParseMethod(signal, scope, out ContractExecutor func_exe))
                {
                    executor = func_exe;
                    return true;
                }
                else if (signal.reader.sig_error != null)
                    goto failure;
                else if (TryParseVariable(signal, scope, out _, out var var_exe))
                {
                    executor = var_exe;
                    return true;
                }
                else if (signal.reader.sig_error != null)
                    goto failure;
                else if (signal.reader.TryReadArgument(out string arg, lint: signal.reader.lint_theme.fallback_default, as_function_argument: false, stoppers: CodeReader._stoppers_factors_))
                    switch (arg.ToLower())
                    {
                        case "true":
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.constants, true);
                            executor = new LiteralExecutor(signal, scope, true);
                            return true;

                        case "false":
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.constants, true);
                            executor = new LiteralExecutor(signal, scope, false);
                            return true;

                        default:
                            if (arg[^1] == 'f' && Util.TryParseFloat(arg[..^1], out float _float))
                                executor = new LiteralExecutor(signal, scope, _float);
                            else if (int.TryParse(arg, out int _int))
                                executor = new LiteralExecutor(signal, scope, _int);
                            else if (Util.TryParseFloat(arg, out _float))
                                executor = new LiteralExecutor(signal, scope, _float);
                            else
                            {
                                signal.reader.Stderr($"unrecognized literal : '{arg}'.");
                                goto failure;
                            }
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.literal, true);
                            return true;
                    }

                failure:
            executor = null;
            return false;
        }
    }
}