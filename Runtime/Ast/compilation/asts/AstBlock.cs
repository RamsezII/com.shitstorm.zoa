using _ZOA_.Ast.execution;
using System.Collections.Generic;

namespace _ZOA_.Ast.compilation
{
    internal sealed class AstBlock : AstStatement
    {
        readonly List<AstStatement> asts;

        //----------------------------------------------------------------------------------------------------------

        AstBlock(in List<AstStatement> asts)
        {
            this.asts = asts;
        }

        //----------------------------------------------------------------------------------------------------------

        public static bool TryBlock(in Signal signal, in TScope tscope, out AstBlock ast_block)
        {
            if (signal.reader.TryReadChar_match('{'))
            {
                signal.reader.LintOpeningBraquet();

                var sub_scope = new TScope(tscope);
                var asts = new List<AstStatement>();

                while (TryStatement(signal, sub_scope, out var ast_statement))
                    asts.Add(ast_statement);

                if (signal.reader.sig_error != null)
                    goto failure;

                if (signal.reader.TryReadChar_match('}', lint: signal.reader.CloseBraquetLint()))
                {
                    ast_block = new AstBlock(asts);
                    return true;
                }
                else
                {
                    signal.reader.Error($"expected closing bracket '}}'.");
                    goto failure;
                }
            }

        failure:
            ast_block = null;
            return false;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override void OnExecutionStack(in Janitor janitor)
        {
            base.OnExecutionStack(janitor);

            for (int i = asts.Count - 1; i >= 0; i--)
                asts[i].OnExecutionStack(null);
        }
    }
}