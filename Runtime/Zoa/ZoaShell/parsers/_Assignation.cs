using _COBRA_;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseAssignation(
            in Signal signal,
            MemScope scope,
            in Type expected_type,
            in ExecutionStack exec_stack
        )
        {
            int read_old = signal.reader.read_i;

            if (signal.reader.TryReadString_matches_out(
                out string var_name,
                as_function_argument: false,
                lint: signal.reader.lint_theme.variables,
                matches: scope.EVarNames().ToArray())
            )
                if (!scope.TryGetCell(var_name, out MemCell var_cell))
                {
                    signal.reader.sig_error ??= $"no variable named '{var_name}'.";
                    goto failure;
                }
                else if (expected_type != null && !var_cell.type.CanBeAssignedTo(expected_type))
                {
                    signal.reader.sig_error ??= $"expted variable of type {expected_type}, got {var_cell.type}";
                    goto failure;
                }
                else
                {
                    signal.reader.LintToThisPosition(signal.reader.lint_theme.variables, true);

                    List<string> matches = new() { "=", "+=", "-=", "*=", "/=", "&=", "|=", "^=", };

                    if (!signal.reader.TryReadString_matches_out(
                        out string op_name,
                        as_function_argument: false,
                        lint: signal.reader.lint_theme.operators,
                        ignore_case: true,
                        add_to_completions: true,
                        skippables: CodeReader._empties_,
                        stoppers: " \n\r{}(),;'\"",
                        matches: matches)
                        )
                        goto failure;
                    else
                    {
                        OP_CODES code = op_name switch
                        {
                            "+=" => OP_CODES.ADD,
                            "-=" => OP_CODES.SUBSTRACT,
                            "*=" => OP_CODES.MULTIPLY,
                            "/=" => OP_CODES.DIVIDE,
                            "&=" => OP_CODES.AND,
                            "|=" => OP_CODES.OR,
                            "^=" => OP_CODES.XOR,
                            "=" => OP_CODES.ASSIGN,
                            _ => OP_CODES._none_,
                        };

                        if (TryParseExpression(signal, scope, false, var_cell.type, exec_stack))
                        {
                            Executor expr_exe = exec_stack.Peek();
                            Type expr_type = expr_exe.type;

                            if (signal.arm_executors)
                                exec_stack.Push(new("assignation", var_cell.type)
                                {
                                    action_SIG_EXE = exe =>
                                    {
                                        object value = expr_exe.output;

                                        if (code != OP_CODES.ASSIGN)
                                        {
                                            if (var_cell.value is bool var_b && value is bool expr_b)
                                                value = code switch
                                                {
                                                    OP_CODES.AND => var_b && expr_b,
                                                    OP_CODES.OR => var_b || expr_b,
                                                    OP_CODES.XOR => var_b != expr_b,
                                                    _ => throw new NotSupportedException(),
                                                };
                                            else if (var_cell.value is int var_i && value is int expr_i)
                                                value = code switch
                                                {
                                                    OP_CODES.ADD => var_i + expr_i,
                                                    OP_CODES.SUBSTRACT => var_i - expr_i,
                                                    OP_CODES.MULTIPLY => var_i * expr_i,
                                                    OP_CODES.DIVIDE => var_i / expr_i,
                                                    OP_CODES.MODULO => var_i % expr_i,
                                                    OP_CODES.AND => var_i & expr_i,
                                                    OP_CODES.OR => var_i | expr_i,
                                                    _ => throw new NotSupportedException(),
                                                };
                                            else if (var_cell.value is float var_f && value is float expr_f)
                                                value = code switch
                                                {
                                                    OP_CODES.ADD => var_f + expr_f,
                                                    OP_CODES.SUBSTRACT => var_f - expr_f,
                                                    OP_CODES.MULTIPLY => var_f * expr_f,
                                                    OP_CODES.DIVIDE => var_f / expr_f,
                                                    OP_CODES.MODULO => var_f % expr_f,
                                                    _ => throw new NotSupportedException(),
                                                };
                                            else if (var_cell.value is string var_s)
                                                value = code switch
                                                {
                                                    OP_CODES.ADD => var_s + value,
                                                    _ => throw new NotSupportedException(),
                                                };
                                        }

                                        var_cell.value = value;
                                    }
                                });

                            return true;
                        }
                        else
                        {
                            signal.reader.Stderr($"expected expression after '{op_name}' operator.");
                            goto failure;
                        }
                    }
                }

            failure:
            signal.reader.read_i = read_old;
            return false;
        }
    }
}