using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Rubidium
{
    /// <summary>
    /// Class representing a numeric value as a fraction.
    /// The value can either be a whole number of an arbitrary size or
    /// a decimal number of an arbitrary size and precision.
    /// Fraction supports most arithmetic and comparison operations.
    /// </summary>
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

        public Fraction(int numerator, int denominator = 1) : this((BigInteger)numerator, (BigInteger)denominator) { }

        /// <summary>
        /// Normalizes the Fraction to make it easier to work with.
        /// Denominator must always be positive.
        /// Numerator and denominator must not have a common divisor greater than 1.
        /// </summary>
        private void Normalize()
        {
            // If denominator is negative, negate both parts of Fraction.
            if (Denominator.Sign < 0)
            {
                Numerator = -Numerator;
                Denominator = -Denominator;
            }

            // Find greatest common divisor of numerator and denominator.
            BigInteger gcd = BigInteger.GreatestCommonDivisor(Numerator, Denominator);

            // Divide both parts of Fraction by their greatest common divisor.
            if (!gcd.IsOne)
            {
                Numerator /= gcd;
                Denominator /= gcd;
            }
        }

        /// <summary>
        /// Parses a Fraction from a double-precision floating-point value.
        /// This function is very slow because it first converts the double to string
        /// in scientific notation and then parses the Fraction from the string.
        /// </summary>
        /// <param name="value">Double value to parse into a Fraction.</param>
        /// <returns>Returns parsed Fraction.</returns>
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

        /// <summary>
        /// Attempts to parse a Fraction from a string representation of a numeric value.
        /// Only standard ### and ###.## notation is allowed (scientific notation is NOT supported).
        /// </summary>
        /// <param name="str">Input string to parse the Fraction from.</param>
        /// <param name="value">Output parsed Fraction.</param>
        /// <returns>Returns boolean value indicating if a Fraction has been successfully parsed.</returns>
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

            value = new Fraction(numerator, PowerOf10(decimalDigits));
            return true;
        }

        /// <summary>
        /// Parses a Fraction from a string representation of a numeric value.
        /// Only standard ### and ###.## notation is allowed (scientific notation is NOT supported).
        /// Invalid input will result in an exception.
        /// </summary>
        /// <param name="str">Input string to parse the Fraction from.</param>
        /// <returns>Returns the parsed Fraction.</returns>
        public static Fraction Parse(string str) =>
            TryParse(str, out Fraction value) ? value : throw new ArgumentException($"Not a valid number: {str}");

        /// <summary>
        /// Calculates the sum of given values.
        /// </summary>
        /// <param name="values">Values to calculate the sum of.</param>
        /// <returns>Returns the sum of given values.</returns>
        public static Fraction Sum(IEnumerable<Fraction> values)
        {
            Fraction sum = 0;

            foreach (Fraction f in values)
            {
                sum += f;
            }

            return sum;
        }

        /// <summary>
        /// Calculates the mean value of given values.
        /// </summary>
        /// <param name="values">Values to calculate the mean value of.</param>
        /// <returns>Returns the mean value of given values.</returns>
        public static Fraction Mean(IEnumerable<Fraction> values) => Sum(values) / values.Count();

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

        /// <summary>
        /// Inversion operation. Swaps numerator and denominator.
        /// </summary>
        /// <param name="f">Input Fraction.</param>
        /// <returns>Returns inverse Fraction to the input Fraction.</returns>
        public static Fraction operator ~(Fraction f) => new Fraction(f.Denominator, f.Numerator);

        public static Fraction operator +(Fraction first, Fraction second) =>
            new Fraction((first.Numerator * second.Denominator) + (second.Numerator * first.Denominator), first.Denominator * second.Denominator);

        public static Fraction operator -(Fraction first, Fraction second) => first + -second;

        public static Fraction operator *(Fraction first, Fraction second) =>
            new Fraction(first.Numerator * second.Numerator, first.Denominator * second.Denominator);

        public static Fraction operator /(Fraction first, Fraction second) =>
            new Fraction(first.Numerator * second.Denominator, first.Denominator * second.Numerator);

        /// <summary>
        /// Power / exponent operation. Raises base (left-side) Fraction to the exponent (right-side) Fraction.
        /// </summary>
        /// <param name="value">Base value.</param>
        /// <param name="exponent">Exponent value.</param>
        /// <returns>Returns base value raised to exponent value.</returns>
        public static Fraction operator ^(Fraction value, Fraction exponent)
        {
            // Whole number powers can be calculated fairly easily without conversion to double and back.
            if (exponent.IsWholeNumber && exponent.Numerator >= int.MinValue && exponent.Numerator <= int.MaxValue)
            {
                int exp = (int)exponent.Numerator;

                // Positive exponent.
                if (exp > 0)
                {
                    return new Fraction(BigInteger.Pow(value.Numerator, exp), BigInteger.Pow(value.Denominator, exp));
                }
                // Negative exponent.
                else if (exp < 0)
                {
                    return new Fraction(BigInteger.Pow(value.Denominator, -exp), BigInteger.Pow(value.Numerator, -exp));
                }
                // Zero exponent.
                else
                {
                    return Fraction.One;
                }
            }
            // Real number powers are difficult to calculate, therefore the Math.Pow() function is used.
            // This operation is typically very slow.
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

        /// <summary>
        /// Generates a specified power of 10 as BigInteger.
        /// </summary>
        /// <param name="power">Requested power of 10.</param>
        /// <returns>Returns BigInteger equal to specified power of 10.</returns>
        private static BigInteger PowerOf10(int power) =>
            power == 0 ? BigInteger.One : BigInteger.Pow(10, power);
    }
}
