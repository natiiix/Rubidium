namespace Rubidium
{
    public class LiteralExpression : ValueExpression
    {
        public Fraction Value { get; }

        public LiteralExpression(Fraction value)
        {
            Value = value;
        }
    }
}
