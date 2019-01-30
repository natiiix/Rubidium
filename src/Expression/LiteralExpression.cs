using System.Collections.Generic;

namespace Rubidium
{
    public class LiteralExpression : Expression
    {
        public override bool IsBound => true;

        public Fraction Value { get; }

        public LiteralExpression(Fraction value)
        {
            Value = value;
        }

        public override string ToString() => Value.ToString();
    }
}
