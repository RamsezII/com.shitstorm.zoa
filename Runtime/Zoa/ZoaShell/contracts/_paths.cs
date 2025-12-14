using _COBRA_;
using UnityEngine;

namespace _ZOA_
{
    partial class ZoaShell
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void CMD_paths()
        {
            ZoaContract.AddContract(new(
                name: "path",
                output_type: typeof(CobraPath),
                parameters: new ZoaTypes(typeof(CobraPath)),
                action_SIG_EXE: static (exe, scope, opts, args) => exe.output = args[0]
            ));

            ZoaContract.AddContract(new(
                name: "dpath",
                output_type: typeof(CobraDPath),
                parameters: new ZoaTypes(typeof(CobraDPath)),
                action_SIG_EXE: static (exe, scope, opts, args) => exe.output = args[0]
            ));

            ZoaContract.AddContract(new(
                name: "fpath",
                output_type: typeof(CobraFPath),
                parameters: new ZoaTypes(typeof(CobraFPath)),
                action_SIG_EXE: static (exe, scope, opts, args) => exe.output = args[0]
            ));

            ZoaContract.AddContract(new(
                name: "cd",
                output_type: null,
                parameters: new ZoaTypes(typeof(CobraDPath)),
                action_SIG_EXE: static (exe, scope, opts, args) => exe.signal.shell.ChangeWorkdir(args[0].ToString())
            ));
        }
    }
}