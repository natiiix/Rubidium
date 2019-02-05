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
            List<Expression> variableParts = new List<Expression>();

            foreach (Expression expr in expressions)
            {
                if (expr is LiteralExpression literal)
                {
                    coefficient *= literal.Value;
                }
                else if (expr is MultiplicationExpression multiplication)
                {
                    foreach (Expression subExpr in multiplication.Expressions)
                    {
                        if (subExpr is LiteralExpression subLiteral)
                        {
                            coefficient *= subLiteral.Value;
                        }
                        else
                        {
                            variableParts.Add(subExpr);
                        }
                    }
                }
                else
                {
                    variableParts.Add(expr);
                }
            }

            if (coefficient == Fraction.Zero || variableParts.Count == 0)
            {
                return new LiteralExpression(coefficient);
            }
            else if (coefficient != Fraction.One)
            {
                variableParts.Add(new LiteralExpression(coefficient));
            }

            return new MultiplicationExpression(variableParts);
        }
    }
}
