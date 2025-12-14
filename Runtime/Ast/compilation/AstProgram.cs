using System.Collections.Generic;

namespace _ZOA_.Ast.compilation
{
    internal sealed class AstProgram
    {
        public readonly TScope tscope = new(parent: null);
        public readonly TStack tstack = new();

        public readonly List<AstBlock> asts = new();

        public readonly bool execute_in_background;

        //----------------------------------------------------------------------------------------------------------

        public AstProgram(in Signal signal)
        {
            tscope._vars.Add("_args_", typeof(string[]));
            tscope._vars.Add("_home_dir_", typeof(string));

#if UNITY_EDITOR
            tscope._vars.Add("_assets_dir_", typeof(string));
#endif

            while (signal.reader.HasNext() && AstBlock.TryParseBlock(signal, tscope, tstack, out AstBlock ast_block))
                asts.Add(ast_block);

            if (signal.reader.sig_error != null)
                return;

            execute_in_background = signal.reader.TryReadChar_match('&', lint: signal.reader.lint_theme.command_separators);

            if (signal.reader.TryPeekChar_out(out char peek, out _))
                signal.reader.Error($"could not parse everything ({nameof(peek)}: '{peek}').");
        }
    }
}