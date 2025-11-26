using System;

namespace _ZOA_
{
    public sealed class MemCell
    {
        public readonly Type type;
        public object value;
        public override string ToString() => $"{{ {nameof(value)}: \"{value}\" : {nameof(type)}: {type} }}";

        //----------------------------------------------------------------------------------------------------------

        public MemCell(in Type type, in object value = null)
        {
            this.type = type;
            this.value = value;
        }

        //----------------------------------------------------------------------------------------------------------

        public bool AcceptsType(in Type type, out string error)
        {
            if (this.type == null || type.IsOfType(this.type))
            {
                error = null;
                return true;
            }
            error = $"{this.type} does not accept {type}";
            return false;
        }
    }
}