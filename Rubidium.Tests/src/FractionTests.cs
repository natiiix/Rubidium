using System;
using Xunit;
using Rubidium;

namespace Rubidium.Tests
{
    public static class FractionTests
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

            ForEachZero(x =>
            {
                Assert.Equal(Fraction.Zero, x);
                Assert.Equal(Fraction.Zero, x);
            });
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
            ForEachPositive(x =>
            {
                Assert.True(x.Positive);

                Assert.False(x.IsZero);
                Assert.False(x.Negative);
            });
        }

        [Fact]
        public static void TestNegative()
        {
            ForEachNegative(x =>
            {
                Assert.True(x.Negative);

                Assert.False(x.IsZero);
                Assert.False(x.Positive);
            });
        }

        [Fact]
        public static void TestAbsoluteValue()
        {
            ForEachPositive(x => Assert.Equal(x, x.AbsoluteValue));

            ForEachNegative(x =>
            {
                Assert.Equal(-x, x.AbsoluteValue);
                Assert.NotEqual(x, x.AbsoluteValue);
            });
        }

        [Fact]
        public static void TestInvert()
        {
            ForEachPositive(x =>
            {
                Fraction inverted = ~x;

                Assert.Equal(x.Numerator, inverted.Denominator);
                Assert.Equal(x.Denominator, inverted.Numerator);
            });

            ForEachNegative(x =>
            {
                Fraction inverted = ~x;

                Assert.Equal(-x.Numerator, inverted.Denominator);
                Assert.Equal(-x.Denominator, inverted.Numerator);
            });
        }

        [Fact]
        public static void TestSimplification()
        {
            ForEachFraction(x =>
            {
                for (int i = 1; i < SIZE_LIMIT; i *= 2)
                {
                    Assert.Equal(x, new Fraction(x.Numerator * i, x.Denominator * i));
                    Assert.Equal(x, new Fraction(x.Numerator * -i, x.Denominator * -i));
                }
            });
        }

        [Fact]
        public static void TestApproximatelyEqual()
        {
            // Zero.
            Assert.True(Fraction.Zero.ApproximatelyEqual(Fraction.Zero));
            Assert.True(Fraction.Zero.ApproximatelyEqual(new Fraction(1, 2000000000)));
            Assert.True(Fraction.Zero.ApproximatelyEqual(new Fraction(-1, 2000000000)));

            Assert.False(Fraction.Zero.ApproximatelyEqual(new Fraction(1, 200000000)));
            Assert.False(Fraction.Zero.ApproximatelyEqual(new Fraction(-1, 200000000)));

            // One.
            Assert.True(Fraction.One.ApproximatelyEqual(Fraction.One));
            Assert.True(Fraction.One.ApproximatelyEqual(new Fraction(2000000001, 2000000000)));
            Assert.True(Fraction.One.ApproximatelyEqual(new Fraction(1999999999, 2000000000)));

            Assert.False(Fraction.One.ApproximatelyEqual(new Fraction(200000001, 200000000)));
            Assert.False(Fraction.One.ApproximatelyEqual(new Fraction(199999999, 200000000)));

            // Negative one.
            Assert.True(Fraction.NegativeOne.ApproximatelyEqual(Fraction.NegativeOne));
            Assert.True(Fraction.NegativeOne.ApproximatelyEqual(new Fraction(-2000000001, 2000000000)));
            Assert.True(Fraction.NegativeOne.ApproximatelyEqual(new Fraction(-1999999999, 2000000000)));

            Assert.False(Fraction.NegativeOne.ApproximatelyEqual(new Fraction(-200000001, 200000000)));
            Assert.False(Fraction.NegativeOne.ApproximatelyEqual(new Fraction(-199999999, 200000000)));
        }

        private static void ForEachZero(Action<Fraction> callback)
        {
            for (int i = 1; i < SIZE_LIMIT; i *= 2)
            {
                callback(new Fraction(0, i));
                callback(new Fraction(0, -i));
            }
        }

        private static void ForEachPositive(Action<Fraction> callback)
        {
            for (int i = 1; i < SIZE_LIMIT; i *= 2)
            {
                for (int j = 1; j < SIZE_LIMIT; j *= 2)
                {
                    callback(new Fraction(i, j));
                }
            }

            for (int i = -1; i > -SIZE_LIMIT; i *= 2)
            {
                for (int j = -1; j > -SIZE_LIMIT; j *= 2)
                {
                    callback(new Fraction(i, j));
                }
            }
        }

        private static void ForEachNegative(Action<Fraction> callback)
        {
            for (int i = -1; i > -SIZE_LIMIT; i *= 2)
            {
                for (int j = 1; j < SIZE_LIMIT; j *= 2)
                {
                    callback(new Fraction(i, j));
                }
            }

            for (int i = 1; i < SIZE_LIMIT; i *= 2)
            {
                for (int j = -1; j > -SIZE_LIMIT; j *= 2)
                {
                    callback(new Fraction(i, j));
                }
            }
        }

        private static void ForEachNonZero(Action<Fraction> callback)
        {
            ForEachPositive(callback);
            ForEachNegative(callback);
        }

        private static void ForEachFraction(Action<Fraction> callback)
        {
            ForEachZero(callback);
            ForEachNonZero(callback);
        }
    }
}
