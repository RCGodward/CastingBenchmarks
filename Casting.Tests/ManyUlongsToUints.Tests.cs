namespace Casting.Tests
{
    public class ManyUlongsToUintsUnitTests
    {
        private readonly ManyUlongsToUints Test = new ManyUlongsToUints();
        private readonly uint[] Expected;

        public ManyUlongsToUintsUnitTests()
        {
            Expected = Test.Cast();
        }

        [Fact]
        public void LinqCast()
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
        public void Net70VectorCast()
        {
            var actual = Test.Net70VectorCast();

            Assert.Equal(Expected, actual);
        }

        [Fact]
        public void Net60VectorCast()
        {
            var actual = Test.Net60VectorCast();

            Assert.Equal(Expected, actual);
        }

        [Fact]
        public void NumericsCast()
        {
            var actual = Test.NumericsCast();

            Assert.Equal(Expected, actual);
        }

        [Fact]
        public void SpanCast()
        {
            var actual = Test.SpanCast();

            Assert.Equal(Expected, actual);
        }
    }
}
