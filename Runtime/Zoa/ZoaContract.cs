using System;
using System.Collections.Generic;
using UnityEngine;

namespace _ZOA_
{
    public sealed class Contract
    {
        public readonly string name;
        public readonly Type output_type;
        internal readonly Dictionary<(char short_name, string long_name), Type> options;
        internal readonly IEnumerable<Type> parameters;
        internal readonly Action<ZoaExecutor, MemScope, Dictionary<string, object>, List<object>> action_SIG_EXE;
        internal readonly Func<ZoaExecutor, MemScope, Dictionary<string, object>, List<object>, IEnumerator<ExecutionOutput>> routine_SIG_EXE, routine_SIG_ALL;

        internal static readonly Dictionary<string, Contract> contracts = new(StringComparer.OrdinalIgnoreCase);

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            contracts.Clear();
        }

        //----------------------------------------------------------------------------------------------------------

        public static void AddContract(in Contract contract, params string[] aliases)
        {
            contracts.Add(contract.name, contract);
            for (int i = 0; i < aliases.Length; i++)
                contracts.Add(aliases[i], contract);
        }

        public Contract(
            in string name,
            in Type output_type,
            in Dictionary<(char, string), Type> options = null,
            in IEnumerable<Type> parameters = null,
            in Action<ZoaExecutor, MemScope, Dictionary<string, object>, List<object>> action_SIG_EXE = null,
            in Func<ZoaExecutor, MemScope, Dictionary<string, object>, List<object>, IEnumerator<ExecutionOutput>> routine_SIG_EXE = null,
            in Func<ZoaExecutor, MemScope, Dictionary<string, object>, List<object>, IEnumerator<ExecutionOutput>> routine_SIG_ALL = null
            )
        {
            this.name = name;
            this.output_type = output_type;
            this.options = options;
            this.parameters = parameters;
            this.action_SIG_EXE = action_SIG_EXE;
            this.routine_SIG_EXE = routine_SIG_EXE;
            this.routine_SIG_ALL = routine_SIG_ALL;
        }
    }
}