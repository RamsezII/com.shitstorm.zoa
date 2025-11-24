namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseUnary(in Signal signal, in MemScope scope, out ExpressionExecutor executor)
        {
            if (signal.reader.TryReadChar_matches_out(out char unary_operator, true, "+-!"))
            {
                UnaryExecutor.Operators code = unary_operator switch
                {
                    '+' => UnaryExecutor.Operators.Add,
                    '-' => UnaryExecutor.Operators.Sub,
                    '!' => UnaryExecutor.Operators.Not,
                    _ => 0,
                };

                switch (code)
                {
                    case UnaryExecutor.Operators.Add:
                    case UnaryExecutor.Operators.Sub:
                        {
                            int read_old = signal.reader.read_i;
                            if (signal.reader.TryReadChar_match(unary_operator, signal.reader.lint_theme.operators, skippables: null))
                            {
                                if (!signal.reader.TryReadArgument(out string var_name, false, signal.reader.lint_theme.variables, skippables: null))
                                    signal.reader.Stderr($"expected variable after increment operator '{unary_operator}{unary_operator}'.");
                                else if (!scope.TryGetCell(var_name, out _))
                                    signal.reader.Stderr($"no variable named '{var_name}'.");
                                else
                                {
                                    executor = new IncrementExecutor(signal, scope, var_name, code switch
                                    {
                                        UnaryExecutor.Operators.Add => IncrementExecutor.Operators.AddBefore,
                                        UnaryExecutor.Operators.Sub => IncrementExecutor.Operators.SubBefore,
                                        _ => 0,
                                    });
                                    return signal.reader.sig_error == null;
                                }
                                signal.reader.read_i = read_old;
                                executor = null;
                                return false;
                            }
                        }
                        break;
                }

                if (TryParseFactor(signal, scope, out executor))
                {
                    executor = new UnaryExecutor(signal, scope, executor, code);
                    return true;
                }
                else
                {
                    signal.reader.Stderr($"expected factor after '{unary_operator}'.");
                    return false;
                }
            }

            executor = null;
            return false;
        }
    }
}