using System;

namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParsePair(
            in Signal signal,
            Type expected_type,
            OP_FLAGS op,
            Executor exe_a,
            Executor exe_b,
            in ExecutionStack exec_stack)
        {
            if (signal.is_exec)
                exec_stack.Push(new Executor(expected_type)
                {
                    action_SIG_EXE = exe =>
                    {
                        object a = exe_a.output_data;
                        object b = exe_b.output_data;
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

                        exe.output_data = c;
                    }
                });

            return true;
        }
    }
}