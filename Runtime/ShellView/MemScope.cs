using _UTIL_;
using System;
using System.Collections.Generic;

namespace _ZOA_
{
    sealed class MemScope : Disposable
    {
        readonly MemScope parent;
        readonly List<MemScope> sub_scopes = new();

        internal readonly List<object> stack = new();
        internal readonly Dictionary<string, object> heap = new(StringComparer.OrdinalIgnoreCase);

        //----------------------------------------------------------------------------------------------------------

        MemScope(in MemScope parent)
        {
            this.parent = parent;
        }

        internal MemScope()
        {

        }

        //----------------------------------------------------------------------------------------------------------

        public MemScope AddScope()
        {
            MemScope scope = new(this);
            sub_scopes.Add(scope);
            return scope;
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();

            for (int i = 0; i < sub_scopes.Count; i++)
                sub_scopes[i].Dispose();
            sub_scopes.Clear();

            stack.Clear();
            heap.Clear();
        }
    }
}