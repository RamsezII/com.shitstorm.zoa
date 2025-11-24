using _UTIL_;
using System.Collections.Generic;

namespace _ZOA_
{
    public sealed class Executor : Disposable
    {
        internal readonly Contract contract;
        internal readonly IEnumerator<ZoaData> routine;
        public readonly Dictionary<object, object> parameters = new();

        //----------------------------------------------------------------------------------------------------------

        internal Executor(in Signal signal, in Contract contract)
        {
            this.contract = contract;

            contract.parameters?.Invoke(this, signal);

            if (signal.reader.sig_error != null)
                return;

            if (signal.flags.HasFlag(SIG_FLAGS.EXEC))
                if (contract.routine != null)
                    routine = contract.routine(this);
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();

            routine?.Dispose();
            parameters.Clear();
        }
    }
}