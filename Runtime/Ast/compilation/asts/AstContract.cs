using System;
using System.Collections.Generic;
using System.Linq;

namespace _ZOA_.Ast.compilation
{
    internal class AstContract : AstExpression
    {
        readonly DevContract contract;

        //----------------------------------------------------------------------------------------------------------

        AstContract(in DevContract contract) : base(contract.output_type)
        {
            this.contract = contract;
        }

        //----------------------------------------------------------------------------------------------------------

        public static bool TryParseContract(in Signal signal, in TScope tscope, in Type expected_type, out AstContract ast_contract)
        {
            if (signal.reader.TryReadString_matches_out(out string cont_name, as_function_argument: false, lint: signal.reader.lint_theme.contracts, matches: ZoaContract.contracts.Keys.ToArray()))
                if (!DevContract.contracts.TryGetValue(cont_name, out var contract))
                {
                    signal.reader.Error($"no contract named '{cont_name}'.");
                    goto failure;
                }
                else if (expected_type != null && (contract.output_type == null || !contract.output_type.CanBeAssignedTo(expected_type)))
                {
                    signal.reader.Error($"expected contract of type {expected_type}, got {contract.output_type}");
                    goto failure;
                }
                else
                {
                    Dictionary<DevContract.OptionKey, AstExpression> topts = null;
                    List<AstExpression> targs = null;

                    if (contract.options != null)
                    {
                        topts = new();
                        foreach (var pair in contract.options)
                            if (pair.Value != null)
                                if (TryExpr(signal, tscope, false, pair.Value, out var ast_expr))
                                    topts.Add(pair.Key, ast_expr);
                                else
                                {
                                    signal.reader.Error($"could not parse expression for option {pair.Key}");
                                    goto failure;
                                }
                    }

                    bool expects_parenthesis = signal.reader.strict_syntax;
                    bool found_parenthesis = signal.reader.TryReadChar_match('(');

                    if (found_parenthesis)
                        signal.reader.LintOpeningBraquet();

                    if (expects_parenthesis && !found_parenthesis)
                    {
                        signal.reader.Error($"'{contract.name}' expected opening parenthesis '('");
                        goto failure;
                    }

                    if (contract.targs != null)
                    {
                        targs = new();
                        for (int i = 0; i < contract.targs.Count; i++)
                        {
                            Type arg_type = contract.targs[i];
                            if (TryExpr(signal, tscope, true, arg_type, out var ast_expr))
                                targs.Add(ast_expr);
                            else
                            {
                                signal.reader.Error($"could not parse argument[{i}] ({arg_type})");
                                goto failure;
                            }
                        }
                    }

                    if (signal.reader.sig_error != null)
                        goto failure;

                    if ((expects_parenthesis || found_parenthesis) && !signal.reader.TryReadChar_match(')', lint: signal.reader.CloseBraquetLint()))
                    {
                        signal.reader.Error($"'{contract.name}' expected closing parenthesis ')'");
                        goto failure;
                    }

                    ast_contract = new AstContract(contract);
                    return true;
                }

            failure:
            ast_contract = null;
            return false;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override void OnExecutionStack(in execution.Janitor janitor)
        {
            base.OnExecutionStack(janitor);
        }
    }
}