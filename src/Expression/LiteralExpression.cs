using System.Collections.Generic;

namespace Rubidium
{
    public class LiteralExpression : ValueExpression
    {
        public Fraction Value { get; }

        public LiteralExpression(Fraction value)
        {
            Value = value;
        }

        public override Fraction Evaluate(Dictionary<string, Fraction> variables) => Value;
    }
}
