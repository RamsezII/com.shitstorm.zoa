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
        public readonly string text;
        public readonly int cursor;

        public int read_i;

        public string output_text, output_lint;

        //----------------------------------------------------------------------------------------------------------

        public Signal(in SIG_FLAGS flags, in string text, in int cursor = 0)
        {
            this.flags = flags;
            this.text = text;
            this.cursor = cursor;

            output_text = output_lint = text;
        }

        //----------------------------------------------------------------------------------------------------------


    }
}