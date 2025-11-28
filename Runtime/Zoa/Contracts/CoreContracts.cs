using UnityEngine;

namespace _ZOA_
{
    static class CoreContracts
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Contract.AddContract(new(
                name: "run",
                output_type: null,
                parameters: new ZoaTypes(Shell.T_path),
                action_SIG_EXE: static (exe, scope, opts, prms) => exe.signal.Stdout(prms[0], null)
            ));
        }
    }
}