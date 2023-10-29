using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casting.Tests
{
    public class UlongToDoubleTests
    {
        private readonly UlongToDouble Test = new UlongToDouble();
        private readonly double[] Expected;

        public UlongToDoubleTests()
        {
            Expected = Test.Cast();
        }

        [Fact]
        public void NumericsCast()
        {
            var actual = Test.NumericsCast();

            Assert.Equal(Expected, actual);
        }

        [Fact]
        public void IntrinsicsCast()
        {
            var actual = Test.IntrinsicsCast();

            Assert.Equal(Expected, actual, new TolerantComparer(1e-15));
        }
    }

    internal class TolerantComparer : IEqualityComparer<double>
    {
        public TolerantComparer(double tolerance)
        {
            Tolerance = tolerance;
        }

        private readonly double Tolerance;

        public bool Equals(double x, double y)
        {
            var a = Math.Max(x, y);
            var b = Math.Min(x, y);

            return ((a - b) / a) < Tolerance;
        }

        public int GetHashCode([DisallowNull] double obj)
        {
            throw new NotImplementedException();
        }
    }
}
