using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
#if NET7_0_OR_GREATER
using System.Runtime.Intrinsics;
#endif

namespace Casting
{
    [SimpleJob(RuntimeMoniker.Net472)]
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net70)]
    [SimpleJob(RuntimeMoniker.NativeAot70)]
    [MemoryDiagnoser]
    //[DisassemblyDiagnoser(maxDepth: 0)]
    [HideColumns("Error", "StdDev", "Median", "RatioSD")]
    public class UlongToDouble
    {
        private const int N = 10000;
        private readonly ulong[] data;

        public UlongToDouble()
        {
            data = new ulong[N];
            var rand = new Random((int)(DateTime.Now.Ticks & 0xffffffff));
            byte[] nextulong = new byte[8];
            for (int i = 0; i < N; i++)
            {
                rand.NextBytes(nextulong);
                data[i] = BitConverter.ToUInt64(nextulong, 0);
            }
        }

        [Benchmark]
        public double[] Nothing() => new double[N];

        [Benchmark(Baseline = true)]
        public double[] Cast()
        {
            double[] result = new double[N];
            for (int i = 0; i < N; ++i)
            {
                result[i] = data[i];
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double NumericsCast(ulong value)
        {
            return Vector.ConvertToDouble(new Vector<ulong>(value))[0];
        }

        [Benchmark]
        public double[] NumericsCast()
        {
            double[] result = new double[N];

            for (int i = 0; i < N; ++i)
            {
                result[i] = NumericsCast(data[i]);
            }

            return result;
        }
#if NET7_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double IntrinsicsCast(ulong value)
        {
            return Vector128.ConvertToDouble(Vector128.Create(value)).GetElement(0);
        }
#endif

        [Benchmark]
        public double[] IntrinsicsCast()
        {
#if NET7_0_OR_GREATER
            double[] result = new double[N];
            for (int i = 0; i < N; ++i)
            {
                result[i] = IntrinsicsCast(data[i]);
            }
            return result;
#else
            throw new NotImplementedException();
#endif
        }
    }
}
