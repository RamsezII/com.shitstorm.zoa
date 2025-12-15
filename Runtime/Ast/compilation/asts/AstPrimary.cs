using _COBRA_;
using _UTIL_;
using System;

namespace _ZOA_.Ast.compilation
{
    static class AstPrimary
    {
        public static bool TryPrimary(in Signal signal, in TScope tscope, in Type expected_type, out AstExpression ast_factor)
        {
            if (signal.reader.sig_error == null)
                if (expected_type == Util_cobra.T_path)
                    if (signal.shell.TryParsePath(signal, FS_TYPES.BOTH, false, out string path))
                    {
                        ast_factor = new AstLiteral<string>(path);
                        return true;
                    }

            if (signal.reader.sig_error == null)
                if (expected_type == Util_cobra.T_fpath)
                    if (signal.shell.TryParsePath(signal, FS_TYPES.FILE, false, out string fpath))
                    {
                        ast_factor = new AstLiteral<string>(fpath);
                        return true;
                    }

            if (signal.reader.sig_error == null)
                if (expected_type == Util_cobra.T_dpath)
                    if (signal.shell.TryParsePath(signal, FS_TYPES.DIRECTORY, false, out string dpath))
                    {
                        ast_factor = new AstLiteral<string>(dpath);
                        return true;
                    }

            if (signal.reader.sig_error == null)
                if (signal.reader.TryReadChar_match('('))
                {
                    signal.reader.LintOpeningBraquet();
                    if (!AstExpression.TryExpr(signal, tscope, false, typeof(object), out var expression))
                    {
                        signal.reader.Error("expected expression inside factor parenthesis.");
                        goto failure;
                    }
                    else if (!signal.reader.TryReadChar_match(')', lint: signal.reader.CloseBraquetLint()))
                    {
                        signal.reader.Error($"expected closing parenthesis ')' after factor.");
                        --signal.reader.read_i;
                        goto failure;
                    }
                    else
                    {
                        ast_factor = expression;
                        return true;
                    }
                }

            if (signal.reader.sig_error == null)
                if (expected_type == null || expected_type.CanBeAssignedTo(typeof(string)))
                    if (AstString.TryParseString(signal, tscope, out var ast_string))
                    {
                        ast_factor = ast_string;
                        return true;
                    }
                    else if (signal.reader.sig_error != null)
                        goto failure;

            if (signal.reader.sig_error == null)
                if (AstContract.TryParseContract(signal, tscope, expected_type, out var ast_contract))
                {
                    ast_factor = ast_contract;
                    return true;
                }
                else if (signal.reader.sig_error != null)
                    goto failure;
                else if (AstVariable.TryParseVariable(signal, tscope, expected_type, out var ast_variable))
                {
                    ast_factor = ast_variable;
                    return true;
                }
                else if (signal.reader.sig_error != null)
                    goto failure;
                else if (signal.reader.TryReadArgument(out string arg, lint: signal.reader.lint_theme.fallback_default, as_function_argument: false, stoppers: CodeReader._stoppers_factors_))
                    switch (arg.ToLower())
                    {
                        case "true":
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.constants, true);
                            ast_factor = new AstLiteral<bool>(true);
                            return true;

                        case "false":
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.constants, true);
                            ast_factor = new AstLiteral<bool>(false);
                            return true;

                        default:
                            if (arg[^1] == 'f' && Util.TryParseFloat(arg[..^1], out float _float))
                                ast_factor = new AstLiteral<float>(_float);
                            else if (int.TryParse(arg, out int _int))
                                ast_factor = new AstLiteral<int>(_int);
                            else if (Util.TryParseFloat(arg, out _float))
                                ast_factor = new AstLiteral<float>(_float);
                            else
                            {
                                signal.reader.Error($"unrecognized literal : '{arg}'.");
                                goto failure;
                            }
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.literal, true);
                            return true;
                    }

                failure:
            ast_factor = null;
            return false;
        }
    }
}