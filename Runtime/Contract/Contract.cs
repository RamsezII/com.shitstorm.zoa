using System;
using System.Collections.Generic;
using UnityEngine;

namespace _ZOA_
{
    public sealed class Contract
    {
        internal readonly Action<Executor, Signal> parameters;
        internal readonly Func<Executor, ZoaData> function;
        internal readonly Func<Executor, IEnumerator<ZoaData>> routine;

        public static readonly Dictionary<string, Contract> contracts = new(StringComparer.OrdinalIgnoreCase);

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            contracts.Clear();
        }

        //----------------------------------------------------------------------------------------------------------

        public Contract(
            in Action<Executor, Signal> parameters = null,
            in Func<Executor, ZoaData> function = null,
            in Func<Executor, IEnumerator<ZoaData>> routine = null
            )
        {
            this.parameters = parameters;
            this.function = function;
            this.routine = routine;
        }
    }
}