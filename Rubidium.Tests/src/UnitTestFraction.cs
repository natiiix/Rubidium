using System;
using Xunit;
using Rubidium;

namespace Rubidium.Tests
{
    public static class UnitTestFraction
    {
        private const int SIZE_LIMIT = 1000000;

        [Fact]
        public static void TestZero()
        {
            Assert.True(Fraction.Zero.IsZero);

            Assert.False(Fraction.Zero.Positive);
            Assert.False(Fraction.Zero.Negative);

            Assert.Equal(Fraction.Zero, (Fraction)0);
            Assert.Equal(Fraction.Zero, (Fraction)0d);

            for (int i = 1; i < SIZE_LIMIT; i *= 2)
            {
                Assert.Equal(Fraction.Zero, new Fraction(0, i));
                Assert.Equal(Fraction.Zero, new Fraction(0, -i));
            }
        }

        [Fact]
        public static void TestOne()
        {
            Assert.True(Fraction.One.Positive);

            Assert.False(Fraction.One.IsZero);
            Assert.False(Fraction.One.Negative);

            Assert.Equal(Fraction.One, (Fraction)1);
            Assert.Equal(Fraction.One, (Fraction)1d);

            for (int i = 1; i < SIZE_LIMIT; i *= 2)
            {
                Assert.Equal(Fraction.One, new Fraction(i, i));
                Assert.Equal(Fraction.One, new Fraction(-i, -i));
            }
        }

        [Fact]
        public static void TestNegativeOne()
        {
            Assert.True(Fraction.NegativeOne.Negative);

            Assert.False(Fraction.NegativeOne.IsZero);
            Assert.False(Fraction.NegativeOne.Positive);

            Assert.Equal(Fraction.NegativeOne, (Fraction)(-1));
            Assert.Equal(Fraction.NegativeOne, (Fraction)(-1d));

            for (int i = 1; i < SIZE_LIMIT; i *= 2)
            {
                Assert.Equal(Fraction.NegativeOne, new Fraction(-i, i));
                Assert.Equal(Fraction.NegativeOne, new Fraction(i, -i));
            }
        }

        [Fact]
        public static void TestIsZero()
        {
            for (int i = 1; i < SIZE_LIMIT; i *= 2)
            {
                Fraction f = new Fraction(0, i);

                Assert.True(f.IsZero);

                Assert.False(f.Positive);
                Assert.False(f.Negative);
            }

            for (int i = -1; i > -SIZE_LIMIT; i *= 2)
            {
                Fraction f = new Fraction(0, i);

                Assert.True(f.IsZero);

                Assert.False(f.Positive);
                Assert.False(f.Negative);
            }
        }

        [Fact]
        public static void TestPositive()
        {
            for (int i = 1; i < SIZE_LIMIT; i *= 2)
            {
                for (int j = 1; j < SIZE_LIMIT; j *= 2)
                {
                    Fraction f = new Fraction(i, j);

                    Assert.True(f.Positive);

                    Assert.False(f.IsZero);
                    Assert.False(f.Negative);
                }
            }

            for (int i = -1; i > -SIZE_LIMIT; i *= 2)
            {
                for (int j = -1; j > -SIZE_LIMIT; j *= 2)
                {
                    Fraction f = new Fraction(i, j);

                    Assert.True(f.Positive);

                    Assert.False(f.IsZero);
                    Assert.False(f.Negative);
                }
            }
        }

        [Fact]
        public static void TestNegative()
        {
            for (int i = -1; i > -SIZE_LIMIT; i *= 2)
            {
                for (int j = 1; j < SIZE_LIMIT; j *= 2)
                {
                    Fraction f = new Fraction(i, j);

                    Assert.True(f.Negative);

                    Assert.False(f.IsZero);
                    Assert.False(f.Positive);
                }
            }

            for (int i = 1; i < SIZE_LIMIT; i *= 2)
            {
                for (int j = -1; j > -SIZE_LIMIT; j *= 2)
                {
                    Fraction f = new Fraction(i, j);

                    Assert.True(f.Negative);

                    Assert.False(f.IsZero);
                    Assert.False(f.Positive);
                }
            }
        }
    }
}
