using _UTIL_;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _ZOA_
{
    public sealed class MemScope : Disposable
    {
        public class MemCell
        {
            public readonly Type _type;
            public object _value;

            //----------------------------------------------------------------------------------------------------------

            public MemCell(in Type type, in object value = null)
            {
                _type = type;
                _value = value;
            }

            //----------------------------------------------------------------------------------------------------------

            public void AssignValue(in object value)
            {
                _value = value;
            }
        }

        readonly MemScope parent;

        internal readonly Dictionary<string, MemCell> _variables = new(StringComparer.OrdinalIgnoreCase);

        //----------------------------------------------------------------------------------------------------------

        internal MemScope(in MemScope parent = null)
        {
            this.parent = parent;
        }

        //----------------------------------------------------------------------------------------------------------

        public IEnumerable<string> EVarNames()
        {
            if (parent != null)
                return _variables.Keys.Union(parent.EVarNames());
            return _variables.Keys;
        }

        public bool TryGetCell(in string name, out MemCell cell)
        {
            if (_variables.TryGetValue(name, out cell))
                return true;
            else if (parent != null)
                return parent.TryGetCell(name, out cell);
            return false;
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();
            _variables.Clear();
        }
    }
}