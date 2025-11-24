using _ARK_;

namespace _ZOA_
{
    partial class ZoaShell
    {
        public bool TryParseProgram(in Signal signal, in MemScope scope, out bool background, out Executor executor)
        {
            background = false;
            executor = null;

            BlockExecutor program = new(signal, scope ?? new MemScope());
            program.mem_scope._variables.Add("_args_", new(typeof(string[]), new string[] { "arg0", "arg1", }));
            program.mem_scope._variables.Add("_home_dir_", new(typeof(string), ArkPaths.instance.Value.dpath_home));

#if UNITY_EDITOR
            program.mem_scope._variables.Add("_assets_dir_", new(typeof(string), ArkPaths.instance.Value.dpath_assets));
#endif

            while (TryParseBlock(signal, program.mem_scope, out var sub_block))
                if (sub_block != null)
                    program.stack.Add(sub_block);

            if (signal.reader.sig_error != null)
                goto failure;

            background = signal.reader.TryReadChar_match('&', lint: signal.reader.lint_theme.command_separators);

            if (signal.reader.TryPeekChar_out(out char peek, out _))
            {
                signal.reader.Stderr($"could not parse everything ({nameof(peek)}: '{peek}').");
                goto failure;
            }

            executor = program;
            return true;

        failure:
            signal.reader.LocalizeError();
            return false;
        }
    }
}