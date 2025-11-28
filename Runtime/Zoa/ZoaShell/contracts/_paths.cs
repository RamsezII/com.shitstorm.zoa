using UnityEngine;

namespace _ZOA_
{
    partial class ZoaShell
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void CMD_paths()
        {
            Contract.AddContract(new(
                name: "path",
                output_type: T_path,
                parameters: new ZoaTypes(T_path),
                action_SIG_EXE: static (exe, scope, opts, prms) => exe.output = prms[0]
            ));

            Contract.AddContract(new(
                name: "dpath",
                output_type: T_dpath,
                parameters: new ZoaTypes(T_dpath),
                action_SIG_EXE: static (exe, scope, opts, prms) => exe.output = prms[0]
            ));

            Contract.AddContract(new(
                name: "fpath",
                output_type: T_fpath,
                parameters: new ZoaTypes(T_fpath),
                action_SIG_EXE: static (exe, scope, opts, prms) => exe.output = prms[0]
            ));

            Contract.AddContract(new(
                name: "cd",
                output_type: null,
                parameters: new ZoaTypes(T_dpath),
                action_SIG_EXE: static (exe, scope, opts, prms) => exe.signal.shell.ChangeWorkdir((string)prms[0])
            ));
        }
    }
}