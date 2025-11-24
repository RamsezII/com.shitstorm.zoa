using System;
using System.Collections.Generic;
using UnityEngine;

namespace _ZOA_
{
    public sealed class Contract
    {
        public readonly string name;
        internal readonly Action<ContractExecutor, Signal> options;
        internal readonly Action<ContractExecutor, Signal> parameters;
        internal readonly Type output_type;
        internal readonly Func<ContractExecutor, object> function;
        internal readonly Func<ContractExecutor, IEnumerator<ExecutionOutput>> routine;

        public static readonly Dictionary<string, Contract> contracts = new(StringComparer.OrdinalIgnoreCase);

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            contracts.Clear();
        }

        //----------------------------------------------------------------------------------------------------------

        public Contract(
            in string name,
            in Action<ContractExecutor, Signal> options = null,
            in Action<ContractExecutor, Signal> parameters = null,
            in Type output_type = null,
            in Func<ContractExecutor, object> function = null,
            in Func<ContractExecutor, IEnumerator<ExecutionOutput>> routine = null
            )
        {
            this.name = name;
            this.options = options;
            this.parameters = parameters;
            this.output_type = output_type;
            this.function = function;
            this.routine = routine;
        }
    }
}