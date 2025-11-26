using System;
using System.Collections;
using System.Collections.Generic;

namespace _ZOA_
{
    public class ZoaList<T> : IEnumerable<T>
    {
        public readonly List<T> _list = new();
        public ZoaList(params T[] elements) => _list.AddRange(elements);
        public ZoaList(in IEnumerable<T> elements) => _list.AddRange(elements);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }

    public sealed class ZoaTypes : ZoaList<Type>
    {
        public ZoaTypes(params Type[] elements) : base(elements)
        {
        }
        public ZoaTypes(in IEnumerable<Type> elements) : base(elements)
        {
        }
    }
}