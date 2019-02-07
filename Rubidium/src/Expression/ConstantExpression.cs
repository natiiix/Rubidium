using System.Collections.Generic;

namespace Rubidium
{
    public class ConstantExpression : Expression
    {
        public static ConstantExpression Zero => Fraction.Zero;
        public static ConstantExpression One => Fraction.One;

        public Fraction Value { get; }

        public override IEnumerable<string> Variables { get; } = new string[0];

        public ConstantExpression(Fraction value)
        {
            Value = value;
        }

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues, Dictionary<string, Expression> variableExpressions) => this;

        public override string ToString() => Value.ToString();
    }
}
