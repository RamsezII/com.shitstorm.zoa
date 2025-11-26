using _ARK_;
using System.Collections.Generic;

namespace _ZOA_
{
    partial class ZoaShell
    {
        public bool TryParseProgram(in Signal signal, in MemScope scope, out bool background, out ZoaExecutor executor)
        {
            executor = null;
            background = false;

            MemScope sub_scope = new(scope);
            sub_scope._vars.Add("_args_", new(typeof(string[]), new string[] { "arg0", "arg1", }));
            sub_scope._vars.Add("_home_dir_", new(typeof(string), ArkPaths.instance.Value.dpath_home));

#if UNITY_EDITOR
            sub_scope._vars.Add("_assets_dir_", new(typeof(string), ArkPaths.instance.Value.dpath_assets));
#endif

            List<ZoaExecutor> stack_blocks = new();

            while (TryParseBlock(signal, sub_scope, new TypeStack(), new ValueStack(), out ZoaExecutor block_exe))
                if (block_exe != null)
                    stack_blocks.Add(block_exe);

            if (signal.reader.sig_error != null)
                goto failure;

            background = signal.reader.TryReadChar_match('&', lint: signal.reader.lint_theme.command_separators);

            if (signal.reader.TryPeekChar_out(out char peek, out _))
            {
                signal.reader.Stderr($"could not parse everything ({nameof(peek)}: '{peek}').");
                goto failure;
            }

            if (stack_blocks.Count > 0)
            {
                executor = new()
                {
                    routine_SIG_ALL = ZoaExecutor.EExecute_SIG_ALL(executor, stack_blocks),
                };
                return true;
            }

            return false;

        failure:
            signal.reader.LocalizeError();
            return false;
        }
    }
}