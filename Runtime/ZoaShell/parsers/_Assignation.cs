using _COBRA_;
using System.Collections.Generic;

namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseAssignation(in Signal signal, in MemScope scope, out AssignationExecutor exe_assign)
        {
            int read_old = signal.reader.read_i;

            if (!TryParseVariable(signal, scope, out string var_name, out var var_exe))
                goto failure;

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
                OP_FLAGS code = op_name switch
                {
                    "+=" => OP_FLAGS.add,
                    "-=" => OP_FLAGS.sub,
                    "*=" => OP_FLAGS.mul,
                    "/=" => OP_FLAGS.div,
                    "&=" => OP_FLAGS.and,
                    "|=" => OP_FLAGS.or,
                    "^=" => OP_FLAGS.xor,
                    _ => OP_FLAGS.unknown,
                };

                code |= OP_FLAGS.assign;

                if (TryParseExpression(signal, scope, false, out var exe_expr))
                {
                    exe_assign = new(signal, scope, var_name, exe_expr);
                    return true;
                }
                else
                {
                    signal.reader.Stderr($"expected expression after '{op_name}' operator.");
                    goto failure;
                }
            }

        failure:
            signal.reader.read_i = read_old;
            exe_assign = null;
            return false;
        }
    }
}