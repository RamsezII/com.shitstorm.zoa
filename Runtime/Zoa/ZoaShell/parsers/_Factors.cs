using _COBRA_;

namespace _ZOA_
{
    partial class ZoaShell
    {
        public bool TryParseFactor(
            in Signal signal,
            MemScope scope,
            in TypeStack type_stack,
            ValueStack value_stack,
            out ZoaExecutor executor
        )
        {
            if (signal.reader.sig_error == null)
                if (signal.reader.TryReadChar_match('('))
                {
                    signal.reader.LintOpeningBraquet();
                    if (!TryParseExpression(signal, scope, type_stack, value_stack, false, T_object, out executor))
                    {
                        signal.reader.Stderr("expected expression inside factor parenthesis.");
                        goto failure;
                    }
                    else if (!signal.reader.TryReadChar_match(')', lint: signal.reader.CloseBraquetLint()))
                    {
                        signal.reader.Stderr($"expected closing parenthesis ')' after factor.");
                        --signal.reader.read_i;
                        goto failure;
                    }
                    else
                        return true;
                }

            if (signal.reader.sig_error == null)
                if (TryParseString(signal, scope, type_stack, value_stack, out executor))
                    return true;
                else if (signal.reader.sig_error != null)
                    goto failure;

            if (signal.reader.sig_error == null)
                if (TryParseContract(signal, scope, type_stack, value_stack, out executor))
                    return true;
                else if (signal.reader.sig_error != null)
                    goto failure;
                else if (TryParseVariable(signal, scope, type_stack, value_stack, out _, out var var_exe))
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
                            executor = new ZoaExecutor(type_stack, value_stack, true);
                            return true;

                        case "false":
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.constants, true);
                            executor = new ZoaExecutor(type_stack, value_stack, false);
                            return true;

                        default:
                            if (arg[^1] == 'f' && Util.TryParseFloat(arg[..^1], out float _float))
                                executor = new ZoaExecutor(type_stack, value_stack, _float);
                            else if (int.TryParse(arg, out int _int))
                                executor = new ZoaExecutor(type_stack, value_stack, _int);
                            else if (Util.TryParseFloat(arg, out _float))
                                executor = new ZoaExecutor(type_stack, value_stack, _float);
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