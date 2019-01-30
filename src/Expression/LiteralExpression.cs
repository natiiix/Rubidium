using System.Collections.Generic;

namespace Rubidium
{
    public class LiteralExpression : Expression
    {
        public Fraction Value { get; }

        public override IEnumerable<string> Variables { get; } = new string[0];
        public override bool ContainsVariables { get; } = false;

        public LiteralExpression(Fraction value)
        {
            Value = value;
        }

        public override string ToString() => Value.ToString();
    }
}
