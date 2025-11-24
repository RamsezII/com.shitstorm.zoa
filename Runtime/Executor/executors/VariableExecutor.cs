using System.Collections.Generic;

namespace _ZOA_
{
    internal class VariableExecutor : ExpressionExecutor
    {
        internal readonly MemScope.MemCell mem_cell;

        //----------------------------------------------------------------------------------------------------------

        public VariableExecutor(in Signal signal, in MemScope scope, in string var_name) : base(signal, scope)
        {
            if (!mem_scope.TryGetCell(var_name, out mem_cell))
                signal.reader.sig_error ??= $"could not find variable named \"{var_name}\"";
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<ExecutionOutput> EExecution()
        {
            yield return new(status: CMD_STATUS.RETURN, data: mem_cell._value);
        }
    }
}