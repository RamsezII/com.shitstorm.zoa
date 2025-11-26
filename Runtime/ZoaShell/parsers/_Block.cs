using System.Collections.Generic;

namespace _ZOA_
{
    partial class ZoaShell
    {
        internal bool TryParseBlock(in Signal signal, in MemScope scope, in TypeStack type_stack, in ValueStack value_stack, out Executor executor)
        {
            executor = null;

            if (signal.reader.TryReadChar_match('{'))
            {
                signal.reader.LintOpeningBraquet();

                var sub_scope = new MemScope(scope);
                List<Executor> exe_stack = new();

                while (TryParseBlock(signal, sub_scope, type_stack, value_stack, out Executor exe))
                    if (exe != null)
                        exe_stack.Add(exe);

                if (signal.reader.sig_error != null)
                    return false;

                if (signal.reader.TryReadChar_match('}', lint: signal.reader.CloseBraquetLint()))
                {
                    executor = new()
                    {
                        routine_SIG_ALL = Executor.EExecute_SIG_ALL(executor, exe_stack),
                    };
                    return true;
                }
                else
                    signal.reader.Stderr($"expected closing bracket '}}'.");
            }
            else if (TryParseInstruction(signal, scope, type_stack, value_stack, out executor))
                return true;

            return false;
        }
    }
}