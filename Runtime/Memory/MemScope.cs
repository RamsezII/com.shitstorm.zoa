using _UTIL_;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _ZOA_
{
    public sealed partial class MemScope : Disposable
    {
        readonly MemScope parent;
        internal readonly Dictionary<string, MemCell> _vars = new(StringComparer.OrdinalIgnoreCase);

        //----------------------------------------------------------------------------------------------------------

        internal MemScope(in MemScope parent = null)
        {
            this.parent = parent;
        }

        //----------------------------------------------------------------------------------------------------------

        public IEnumerable<string> EVarNames()
        {
            if (parent != null)
                return _vars.Keys.Union(parent.EVarNames());
            return _vars.Keys;
        }

        public bool TryGetCell(in string name, out MemCell cell)
        {
            if (_vars.TryGetValue(name, out cell))
                return true;
            else if (parent != null)
                return parent.TryGetCell(name, out cell);
            return false;
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();
            _vars.Clear();
        }
    }
}