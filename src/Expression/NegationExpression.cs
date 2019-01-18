using System.Collections.Generic;

namespace Rubidium
{
    public class NegationExpression : ValueExpression
    {
        public ValueExpression Expression { get; }

        public NegationExpression(ValueExpression expr)
        {
            Expression = expr;
        }

        public override Fraction Evaluate(Dictionary<string, Fraction> variables) => Expression.Evaluate(variables); // TODO
    }
}
