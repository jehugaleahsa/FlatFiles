using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace FlatFiles.Benchmark
{
    public class ArrayCopyVsCustomTester
    {
        private readonly char[] values = new char[4096];
        private readonly char[] destination = new char[4096];

        public ArrayCopyVsCustomTester()
        {
            for (int i = 0; i != values.Length; ++i)
            {
                values[i] = (char)i;
            }
        }

        [Benchmark]
        public void ArrayCopyTest()
        {
            Array.Copy(values, 0, destination, 0, values.Length);
        }

        [Benchmark]
        public void CustomCopyTest()
        {
            copy(values, 0, destination, 0, values.Length);
        }

        private static void copy<T>(T[] source, int sourceIndex, T[] destination, int destinationIndex, int length)
        {
            while (length != 0)
            {
                destination[destinationIndex] = source[sourceIndex];
                ++sourceIndex;
                ++destinationIndex;
                --length;
            }
        }
    }
}
