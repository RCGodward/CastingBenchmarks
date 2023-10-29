using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
#if NET5_0_OR_GREATER
using System.Runtime.Intrinsics;
#endif

namespace Casting
{
    [SimpleJob(RuntimeMoniker.Net472)]
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net70)]
    //[SimpleJob(RuntimeMoniker.NativeAot70)]
    [MemoryDiagnoser]
    [DisassemblyDiagnoser(maxDepth:0)]
    [HideColumns("Error", "StdDev", "Median", "RatioSD")]
    public class UlongToUint
    {
        private const int N = 10000;
        private readonly ulong[] data;

        public UlongToUint()
        {
            data = new ulong[N];
            var rand = new Random((int)(DateTime.Now.Ticks & 0xffffffff));
            byte[] nextulong = new byte[4];
            for (int i = 0; i < N; i++)
            {
                rand.NextBytes(nextulong);
                data[i] = BitConverter.ToUInt32(nextulong, 0);
            }
        }

        [Benchmark]
        public uint[] Nothing() => new uint[N];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Cast(ulong value)
        {
            return (uint)value;
        }

        [Benchmark(Baseline = true)]
        public uint[] Cast()
        {
            uint[] result = new uint[N];
            for (int i = 0; i < N; ++i)
            {
                result[i] = Cast(data[i]);
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint UncheckedCast(ulong value)
        {
            return unchecked((uint)value);
        }

        [Benchmark]
        public uint[] UncheckedCast()
        {
            uint[] result = new uint[N];
            for (int i = 0; i < N; ++i)
            {
                result[i] = UncheckedCast(data[i]);
            }
            return result;
        }

        [Benchmark]
        public uint[] Linq() => data.Select(UncheckedCast).ToArray();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe uint UnsafeCast(ulong value)
        {
            return *(uint*)&value;
        }

        [Benchmark]
        public uint[] UnsafeCast()
        {
            uint[] result = new uint[N];
            for (int i = 0; i < N; ++i)
            {
                result[i] = UnsafeCast(data[i]);
            }
            return result;
        }

#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint VectorCast(ulong value)
        {
            return Vector256.Create(value).AsUInt32().GetElement(0);
        }
#endif

        [Benchmark]
        public uint[] VectorCast()
        {
#if NET5_0_OR_GREATER
            uint[] result = new uint[N];
            for (int i = 0; i < N; ++i)
            {
                result[i] = VectorCast(data[i]);
            }
            return result;
#else
            throw new NotImplementedException();
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint NumericsCast(ulong value)
        {
            return ((Vector<uint>)new Vector<ulong>(value))[0];
        }

        [Benchmark]
        public uint[] NumericsCast()
        {
            uint[] result = new uint[N];
            for (int i = 0; i < N; ++i)
            {
                result[i] = NumericsCast(data[i]);
            }
            return result;
        }
    }
}
