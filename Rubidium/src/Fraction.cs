using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Rubidium
{
    public class Fraction : IComparable<Fraction>
    {
        public static char DecimalSeparator => '.';

        public static Fraction Zero => new Fraction(0);
        public static Fraction One => new Fraction(1);
        public static Fraction NegativeOne => new Fraction(-1);

        public BigInteger Numerator { get; private set; }
        public BigInteger Denominator { get; private set; }

        public bool IsZero => Numerator.IsZero;
        public bool Positive => Numerator.Sign > 0;
        public bool Negative => Numerator.Sign < 0;
        public bool IsWholeNumber => Denominator.IsOne;
        public Fraction AbsoluteValue => Positive ? this : -this;
        public Fraction Square => new Fraction(Numerator * Numerator, Denominator * Denominator);
        public Fraction SquareRoot => (Fraction)Math.Sqrt((double)this);

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

            if (!double.IsFinite(value))
            {
                throw new ArgumentException("Value must be finite");
            }

            string valueString = value.ToString($"e{PRECISION}");
            int decimalSeparatorIndex = valueString.Length - PRECISION - FULL_EXPONENT_LENGTH - 1;

            string mantissaString = valueString.Substring(0, decimalSeparatorIndex) + valueString.Substring(decimalSeparatorIndex + 1, PRECISION);
            string exponentString = valueString.Substring(valueString.Length - EXPONENT_VALUE_LENGTH, EXPONENT_VALUE_LENGTH);

            int exponent = int.Parse(exponentString) - PRECISION;

            BigInteger numerator = BigInteger.Parse(mantissaString) * (exponent >= 0 ? PowerOf10(exponent) : 1);
            BigInteger denominator = exponent < 0 ? PowerOf10(-exponent) : 1;

            return new Fraction(numerator, denominator);
        }

        public static bool TryParse(string str, out Fraction value)
        {
            BigInteger numerator = 0;

            bool decimalPart = false;
            int decimalDigits = 0;

            foreach (char c in str)
            {
                if (char.IsDigit(c))
                {
                    numerator *= 10;
                    numerator += c - '0';

                    if (decimalPart)
                    {
                        decimalDigits++;
                    }
                }
                else if (!decimalPart && c == DecimalSeparator)
                {
                    decimalPart = true;
                }
                else
                {
                    value = Fraction.Zero;
                    return false;
                }
            }

            value = new Fraction(numerator, BigInteger.Pow(10, decimalDigits));
            return false;
        }

        public static Fraction Parse(string str) =>
            TryParse(str, out Fraction value) ? value : throw new ArgumentException($"Not a valid number: {str}");

        public static Fraction Sum(IEnumerable<Fraction> values)
        {
            Fraction sum = 0;

            foreach (Fraction f in values)
            {
                sum += f;
            }

            return sum;
        }

        public static Fraction Average(IEnumerable<Fraction> values) => Sum(values) / values.Count();

        public int CompareTo(Fraction other) => (this - other).Numerator.Sign;

        public override bool Equals(object obj) => obj is Fraction f && f.Numerator == Numerator && f.Denominator == Denominator;

        public override int GetHashCode() => Numerator.GetHashCode() ^ Denominator.GetHashCode();

        public override string ToString() => IsWholeNumber ? Numerator.ToString() : $"({Numerator}/{Denominator})";

        public static implicit operator Fraction(BigInteger b) => new Fraction(b);

        public static implicit operator Fraction(int i) => new Fraction(i);

        public static explicit operator Fraction(double d) => FromDouble(d);
        public static implicit operator Fraction(ConstantExpression c) => c.Value;

        public static explicit operator double(Fraction f) => (double)f.Numerator / (double)f.Denominator;

        public static implicit operator ConstantExpression(Fraction f) => new ConstantExpression(f);

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
            if (exponent.IsWholeNumber && exponent.Numerator >= int.MinValue && exponent.Numerator <= int.MaxValue)
            {
                int exp = (int)exponent.Numerator;

                if (exp > 0)
                {
                    return new Fraction(BigInteger.Pow(value.Numerator, exp), BigInteger.Pow(value.Denominator, exp));
                }
                else if (exp < 0)
                {
                    return new Fraction(BigInteger.Pow(value.Denominator, -exp), BigInteger.Pow(value.Numerator, -exp));
                }
                else
                {
                    return Fraction.One;
                }
            }
            else
            {
                // throw new NotImplementedException($"Exponent must be an integer within the range {int.MinValue} - {int.MaxValue}");

                double result = Math.Pow((double)value, (double)exponent);
                return (Fraction)result;
            }
        }

        public static bool operator ==(Fraction first, Fraction second) => first.Equals(second);

        public static bool operator !=(Fraction first, Fraction second) => !first.Equals(second);

        public static bool operator <(Fraction first, Fraction second) => first.CompareTo(second) < 0;

        public static bool operator <=(Fraction first, Fraction second) => first.CompareTo(second) <= 0;

        public static bool operator >(Fraction first, Fraction second) => first.CompareTo(second) > 0;

        public static bool operator >=(Fraction first, Fraction second) => first.CompareTo(second) >= 0;

        private static BigInteger PowerOf10(int power) => BigInteger.Pow(10, power);
    }
}
