using _UTIL_;
using _ZOA_.Ast.compilation;
using System.Collections.Generic;

namespace _ZOA_.Ast.execution
{
    public sealed class Janitor : Disposable
    {
        internal readonly VScope vscope = new(parent: null);
        internal readonly VStack stack = new();
        internal readonly List<Executor> exe_stack = new();

        readonly IEnumerator<ExecutionOutput> routine;

        internal Signal signal;

        //----------------------------------------------------------------------------------------------------------

        internal Janitor(in AstProgram program)
        {
            for (int i = program.asts.Count - 1; i >= 0; i--)
                program.asts[i].OnExecutionStack(this);
            routine = ERoutine();
        }

        //----------------------------------------------------------------------------------------------------------

        IEnumerator<ExecutionOutput> ERoutine()
        {
            while (exe_stack.Count > 0)
            {
                using var exe = exe_stack[^1];
                exe_stack.RemoveAt(exe_stack.Count - 1);

                exe.action?.Invoke();

                if (exe.routine != null)
                    while (!exe.Disposed && exe.routine.MoveNext())
                        yield return exe.routine.Current;

                exe.onDone?.Invoke();
            }
        }

        public bool OnSignal(in Signal signal, out ExecutionOutput output)
        {
            if (!Disposed)
            {
                this.signal = signal;
                if (routine.MoveNext())
                {
                    output = routine.Current;
                    this.signal = null;
                    return true;
                }
                this.signal = null;
                Dispose();
            }
            output = default;
            return false;
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();

            routine.Dispose();

            for (int i = 0; i < exe_stack.Count; i++)
                exe_stack[i].Dispose();
            exe_stack.Clear();
        }
    }
}