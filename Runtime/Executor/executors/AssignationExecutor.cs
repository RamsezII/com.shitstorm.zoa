using System.Collections.Generic;

namespace _ZOA_
{
    internal sealed class AssignationExecutor : Executor
    {
        readonly MemScope.MemCell mem_cell;
        readonly ExpressionExecutor expr_exe;

        //----------------------------------------------------------------------------------------------------------

        public AssignationExecutor(in Signal signal, in MemScope mem_scope, in string var_name, in ExpressionExecutor expr_exe) : base(signal, mem_scope)
        {
            if (!mem_scope.TryGetCell(var_name, out mem_cell))
                signal.reader.sig_error ??= $"could not find variable named \"{var_name}\" in current scope";
            else
            {
                mem_scope._variables.Add(var_name, mem_cell = new(type: null));
                this.expr_exe = expr_exe;
            }
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<ExecutionOutput> EExecution()
        {
            while (!expr_exe.isDone)
            {
                ExecutionOutput output = expr_exe.OnSignal(signal);
                if (expr_exe.isDone)
                {
                    mem_cell._value = output.data;
                    yield return new(status: CMD_STATUS.RETURN, data: output.data);
                }
            }
        }
    }
}