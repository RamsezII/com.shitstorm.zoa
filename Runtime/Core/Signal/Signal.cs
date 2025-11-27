using _COBRA_;
using System;
using UnityEngine;

namespace _ZOA_
{
    enum SIG_ENUM : byte
    {
        _NONE_,
        Cmd,
        Sript,
        Lint,
        Check,
        Exec,
    }

    [Flags]
    public enum SIG_FLAGS : byte
    {
        CMD = 1 << SIG_ENUM.Cmd,
        SCRIPT = 1 << SIG_ENUM.Sript,
        LINT = 1 << SIG_ENUM.Lint,
        CHECK = 1 << SIG_ENUM.Check,
        EXEC = 1 << SIG_ENUM.Exec,
    }

    public sealed class Signal
    {
        public readonly SIG_FLAGS flags;
        public readonly CodeReader reader;
        public readonly Action<object, string> Stdout;
        public string exe_error;
        public bool is_exec;

        //----------------------------------------------------------------------------------------------------------

        public Signal(in SIG_FLAGS flags, in CodeReader reader, in Action<object, string> on_stdout)
        {
            this.flags = flags;
            this.reader = reader;
            is_exec = flags.HasFlag(SIG_FLAGS.EXEC);
            Stdout = on_stdout ?? ((data, lint) => Debug.Log(lint ?? data.ToString()));
        }
    }
}