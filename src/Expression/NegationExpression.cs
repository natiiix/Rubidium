namespace Rubidium
{
    public class NegationExpression : ValueExpression
    {
        public ValueExpression Expression { get; }

        public NegationExpression(ValueExpression expr)
        {
            Expression = expr;
        }
    }
}
