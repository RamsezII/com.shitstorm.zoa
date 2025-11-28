using System.Collections.Generic;
using UnityEngine;

namespace _ZOA_
{
    static class Zoa_maths
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            Contract.AddContract(new(
                name: "echo",
                output_type: null,
                parameters: new ZoaTypes(Shell.T_object),
                action_SIG_EXE: static (exe, scope, opts, args) =>
                {
                    object msg = args[0];
                    exe.signal.Stdout(msg, null);
                }
            ));

            Contract.AddContract(new(
                name: "wait",
                output_type: null,
                parameters: new ZoaTypes(Shell.T_float),
                routine_SIG_EXE: static (exe, scope, opts, prms) =>
                {
                    return ERoutine(exe, prms);
                    static IEnumerator<ExecutionOutput> ERoutine(Executor exe, List<object> prms)
                    {
                        float time = (float)prms[0];
                        float timer = 0;
                        while (timer < time)
                        {
                            timer += Time.unscaledDeltaTime;
                            yield return new(progress: timer / time);
                        }
                        exe.signal.Stdout("wait end", null);
                    }
                }
            ));
        }
    }
}