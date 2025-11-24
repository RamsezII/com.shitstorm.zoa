using System.Collections.Generic;

namespace _ZOA_
{
    internal class UnaryExecutor : ExpressionExecutor
    {
        public enum Operators : byte
        {
            Add,
            Sub,
            Not,
        }

        readonly Operators code;
        readonly ExpressionExecutor expr;

        //----------------------------------------------------------------------------------------------------------

        public UnaryExecutor(in Signal signal, in MemScope scope, in ExpressionExecutor expr, in Operators code) : base(signal, scope)
        {
            this.code = code;
            this.expr = expr;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<ExecutionOutput> EExecution()
        {
            while (!expr.isDone)
            {
                ExecutionOutput output = expr.OnSignal(signal);
                if (expr.isDone)
                {
                    var data = output.data switch
                    {
                        int _i => code switch
                        {
                            Operators.Sub => -_i,
                            Operators.Not => _i switch
                            {
                                0 => 1,
                                _ => 0,
                            },
                            _ => _i,
                        },
                        float _f => code switch
                        {
                            Operators.Sub => -_f,
                            Operators.Not => _f switch
                            {
                                0f => 1f,
                                _ => 0f,
                            },
                            _ => _f,
                        },
                        _ => output.data,
                    };
                    yield return new(status: CMD_STATUS.RETURN, data: data);
                }
            }
        }
    }
}