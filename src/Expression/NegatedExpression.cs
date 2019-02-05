using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public class NegatedExpression : Expression
    {
        public override IEnumerable<string> Variables => Expression.Variables;

        public Expression Expression { get; }

        private NegatedExpression(Expression expr)
        {
            Expression = expr;
        }

        public static Expression Build(Expression expr)
        {
            if (expr is NegatedExpression negated)
            {
                return negated.Expression;
            }
            else if (expr is ConstantExpression constant)
            {
                return new ConstantExpression(-constant.Value);
            }
            else if (expr is AdditionExpression addition)
            {
                return AdditionExpression.Build(-addition.Constant, addition.VariableParts.Select(x => NegatedExpression.Build(x)));
            }
            else
            {
                return new NegatedExpression(expr);
            }
        }

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues) =>
            Build(Expression.SubstituteVariables(variableValues));

        public override string ToString() => $"-({Expression})";
    }
}
