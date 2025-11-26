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
            in TypeStack type_stack,
            ValueStack value_stack,
            out ZoaExecutor executor
        )
        {
            int read_old = signal.reader.read_i;

            if (signal.reader.TryReadString_matches_out(
                out string var_name,
                as_function_argument: false,
                lint: signal.reader.lint_theme.variables,
                matches: scope.EVarNames().ToArray())
            )
                if (!scope.TryGetCell(var_name, out MemCell cell))
                {
                    signal.reader.Stderr($"no variable named '{var_name}'.");
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

                        if (TryParseExpression(signal, scope, type_stack, value_stack, false, T_object, out ZoaExecutor exe_expr))
                        {
                            Type expr_type = type_stack.Pop();
                            executor = new();

                            if (signal.flags.HasFlag(SIG_FLAGS.EXEC))
                                executor.action_SIG_EXE = exe =>
                                {
                                    object expr_val = value_stack.Pop();

                                    if (code != OP_CODES.ASSIGN)
                                    {
                                        if (cell.value is bool var_b && expr_val is bool expr_b)
                                            expr_val = code switch
                                            {
                                                OP_CODES.AND => var_b && expr_b,
                                                OP_CODES.OR => var_b || expr_b,
                                                OP_CODES.XOR => var_b != expr_b,
                                                _ => throw new NotSupportedException(),
                                            };
                                        else if (cell.value is int var_i && expr_val is int expr_i)
                                            expr_val = code switch
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
                                        else if (cell.value is float var_f && expr_val is float expr_f)
                                            expr_val = code switch
                                            {
                                                OP_CODES.ADD => var_f + expr_f,
                                                OP_CODES.SUBSTRACT => var_f - expr_f,
                                                OP_CODES.MULTIPLY => var_f * expr_f,
                                                OP_CODES.DIVIDE => var_f / expr_f,
                                                OP_CODES.MODULO => var_f % expr_f,
                                                _ => throw new NotSupportedException(),
                                            };
                                        else if (cell.value is string var_s)
                                            expr_val = code switch
                                            {
                                                OP_CODES.ADD => var_s + expr_val,
                                                _ => throw new NotSupportedException(),
                                            };
                                    }

                                    cell.value = expr_val;
                                    value_stack.Push(expr_val);
                                };

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
            executor = null;
            return false;
        }
    }
}