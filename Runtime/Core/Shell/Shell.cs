using _ARK_;
using _UTIL_;
using System;

namespace _ZOA_
{
    public enum CMD_STATUS : byte
    {
        BLOCKED,
        WAIT_FOR_STDIN,
        NETWORKING,
        RETURN,
        ERROR,
    }

    public abstract partial class Shell : Disposable
    {
        public readonly ValueHandler<CMD_STATUS> status = new();
        public readonly ValueHandler<LintedString> prefixe = new();
        public readonly MemScope mem_scope = new();
        public Action<object, string> on_output;

        //----------------------------------------------------------------------------------------------------------

        protected Shell()
        {
            prefixe.Value = RegularPrefixe();
        }

        //----------------------------------------------------------------------------------------------------------

        public virtual void Init()
        {
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