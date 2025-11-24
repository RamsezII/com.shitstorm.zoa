using System.Collections.Generic;

namespace _ZOA_
{
    internal sealed class TernaryOpExecutor : ExpressionExecutor
    {
        readonly Executor cond, _if, _else;

        //----------------------------------------------------------------------------------------------------------

        public TernaryOpExecutor(in Signal signal, in MemScope scope, in ExpressionExecutor cond, in Executor _if, in Executor _else) : base(signal, scope)
        {
            this.cond = cond;
            this._if = _if;
            this._else = _else;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<ExecutionOutput> EExecution()
        {
            bool condition = false;
            while (!cond.isDone)
            {
                ExecutionOutput output = cond.OnSignal(signal);
                if (cond.isDone)
                    condition = output.ToBool();
                else
                    yield return new(status: output.status, prefixe: output.prefixe, progress: output.progress);
            }

            Executor block = condition ? _if : _else;
            if (block != null)
                while (!block.isDone)
                {
                    ExecutionOutput output = block.OnSignal(signal);
                    if (block.isDone)
                        yield return new(CMD_STATUS.RETURN, data: output.data, lint: output.lint, progress: 1, error: output.error);
                    else
                        yield return output;
                }
        }
    }
}