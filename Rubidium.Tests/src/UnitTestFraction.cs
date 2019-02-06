using System;
using Xunit;
using Rubidium;

namespace Rubidium.Tests
{
    public class UnitTestFraction
    {
        [Fact]
        public void TestZeroPositiveNegative()
        {
            Assert.True(Fraction.Zero.IsZero);
            Assert.True(Fraction.One.Positive);
            Assert.True(Fraction.NegativeOne.Negative);

            Assert.False(Fraction.One.IsZero);
            Assert.False(Fraction.NegativeOne.IsZero);

            Assert.False(Fraction.Zero.Positive);
            Assert.False(Fraction.NegativeOne.Positive);

            Assert.False(Fraction.Zero.Negative);
            Assert.False(Fraction.One.Negative);
        }
    }
}
