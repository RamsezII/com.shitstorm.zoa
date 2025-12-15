using _ZOA_.Ast.execution;
using System.Collections.Generic;

namespace _ZOA_.Ast.compilation
{
    internal sealed class AstBlock : AstAbstract
    {
        readonly List<AstAbstract> asts;

        //----------------------------------------------------------------------------------------------------------

        AstBlock(in List<AstAbstract> asts) : base(null)
        {
            this.asts = asts;
        }

        //----------------------------------------------------------------------------------------------------------

        public static bool TryParseBlock(in Signal signal, in TScope tscope, out AstAbstract ast)
        {
        skipped_comments:
            if (signal.reader.TryReadChar_match('#', lint: signal.reader.lint_theme.comments))
            {
                signal.reader.SkipUntil('\n');
                goto skipped_comments;
            }

            if (signal.reader.TryReadChar_match(';', lint: signal.reader.lint_theme.command_separators))
            {
                ast = null;
                return true;
            }

            if (signal.reader.TryReadChar_match('{'))
            {
                signal.reader.LintOpeningBraquet();

                var sub_scope = new TScope(tscope);
                var asts = new List<AstAbstract>();

                while (TryParseBlock(signal, sub_scope, out var sub_block))
                    asts.Add(sub_block);

                if (signal.reader.sig_error != null)
                    goto failure;

                if (signal.reader.TryReadChar_match('}', lint: signal.reader.CloseBraquetLint()))
                {
                    ast = new AstBlock(asts);
                    return true;
                }
                else
                {
                    signal.reader.Error($"expected closing bracket '}}'.");
                    goto failure;
                }
            }
            else if (AstExpression.TryParseExpression(signal, tscope, false, null, out var expression))
            {
                ast = expression;
                return true;
            }

        failure:
            ast = null;
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