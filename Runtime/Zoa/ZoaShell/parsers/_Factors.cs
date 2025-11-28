using _COBRA_;
using _UTIL_;
using System;

namespace _ZOA_
{
    partial class ZoaShell
    {
        public bool TryParseFactor(
            in Signal signal,
            MemScope scope,
            in Type expected_type,
            in ExecutionStack exec_stack
        )
        {
            if (signal.reader.sig_error == null)
                if (expected_type == T_path)
                    if (TryParsePath(signal, FS_TYPES.BOTH, false, out string fpath))
                    {
                        if (signal.arm_executors)
                            exec_stack.Push(Executor.Literal((ZoaFPath)fpath));
                        else
                            exec_stack.Push(new Executor("file path", T_fpath));
                        return true;
                    }

            if (signal.reader.sig_error == null)
                if (expected_type == T_fpath)
                    if (TryParsePath(signal, FS_TYPES.FILE, false, out string fpath))
                    {
                        if (signal.arm_executors)
                            exec_stack.Push(Executor.Literal((ZoaFPath)fpath));
                        else
                            exec_stack.Push(new Executor("file path", T_fpath));
                        return true;
                    }

            if (signal.reader.sig_error == null)
                if (expected_type == T_dpath)
                    if (TryParsePath(signal, FS_TYPES.DIRECTORY, false, out string dpath))
                    {
                        if (signal.arm_executors)
                            exec_stack.Push(Executor.Literal((ZoaDPath)dpath));
                        else
                            exec_stack.Push(new Executor("directory path", T_dpath));
                        return true;
                    }

            if (signal.reader.sig_error == null)
                if (signal.reader.TryReadChar_match('('))
                {
                    signal.reader.LintOpeningBraquet();
                    if (!TryParseExpression(signal, scope, false, T_object, exec_stack))
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
                if (expected_type == null || expected_type.CanBeAssignedTo(typeof(string)))
                    if (TryParseString(signal, scope, exec_stack))
                        return true;
                    else if (signal.reader.sig_error != null)
                        goto failure;

            if (signal.reader.sig_error == null)
                if (TryParseContract(signal, scope, expected_type, exec_stack))
                    return true;
                else if (signal.reader.sig_error != null)
                    goto failure;
                else if (TryParseVariable(signal, scope, expected_type, out _, exec_stack))
                    return true;
                else if (signal.reader.sig_error != null)
                    goto failure;
                else if (signal.reader.TryReadArgument(out string arg, lint: signal.reader.lint_theme.fallback_default, as_function_argument: false, stoppers: CodeReader._stoppers_factors_))
                    switch (arg.ToLower())
                    {
                        case "true":
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.constants, true);
                            if (signal.arm_executors)
                                exec_stack.Push(Executor.Literal(true));
                            else
                                exec_stack.Push(new Executor("bool", T_bool));
                            return true;

                        case "false":
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.constants, true);
                            if (signal.arm_executors)
                                exec_stack.Push(Executor.Literal(false));
                            else
                                exec_stack.Push(new Executor("condition", T_bool));
                            return true;

                        default:
                            if (arg[^1] == 'f' && Util.TryParseFloat(arg[..^1], out float _float))
                            {
                                if (signal.arm_executors)
                                    exec_stack.Push(Executor.Literal(_float));
                                else
                                    exec_stack.Push(new Executor("number", T_float));
                            }
                            else if (int.TryParse(arg, out int _int))
                            {
                                if (signal.arm_executors)
                                    exec_stack.Push(Executor.Literal(_int));
                                else
                                    exec_stack.Push(new Executor("int", T_int));
                            }
                            else if (Util.TryParseFloat(arg, out _float))
                            {
                                if (signal.arm_executors)
                                    exec_stack.Push(Executor.Literal(_float));
                                else
                                    exec_stack.Push(new Executor("float", T_float));
                            }
                            else
                            {
                                signal.reader.Stderr($"unrecognized literal : '{arg}'.");
                                goto failure;
                            }
                            signal.reader.LintToThisPosition(signal.reader.lint_theme.literal, true);
                            return true;
                    }

                failure:
            return false;
        }
    }
}