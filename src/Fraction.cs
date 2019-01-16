using System;
using System.Numerics;

namespace Rubidium
{
    public class Fraction
    {
        public BigInteger Numerator { get; }
        public BigInteger Denominator { get; }

        public Fraction(BigInteger numerator, BigInteger denominator)
        {
            if (denominator == 0)
            {
                throw new DivideByZeroException();
            }

            Numerator = numerator;
            Denominator = denominator;
        }

        public Fraction(int numerator, int denominator = 1) : this((BigInteger)numerator, (BigInteger)denominator) { }
    }
}
