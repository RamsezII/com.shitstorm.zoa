using System;
using System.Collections.Generic;
using UnityEngine;

namespace _ZOA_
{
    public sealed class Contract
    {
        public readonly string name;
        internal readonly Action<Executor, Signal, TypeStack> options;
        internal readonly Action<Executor, Signal, TypeStack> parameters;
        internal readonly Action<Executor, MemScope, ValueStack> action_SIG_EXE;
        internal readonly Func<Executor, MemScope, ValueStack, IEnumerator<ExecutionOutput>> routine_SIG_EXE, routine_SIG_ALL;

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
            in Action<Executor, Signal, TypeStack> options = null,
            in Action<Executor, Signal, TypeStack> parameters = null,
            in Action<Executor, MemScope, ValueStack> action_SIG_EXE = null,
            in Func<Executor, MemScope, ValueStack, IEnumerator<ExecutionOutput>> routine_SIG_EXE = null,
            in Func<Executor, MemScope, ValueStack, IEnumerator<ExecutionOutput>> routine_SIG_ALL = null
            )
        {
            this.name = name;
            this.options = options;
            this.parameters = parameters;
            this.action_SIG_EXE = action_SIG_EXE;
            this.routine_SIG_EXE = routine_SIG_EXE;
            this.routine_SIG_ALL = routine_SIG_ALL;
        }
    }
}