using System;
using System.Numerics;

namespace Rubidium
{
    public class Fraction
    {
        public BigInteger Numerator { get; private set; }
        public BigInteger Denominator { get; private set; }

        public Fraction(BigInteger numerator, BigInteger denominator)
        {
            if (denominator.IsZero)
            {
                throw new DivideByZeroException();
            }

            Numerator = numerator;
            Denominator = denominator;

            Normalize();
        }

        public Fraction(BigInteger numerator) : this(numerator, 1) { }

        private void Normalize()
        {
            if (Denominator.Sign < 0)
            {
                Denominator = -Denominator;
                Numerator = -Numerator;
            }

            BigInteger gcd = BigInteger.GreatestCommonDivisor(Numerator, Denominator);

            if (!gcd.IsOne)
            {
                Numerator /= gcd;
                Denominator /= gcd;
            }
        }

        public Fraction(int numerator, int denominator = 1) : this((BigInteger)numerator, (BigInteger)denominator) { }

        public static Fraction FromDouble(double value)
        {
            const int PRECISION = 15;
            const int EXPONENT_VALUE_LENGTH = 4;
            const int FULL_EXPONENT_LENGTH = EXPONENT_VALUE_LENGTH + 1;

            string valueString = value.ToString($"e{PRECISION}");
            int decimalSeparatorIndex = valueString.Length - PRECISION - FULL_EXPONENT_LENGTH - 1;

            string mantissaString = valueString.Substring(0, decimalSeparatorIndex) + valueString.Substring(decimalSeparatorIndex + 1, PRECISION);
            string exponentString = valueString.Substring(valueString.Length - EXPONENT_VALUE_LENGTH, EXPONENT_VALUE_LENGTH);

            int exponent = int.Parse(exponentString) - PRECISION;

            BigInteger numerator = BigInteger.Parse(mantissaString) * (exponent >= 0 ? PowerOf10(exponent) : 1);
            BigInteger denominator = exponent < 0 ? PowerOf10(-exponent) : 1;

            return new Fraction(numerator, denominator);
        }

        public override string ToString() => $"({Numerator}/{Denominator})";

        public static implicit operator Fraction(BigInteger b) => new Fraction(b);

        public static implicit operator Fraction(int i) => new Fraction(i);

        public static explicit operator Fraction(double d) => FromDouble(d);

        public static explicit operator double(Fraction f) => (double)f.Numerator / (double)f.Denominator;

        public static Fraction operator -(Fraction f) => new Fraction(-f.Numerator, f.Denominator);

        public static Fraction operator ~(Fraction f) => new Fraction(f.Denominator, f.Numerator);

        public static Fraction operator +(Fraction first, Fraction second) =>
            new Fraction((first.Numerator * second.Denominator) + (second.Numerator * first.Denominator), first.Denominator * second.Denominator);

        public static Fraction operator -(Fraction first, Fraction second) => first + -second;

        public static Fraction operator *(Fraction first, Fraction second) =>
            new Fraction(first.Numerator * second.Numerator, first.Denominator * second.Denominator);

        public static Fraction operator /(Fraction first, Fraction second) =>
            new Fraction(first.Numerator * second.Denominator, first.Denominator * second.Numerator);

        public static Fraction operator ^(Fraction value, Fraction exponent)
        {
            if (exponent.Denominator != 1 || exponent.Numerator < 0 || exponent.Numerator > int.MaxValue)
            {
                throw new NotImplementedException($"Exponent must be within the range 0 - {int.MaxValue}");
            }

            int exp = (int)exponent.Numerator;
            return new Fraction(BigInteger.Pow(value.Numerator, exp), BigInteger.Pow(value.Denominator, exp));
        }

        private static BigInteger PowerOf10(int power) => BigInteger.Pow(10, power);
    }
}
