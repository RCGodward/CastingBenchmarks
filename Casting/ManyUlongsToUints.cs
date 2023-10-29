using System;
using System.Buffers;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if NET5_0_OR_GREATER
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
#endif
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Casting
{
    [SimpleJob(RuntimeMoniker.Net472)]
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net70)]
    //[SimpleJob(RuntimeMoniker.NativeAot70)]
    [MemoryDiagnoser]
    [DisassemblyDiagnoser(maxDepth: 0)]
    [HideColumns("Error", "StdDev", "Median", "RatioSD")]
    public class ManyUlongsToUints
    {
        private const int N = 10000;
        private readonly ulong[] data;

        public ManyUlongsToUints()
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

        [Benchmark(Baseline = true)]
        public uint[] Cast()
        {
            uint[] result = new uint[N];
            for (int i = 0; i < N; ++i)
            {
                result[i] = unchecked((uint)data[i]);
            }
            return result;
        }

        [Benchmark]
        public uint[] Linq() => data.Select(x => unchecked((uint)x)).ToArray();

        [Benchmark]
        public unsafe uint[] UnsafeCast()
        {
            uint[] result = new uint[N];
            fixed (ulong* src = data)
            fixed (uint* dst = result)
            {
                for (int i = 0; i < N; ++i)
                {
                    *(dst + i) = *(uint*)(src + i);
                }
            }
            return result;
        }

        [Benchmark]
        public uint[] Net70VectorCast()
        {
#if NET7_0_OR_GREATER
            var src = MemoryMarshal.Cast<ulong, uint>(data);
            uint[] result = new uint[N];

            var mask = Vector256.Create(0u, 2, 4, 6, 1, 3, 5, 7);

            ref uint current = ref MemoryMarshal.GetReference(src);
            ref uint dst = ref MemoryMarshal.GetReference(result.AsSpan());

            int count = Vector256<uint>.Count;

            ref uint endMinusOneVector = ref Unsafe.Add(ref dst, N - count);
            ref uint end = ref Unsafe.Add(ref dst, N);

            int i = 0;
            for (; i <= N - count; i += count)
            {
                var v1 = Vector256.LoadUnsafe(ref current);
                current = ref Unsafe.Add(ref current, count);

                var v2 = Vector256.LoadUnsafe(ref current);
                current = ref Unsafe.Add(ref current, count);

                v1 = Avx2.PermuteVar8x32(v1, mask);
                v2 = Avx2.PermuteVar8x32(v2, mask);

                Vector256.Create(v1.GetLower(), v2.GetLower()).StoreUnsafe(ref dst);

                dst = ref Unsafe.Add(ref dst, count);
            }
            for (; i < N; ++i)
            {
                dst = current;

                dst = ref Unsafe.Add(ref dst, 1);
                current = ref Unsafe.Add(ref current, 2);
            }

            return result;
#else
            throw new NotImplementedException();
#endif
        }

        [Benchmark]
        public unsafe uint[] Net60VectorCast()
        {
#if NET6_0_OR_GREATER
            var src = MemoryMarshal.Cast<ulong, uint>(data);
            uint[] result = new uint[N];

            fixed (uint* s = src, d = result)
            {

                var mask = Vector256.Create(0u, 2, 4, 6, 1, 3, 5, 7);

                int count = Vector256<uint>.Count;
                int srcOffset = 0, dstOffset = 0;
                while (dstOffset <= N - count)
                {
                    var v1 = Avx2.LoadVector256(s + srcOffset);
                    var v2 = Avx2.LoadVector256(s + srcOffset  + count);

                    v1 = Avx2.PermuteVar8x32(v1, mask);
                    v2 = Avx2.PermuteVar8x32(v2, mask);

                    Avx2.Store(d + dstOffset, Vector256.Create(v1.GetLower(), v2.GetLower()));
                    srcOffset += 2 * count;
                    dstOffset += count;
                }
                while (dstOffset < N)
                {
                    result[dstOffset] = (uint)data[dstOffset];
                }
            }

            return result;
#else
            throw new NotImplementedException();
#endif
        }

        [Benchmark]
        public uint[] NumericsCast()
        {
            uint[] result = new uint[N];

            var count = System.Numerics.Vector<uint>.Count;

            int i = 0;
            for (; i <= N - count; i += count)
            {
                var v1 = new System.Numerics.Vector<ulong>(data, i);
                var v2 = new System.Numerics.Vector<ulong>(data, i + count / 2);

                System.Numerics.Vector.Narrow(v1, v2).CopyTo(result, i);
            }

            return result;
        }
    }
}
