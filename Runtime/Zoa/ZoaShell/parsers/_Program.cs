using _ARK_;
using System.Collections.Generic;

namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseProgram(in Signal signal, in MemScope scope, in ExecutionStack exec_stack, out bool background)
        {
            background = false;

            MemScope sub_scope = new(scope);
            sub_scope._vars.Add("_args_", new(typeof(string[]), new string[] { "arg0", "arg1", }));
            sub_scope._vars.Add("_home_dir_", new(typeof(string), ArkPaths.instance.Value.dpath_home));

#if UNITY_EDITOR
            sub_scope._vars.Add("_assets_dir_", new(typeof(string), ArkPaths.instance.Value.dpath_assets));
#endif

            List<Executor> stack_blocks = new();

            while (signal.reader.HasNext() && TryParseBlock(signal, sub_scope, exec_stack)) ;

            if (signal.reader.sig_error != null)
                goto failure;

            background = signal.reader.TryReadChar_match('&', lint: signal.reader.lint_theme.command_separators);

            if (signal.reader.TryPeekChar_out(out char peek, out _))
            {
                signal.reader.Stderr($"could not parse everything ({nameof(peek)}: '{peek}').");
                goto failure;
            }

            if (exec_stack._stack.Count > 0)
                return true;

            failure:
            return false;
        }
    }
}