using UnityEngine;

namespace _ZOA_
{
    static class CoreContracts
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Contract.AddContract(new(
                name: "path",
                output_type: Shell.T_path,
                parameters: new ZoaTypes(Shell.T_path),
                action_SIG_EXE: static (exe, scope, opts, prms) => exe.output = prms[0]
            ));
        }
    }
}