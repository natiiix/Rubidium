using System.Collections.Generic;

namespace Rubidium
{
    public class ConstantExpression : Expression
    {
        public static ConstantExpression Zero => new ConstantExpression(Fraction.Zero);
        public static ConstantExpression One => new ConstantExpression(Fraction.One);

        public Fraction Value { get; }

        public override IEnumerable<string> Variables { get; } = new string[0];

        public ConstantExpression(Fraction value)
        {
            Value = value;
        }

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues) => this;

        public override string ToString() => Value.ToString();

        public static implicit operator ConstantExpression(Fraction f) => new ConstantExpression(f);
        public static implicit operator Fraction(ConstantExpression c) => c.Value;
    }
}
