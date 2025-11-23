using _UTIL_;

namespace _ZOA_
{
    public abstract partial class Shell : Disposable
    {
        public enum STATUS : byte
        {
            WAIT_FOR_STDIN,
            BLOCKED,
            NETWORKING,
        }

        public readonly ValueHandler<STATUS> status = new();
        public readonly ValueHandler<LintedString> prefixe = new();

        //----------------------------------------------------------------------------------------------------------

        public void Init()
        {
            RefreshPrefixe();
        }

        //----------------------------------------------------------------------------------------------------------

        public virtual void OnSignal(in Signal signal)
        {

        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();

            status.Dispose();
            prefixe.Dispose();
        }
    }
}