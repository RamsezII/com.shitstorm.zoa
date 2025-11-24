using _ARK_;
using _UTIL_;
using System;

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
        public Action<string, string> on_output;

        //----------------------------------------------------------------------------------------------------------

        public void Init()
        {
            RefreshPrefixe();
            Util.AddAction(ref NUCLEOR.delegates.Update_OnShellTick, OnTick);
        }

        //----------------------------------------------------------------------------------------------------------

        public abstract void OnSignal(in Signal signal);

        protected abstract void OnTick();

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            NUCLEOR.delegates.Update_OnShellTick -= OnTick;

            base.OnDispose();

            status.Dispose();
            prefixe.Dispose();
        }
    }
}