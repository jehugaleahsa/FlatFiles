using System;

namespace FlatFiles
{
    internal static class ValueTuple
    {
        public static ValueTuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
        {
            return new ValueTuple<T1, T2>(item1, item2);
        }
    }

    internal struct ValueTuple<T1, T2>
    {
        public ValueTuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public void Deconstruct(out T1 item1, out T2 item2)
        {
            item1 = Item1;
            item2 = Item2;
        }

        public T1 Item1 { get; private set; }

        public T2 Item2 { get; private set; }
    }
}
