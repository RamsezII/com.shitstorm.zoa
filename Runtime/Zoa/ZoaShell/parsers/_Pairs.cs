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
            if (signal.arm_executors)
                exec_stack._stack.Add(new Executor("pair operator", expected_type)
                {
                    action_SIG_EXE = exe =>
                    {
                        object a = exe_a.output;
                        object b = exe_b.output;
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
                        else if (a is int a1 && b is int b1)
                            c = op switch
                            {
                                OP_FLAGS.ADD => a1 + b1,
                                OP_FLAGS.SUBSTRACT => a1 - b1,
                                OP_FLAGS.MULTIPLY => a1 * b1,
                                OP_FLAGS.DIVIDE => a1 / b1,
                                OP_FLAGS.MODULO => a1 % b1,
                                OP_FLAGS.EQUAL => a1 == b1,
                                OP_FLAGS.GREATER_THAN => a1 > b1,
                                OP_FLAGS.LESSER_THAN => a1 < b1,
                                OP_FLAGS.AND => a1 & b1,
                                OP_FLAGS.OR => a1 | b1,
                                OP_FLAGS.XOR => a1 ^ b1,
                                _ => throw new NotImplementedException(),
                            };
                        else if (a is float a2 && b is float b2)
                            c = op switch
                            {
                                OP_FLAGS.ADD => a2 + b2,
                                OP_FLAGS.SUBSTRACT => a2 - b2,
                                OP_FLAGS.MULTIPLY => a2 * b2,
                                OP_FLAGS.DIVIDE => a2 / b2,
                                OP_FLAGS.MODULO => a2 % b2,
                                OP_FLAGS.EQUAL => a2 == b2,
                                OP_FLAGS.GREATER_THAN => a2 > b2,
                                OP_FLAGS.LESSER_THAN => a2 < b2,
                                _ => throw new NotImplementedException(),
                            };
                        else if (a is int a3 && b is float b3)
                            c = op switch
                            {
                                OP_FLAGS.ADD => a3 + b3,
                                OP_FLAGS.SUBSTRACT => a3 - b3,
                                OP_FLAGS.MULTIPLY => a3 * b3,
                                OP_FLAGS.DIVIDE => a3 / b3,
                                OP_FLAGS.MODULO => a3 % b3,
                                OP_FLAGS.EQUAL => a3 == b3,
                                OP_FLAGS.GREATER_THAN => a3 > b3,
                                OP_FLAGS.LESSER_THAN => a3 < b3,
                                _ => throw new NotImplementedException(),
                            };
                        else if (a is float a4 && b is int b4)
                            c = op switch
                            {
                                OP_FLAGS.ADD => a4 + b4,
                                OP_FLAGS.SUBSTRACT => a4 - b4,
                                OP_FLAGS.MULTIPLY => a4 * b4,
                                OP_FLAGS.DIVIDE => a4 / b4,
                                OP_FLAGS.MODULO => a4 % b4,
                                OP_FLAGS.EQUAL => a4 == b4,
                                OP_FLAGS.GREATER_THAN => a4 > b4,
                                OP_FLAGS.LESSER_THAN => a4 < b4,
                                _ => throw new NotImplementedException(),
                            };
                        else
                            throw new NotImplementedException();

                        exe.output = c;
                    }
                });

            return true;
        }
    }
}