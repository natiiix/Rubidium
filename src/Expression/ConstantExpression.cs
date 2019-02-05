using System.Collections.Generic;

namespace Rubidium
{
    public class ConstantExpression : Expression
    {
        public static ConstantExpression Zero => new ConstantExpression(0);

        public Fraction Value { get; }

        public override IEnumerable<string> Variables { get; } = new string[0];

        public ConstantExpression(Fraction value)
        {
            Value = value;
        }

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues) => this;

        public override string ToString() => Value.ToString();
    }
}
