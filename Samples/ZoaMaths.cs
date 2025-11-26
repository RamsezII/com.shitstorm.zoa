using System.Collections.Generic;
using UnityEngine;

namespace _ZOA_
{
    static class Zoa_maths
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            Contract.AddContract(new("echo",
                parameters: (exe, sig, tstack) =>
                {
                    tstack.Pop();
                },
                action_SIG_EXE: static (exe, scope, vstack) =>
                {
                    object msg = vstack.Peek();
                    exe.signal.Stdout(msg, msg.ToString().SetColor(Colors.yellow));
                }
            ));

            Contract.AddContract(new("wait",
                routine_SIG_EXE: static (exe, scope, vstack) =>
                {
                    return ERoutine(exe);
                    static IEnumerator<ExecutionOutput> ERoutine(ZoaExecutor exe)
                    {
                        float timer = 0;
                        while (timer < 1)
                        {
                            timer += Time.unscaledDeltaTime;
                            yield return new(progress: timer);
                        }
                        exe.signal.Stdout("wait end", null);
                    }
                }
            ));
        }
    }
}