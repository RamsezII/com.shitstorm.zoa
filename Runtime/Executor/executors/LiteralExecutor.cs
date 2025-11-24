using System.Collections.Generic;

namespace _ZOA_
{
    public class LiteralExecutor : ExpressionExecutor
    {
        readonly object value;

        //----------------------------------------------------------------------------------------------------------

        public LiteralExecutor(in Signal signal, in MemScope scope, in object value) : base(signal, scope)
        {
            this.value = value;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<ExecutionOutput> EExecution()
        {
            yield return new(CMD_STATUS.RETURN, data: value, lint: value.ToString().SetColor(signal.reader.lint_theme.literal));
        }
    }
}