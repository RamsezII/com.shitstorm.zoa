using System;

namespace _ZOA_
{
    [Serializable]
    public readonly struct ExecutionOutput
    {
        public readonly CMD_STATUS status;
        public readonly object data;
        public readonly LintedString prefixe;
        public readonly float progress;
        public readonly string lint, error;

        //----------------------------------------------------------------------------------------------------------

        public ExecutionOutput(in CMD_STATUS status = 0, in LintedString prefixe = default, in object data = null, in string lint = null, in float progress = 0, in string error = null)
        {
            this.status = status;
            this.prefixe = prefixe;
            this.data = data;
            this.lint = lint ?? data?.ToString() ?? string.Empty;
            this.progress = progress;
            this.error = error;
        }
    }
}