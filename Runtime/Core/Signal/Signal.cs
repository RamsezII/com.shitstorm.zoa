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
        Change,
        Lint,
        Check,
        Stdin,
        Tick,
    }

    [Flags]
    public enum SIG_FLAGS : byte
    {
        CMD = 1 << SIG_ENUM.Cmd,
        SCRIPT = 1 << SIG_ENUM.Sript,
        CHANGE = 1 << SIG_ENUM.Change,
        LINT = 1 << SIG_ENUM.Lint,
        CHECK = 1 << SIG_ENUM.Check,
        STDIN = 1 << SIG_ENUM.Stdin,
        TICK = 1 << SIG_ENUM.Tick,
    }

    public sealed class Signal
    {
        public readonly SIG_FLAGS flags;
        public readonly CodeReader reader;
        public readonly Action<object, string> Stdout;
        public string exe_error;
        public bool arm_executors;

        //----------------------------------------------------------------------------------------------------------

        public Signal(in SIG_FLAGS flags, in CodeReader reader, in Action<object, string> on_stdout)
        {
            this.flags = flags;
            this.reader = reader;
            arm_executors = flags.HasFlag(SIG_FLAGS.STDIN);
            Stdout = on_stdout ?? ((data, lint) => Debug.Log(lint ?? data.ToString()));
        }
    }
}