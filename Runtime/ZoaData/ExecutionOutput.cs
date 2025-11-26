using System;

namespace _ZOA_
{
    [Serializable]
    public readonly struct ExecutionOutput
    {
        public readonly CMD_STATUS status;
        public readonly LintedString prefixe;
        public readonly float progress;
        public readonly string error;

        //----------------------------------------------------------------------------------------------------------

        public ExecutionOutput(in CMD_STATUS status = 0, in LintedString prefixe = default, in float progress = 0, in string error = null)
        {
            this.status = status;
            this.prefixe = prefixe;
            this.progress = progress;
            this.error = error;
        }
    }
}