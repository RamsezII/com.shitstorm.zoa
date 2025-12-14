using UnityEngine;

namespace _ZOA_
{
    partial class ZoaShell
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void CMD_tests()
        {
            ZoaContract.AddContract(new(
                name: "test",
                output_type: typeof(bool),
                parameters: new ZoaTypes(typeof(bool)),
                action_SIG_EXE: static (exe, scope, opts, args) =>
                {
                    bool a = (bool)args[0];
                    exe.signal.Stdout("test: " + a, null);
                    exe.output = a;
                }
            ));
        }
    }
}