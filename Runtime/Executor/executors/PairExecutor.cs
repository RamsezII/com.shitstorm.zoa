using System.Collections.Generic;

namespace _ZOA_
{
    partial class ZoaShell
    {
        class PairExecutor : ExpressionExecutor
        {
            readonly OP_FLAGS op;
            readonly ExpressionExecutor expr_a, expr_b;

            //----------------------------------------------------------------------------------------------------------

            public PairExecutor(in Signal signal, in MemScope mem_scope, in OP_FLAGS op, in ExpressionExecutor expr_a, in ExpressionExecutor expr_b) : base(signal, mem_scope)
            {
                this.op = op;
                this.expr_a = expr_a;
                this.expr_b = expr_b;
            }

            //----------------------------------------------------------------------------------------------------------

            internal override IEnumerator<ExecutionOutput> EExecution()
            {
                object a = null;
                object b = null;

                while (!expr_a.isDone)
                {
                    ExecutionOutput outpout = expr_a.OnSignal(signal);
                    if (expr_a.isDone)
                        a = outpout.data;
                    else
                        yield return outpout;
                }

                while (!expr_b.isDone)
                {
                    ExecutionOutput output = expr_b.OnSignal(signal);
                    if (expr_b.isDone)
                        b = output.data;
                    else
                        yield return output;
                }

                object data = null;

                if (a is int ai && b is int bi)
                    data = op switch
                    {
                        OP_FLAGS.add => ai + bi,
                        OP_FLAGS.sub => ai - bi,
                        OP_FLAGS.mul => ai * bi,
                        OP_FLAGS.div => ai / bi,
                        OP_FLAGS.mod => ai % bi,
                        OP_FLAGS.eq => ai == bi,
                        OP_FLAGS.gt => ai > bi,
                        OP_FLAGS.lt => ai < bi,
                        OP_FLAGS.and => ai & bi,
                        OP_FLAGS.or => ai | bi,
                        OP_FLAGS.xor => ai ^ bi,
                    };
                else if (a is float af && b is float bf)
                    data = op switch
                    {
                        OP_FLAGS.add => af + bf,
                        OP_FLAGS.sub => af - bf,
                        OP_FLAGS.mul => af * bf,
                        OP_FLAGS.div => af / bf,
                        OP_FLAGS.mod => af % bf,
                        OP_FLAGS.eq => af == bf,
                        OP_FLAGS.gt => af > bf,
                        OP_FLAGS.lt => af < bf,
                    };

                yield return new(status: CMD_STATUS.RETURN, data: data);
            }
        }
    }
}