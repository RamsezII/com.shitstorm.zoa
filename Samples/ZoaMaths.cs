using System.Collections.Generic;
using UnityEngine;

namespace _ZOA_
{
    static class Zoa_maths
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            Contract.contracts.Add("echo", new("echo",
                parameters: (exe, sig) =>
                {
                    if (sig.reader.TryParseString(out string s, true))
                        exe.parameters[0] = s;
                },
                function: static exe =>
                {
                    return exe.parameters[0];
                }
            ));

            Contract.contracts.Add("wait", new("wait",
                routine: static exe =>
                {
                    return ERoutine(exe);
                    static IEnumerator<ExecutionOutput> ERoutine(Executor exe)
                    {
                        float timer = 0;
                        while (timer < 1)
                        {
                            timer += Time.unscaledDeltaTime;
                            yield return new(progress: timer);
                        }
                    }
                }
            ));
        }
    }
}