using System.Collections.Generic;
using UnityEngine;

namespace _ZOA_
{
    partial class ZoaShell
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void CMD_basics()
        {
            Contract.AddContract(new(
                name: "echo",
                output_type: null,
                parameters: new ZoaTypes(T_object),
                action_SIG_EXE: static (exe, scope, opts, args) =>
                {
                    object msg = args[0];
                    exe.signal.Stdout(msg, null);
                }
            ));

            Contract.AddContract(new(
                name: "wait",
                output_type: null,
                parameters: new ZoaTypes(T_float),
                routine_SIG_EXE: static (exe, scope, opts, prms) =>
                {
                    return ERoutine(exe, prms);
                    static IEnumerator<ExecutionOutput> ERoutine(Executor exe, List<object> prms)
                    {
                        float time = prms[0] switch
                        {
                            int _i => _i,
                            _ => (float)prms[0],
                        };

                        float timer = 0;
                        while (timer < time)
                        {
                            timer += Time.unscaledDeltaTime;
                            yield return new(progress: timer / time);
                        }
                    }
                }
            ));

            Contract.AddContract(new(
                name: "stdin",
                output_type: T_string,
                parameters: new ZoaTypes(T_string),
                routine_SIG_ALL: static (exe, scope, opts, prms) =>
                {
                    return ERoutine(exe, prms);
                    static IEnumerator<ExecutionOutput> ERoutine(Executor exe, List<object> prms)
                    {
                        string prefixe = prms[0].ToString();
                        while (true)
                            if (!exe.signal.flags.HasFlag(SIG_FLAGS.SUBMIT))
                                yield return new(CMD_STATUS.WAIT_FOR_STDIN, prefixe: new(prefixe, Colors.alice_blue));
                            else
                            {
                                string stdin = exe.signal.reader.ReadAll();
                                exe.output = stdin;
                                break;
                            }
                    }
                }
            ));
        }
    }
}