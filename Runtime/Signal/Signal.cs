using _COBRA_;
using System;

namespace _ZOA_
{
    enum SIG_ENUM : byte
    {
        _NONE_,
        Lint,
        Check,
        Exec,
    }

    [Flags]
    public enum SIG_FLAGS : byte
    {
        LINT = 1 << SIG_ENUM.Lint,
        CHECK = 1 << SIG_ENUM.Check,
        EXEC = 1 << SIG_ENUM.Exec,
    }

    public sealed class Signal
    {
        public readonly SIG_FLAGS flags;
        public CodeReader reader;

        //----------------------------------------------------------------------------------------------------------

        public Signal(in SIG_FLAGS flags, in CodeReader reader)
        {
            this.flags = flags;
            this.reader = reader;
        }

        //----------------------------------------------------------------------------------------------------------


    }
}