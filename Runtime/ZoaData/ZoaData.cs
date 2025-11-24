using System;

namespace _ZOA_
{
    [Serializable]
    public readonly struct ZoaData
    {
        public readonly object data;
        public readonly LintedString prefixe;
        public readonly float progress;
        public readonly string error;

        //----------------------------------------------------------------------------------------------------------

        public ZoaData(in LintedString prefixe = default, in object data = null, in float progress = 0, in string error = null)
        {
            this.prefixe = prefixe;
            this.data = data;
            this.progress = progress;
            this.error = error;
        }
    }
}