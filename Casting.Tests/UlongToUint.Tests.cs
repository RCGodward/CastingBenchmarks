using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casting.Tests
{
    public class UlongToUintTests
    {
        private readonly UlongToUint Test = new UlongToUint();
        private readonly uint[] Expected;

        public UlongToUintTests()
        {
            Expected = Test.Cast();
        }

        [Fact]
        public void UncheckedCast()
        {
            var actual = Test.UncheckedCast();

            Assert.Equal(Expected, actual);
        }

        [Fact]
        public void Linq()
        {
            var actual = Test.Linq();

            Assert.Equal(Expected, actual);
        }

        [Fact]
        public void UnsafeCast()
        {
            var actual = Test.UnsafeCast();

            Assert.Equal(Expected, actual);
        }

        [Fact]
        public void VectorCast()
        {
            var actual = Test.VectorCast();

            Assert.Equal(Expected, actual);
        }

        [Fact]
        public void NumericsCast()
        {
            var actual = Test.NumericsCast();

            Assert.Equal(Expected, actual);
        }
    }
}
