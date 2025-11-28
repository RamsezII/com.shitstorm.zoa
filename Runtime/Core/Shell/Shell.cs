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

        public static readonly Type
            T_object = typeof(object),
            T_bool = typeof(bool),
            T_int = typeof(int),
            T_float = typeof(float),
            T_string = typeof(string),
            T_path = typeof(ZoaPath),
            T_fpath = typeof(ZoaFPath),
            T_dpath = typeof(ZoaDPath);

        //----------------------------------------------------------------------------------------------------------

        protected Shell()
        {
            prefixe.Value = RegularPrefixe();
        }

        //----------------------------------------------------------------------------------------------------------

        public void Init()
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