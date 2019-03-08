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
                return -constant.Value;
            }
            else if (expr is AdditionExpression addition)
            {
                return AdditionExpression.Build(-addition.Constant, addition.VariableParts.Select(x => NegatedExpression.Build(x)));
            }
            else if (expr is MultiplicationExpression multiplication)
            {
                return MultiplicationExpression.Build(-multiplication.Coefficient, multiplication.VariableParts);
            }
            else
            {
                return new NegatedExpression(expr);
            }
        }

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues, Dictionary<string, Expression> variableExpressions) =>
            Build(Expression.SubstituteVariables(variableValues, variableExpressions));

        public override Expression FindDerivative() => -Expression.FindDerivative();

        public override string ToString() => $"-{Expression}";
    }
}
