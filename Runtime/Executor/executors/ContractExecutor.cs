using System.Collections.Generic;

namespace _ZOA_
{
    public class ContractExecutor : ExpressionExecutor
    {
        internal readonly Contract contract;
        public readonly Dictionary<object, object> parameters = new();
        readonly IEnumerator<ExecutionOutput> routine;

        //----------------------------------------------------------------------------------------------------------

        public ContractExecutor(in Signal signal, in MemScope scope, in Contract contract, in bool parse_arguments = true) : base(signal, scope)
        {
            this.contract = contract;

            if (parse_arguments)
            {
                contract.options?.Invoke(this, signal);

                bool expects_parenthesis = signal.reader.strict_syntax;
                bool found_parenthesis = signal.reader.TryReadChar_match('(');

                if (found_parenthesis)
                    signal.reader.LintOpeningBraquet();

                if (expects_parenthesis && !found_parenthesis)
                {
                    signal.reader.Stderr($"'{contract.name}' expected opening parenthesis '('");
                    return;
                }

                contract.parameters?.Invoke(this, signal);

                if (signal.reader.sig_error != null)
                    return;

                if ((expects_parenthesis || found_parenthesis) && !signal.reader.TryReadChar_match(')', lint: signal.reader.CloseBraquetLint()))
                {
                    signal.reader.Stderr($"'{contract.name}' expected closing parenthesis ')'");
                    return;
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<ExecutionOutput> EExecution()
        {
            if (contract.function != null)
            {
                ExecutionOutput output = new(CMD_STATUS.RETURN, data: contract.function(this));
                yield return new(status: CMD_STATUS.RETURN, data: output);
            }

            while (routine.MoveNext())
                yield return routine.Current;

            yield return new(CMD_STATUS.RETURN, data: routine.Current.data, progress: 1, error: routine.Current.error);
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();
            routine?.Dispose();
        }
    }
}