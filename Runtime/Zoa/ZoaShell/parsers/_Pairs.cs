using System;
using System.Collections.Generic;

namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParsePair(
            in Signal signal,
            in TypeStack type_stack,
            ValueStack value_stack,
            OP_FLAGS op,
            ZoaExecutor exe_a,
            Type type_a,
            ZoaExecutor exe_b,
            Type type_b,
            out ZoaExecutor executor
        )
        {
            executor = null;

            if (!TryPair(signal, type_stack, typeof(bool)))
                if (!TryPair(signal, type_stack, typeof(int)))
                    if (!TryPair(signal, type_stack, typeof(float)))
                    {
                        signal.reader.sig_error ??= $"type inconsistency between {type_a} and {type_b}";
                        return false;
                    }

            bool TryPair(in Signal signal, in TypeStack type_stack, in Type attemtp)
            {
                if (type_a == attemtp && type_b == attemtp)
                {
                    type_stack.Push(attemtp);
                    return true;
                }
                return false;
            }

            var exe = executor;
            exe = executor = new()
            {
                routine_SIG_ALL = ERoutine_SIG_ALL(),
                action_SIG_EXE = exe =>
                {
                },
            };

            IEnumerator<ExecutionOutput> ERoutine_SIG_ALL()
            {
                object b = null;
                object a = null;

                while (!exe_a.isDone)
                {
                    ExecutionOutput output = exe_a.OnSignal(exe.signal);
                    yield return output;
                    if (exe_a.isDone)
                        a = value_stack.Pop();
                }

                while (!exe_b.isDone)
                {
                    ExecutionOutput output = exe_b.OnSignal(exe.signal);
                    yield return output;
                    if (exe_b.isDone)
                        b = value_stack.Pop();
                }

                object c = null;

                if (a is bool ab && b is bool bb)
                    c = op switch
                    {
                        OP_FLAGS.EQUAL => ab == bb,
                        OP_FLAGS.NOT_EQUAL => ab != bb,
                        OP_FLAGS.AND => ab && bb,
                        OP_FLAGS.OR => ab || bb,
                        OP_FLAGS.XOR => ab != bb,
                        _ => throw new NotImplementedException(),
                    };
                if (a is int ai && b is int bi)
                    c = op switch
                    {
                        OP_FLAGS.ADD => ai + bi,
                        OP_FLAGS.SUBSTRACT => ai - bi,
                        OP_FLAGS.MULTIPLY => ai * bi,
                        OP_FLAGS.DIVIDE => ai / bi,
                        OP_FLAGS.MODULO => ai % bi,
                        OP_FLAGS.EQUAL => ai == bi,
                        OP_FLAGS.GREATER_THAN => ai > bi,
                        OP_FLAGS.LESSER_THAN => ai < bi,
                        OP_FLAGS.AND => ai & bi,
                        OP_FLAGS.OR => ai | bi,
                        OP_FLAGS.XOR => ai ^ bi,
                        _ => throw new NotImplementedException(),
                    };
                else if (a is float af && b is float bf)
                    c = op switch
                    {
                        OP_FLAGS.ADD => af + bf,
                        OP_FLAGS.SUBSTRACT => af - bf,
                        OP_FLAGS.MULTIPLY => af * bf,
                        OP_FLAGS.DIVIDE => af / bf,
                        OP_FLAGS.MODULO => af % bf,
                        OP_FLAGS.EQUAL => af == bf,
                        OP_FLAGS.GREATER_THAN => af > bf,
                        OP_FLAGS.LESSER_THAN => af < bf,
                        _ => throw new NotImplementedException(),
                    };

                value_stack.Push(c);
            }

            return true;
        }
    }
}