using System.Collections.Generic;

namespace _ZOA_.Ast.compilation
{
    internal sealed class AstBlock : AstAbstract
    {
        public readonly List<AstAbstract> asts;

        //----------------------------------------------------------------------------------------------------------

        AstBlock(in List<AstAbstract> asts) : base(null)
        {
            this.asts = asts;
        }

        //----------------------------------------------------------------------------------------------------------

        public static bool TryParseBlock(in Signal signal, in TScope tscope, in TStack tstack, out AstBlock ast_block)
        {
        skipped_comments:
            if (signal.reader.TryReadChar_match('#', lint: signal.reader.lint_theme.comments))
            {
                signal.reader.SkipUntil('\n');
                goto skipped_comments;
            }

            if (signal.reader.TryReadChar_match(';', lint: signal.reader.lint_theme.command_separators))
            {
                ast_block = null;
                return true;
            }

            if (signal.reader.TryReadChar_match('{'))
            {
                signal.reader.LintOpeningBraquet();

                var sub_scope = new TScope(tscope);
                var asts = new List<AstAbstract>();

                while (TryParseBlock(signal, sub_scope, tstack, out var sub_block))
                    asts.Add(sub_block);

                if (signal.reader.sig_error != null)
                    goto failure;

                if (signal.reader.TryReadChar_match('}', lint: signal.reader.CloseBraquetLint()))
                {
                    ast_block = new(asts);
                    return true;
                }
                else
                {
                    signal.reader.Error($"expected closing bracket '}}'.");
                    goto failure;
                }
            }
            else
            {
                if (AstExpression.TryParseExpression(signal, tscope, tstack, false, null, out var expression))
                {
                    ast_block = new(new() { expression, });
                    return true;
                }
            }

        failure:
            ast_block = null;
            return false;
        }
    }
}