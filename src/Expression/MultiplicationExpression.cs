using System;
using System.Collections.Generic;

namespace Rubidium
{
    public class MultiplicationExpression : OperationExpression
    {
        private MultiplicationExpression(List<Expression> expressions) : base(expressions, Operation.Multiplication) { }

        public static Expression Build(List<Expression> expressions)
        {
            if (expressions.Count == 0)
            {
                throw new Exception("Unable to make multiplication expression from zero expressions");
            }
            else if (expressions.Count == 1)
            {
                return expressions[0];
            }

            Fraction coefficient = Fraction.One;
            List<Expression> newExpressions = new List<Expression>();

            for (int i = 0; i < expressions.Count; i++)
            {
                Expression expr = expressions[i];

                if (expr is LiteralExpression literal)
                {
                    coefficient *= literal.Value;
                }
                else
                {
                    newExpressions.Add(expr);
                }
            }

            if (coefficient == Fraction.Zero || newExpressions.Count == 0)
            {
                return new LiteralExpression(coefficient);
            }
            else if (coefficient != Fraction.One)
            {
                newExpressions.Add(new LiteralExpression(coefficient));
            }

            return new MultiplicationExpression(newExpressions);
        }
    }
}
