using System.Collections.Generic;

namespace _ZOA_
{
    internal class IncrementExecutor : ExpressionExecutor
    {
        public enum Operators : byte
        {
            None,
            AddBefore,
            SubBefore,
            AddAfter,
            SubAfter,
        }

        readonly Operators code;
        readonly string var_name;

        //----------------------------------------------------------------------------------------------------------

        internal IncrementExecutor(in Signal signal, in MemScope scope, in string var_name, in Operators code) : base(signal, scope)
        {
            this.code = code;
            this.var_name = var_name;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<ExecutionOutput> EExecution()
        {
            if (mem_scope.TryGetCell(var_name, out MemScope.MemCell cell))
            {
                var value = cell._value switch
                {
                    int _int => code switch
                    {
                        Operators.AddBefore => ++_int,
                        Operators.SubBefore => --_int,
                        Operators.AddAfter => _int++,
                        Operators.SubAfter => _int--,
                        _ => _int,
                    },
                    float _float => code switch
                    {
                        Operators.AddBefore => ++_float,
                        Operators.SubBefore => --_float,
                        Operators.AddAfter => _float++,
                        Operators.SubAfter => _float--,
                        _ => _float,
                    },
                    _ => cell._value,
                };
                yield return new(CMD_STATUS.RETURN, data: value);
            }
            else
                signal.reader.sig_error ??= $"no variable named \"{var_name}\" in current scope";
        }
    }
}