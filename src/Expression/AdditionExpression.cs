using System;
using System.Collections.Generic;

namespace Rubidium
{
    public class AdditionExpression : OperationExpression
    {
        private AdditionExpression(List<Expression> expressions) : base(expressions, Operation.Addition) { }

        public static Expression Build(List<Expression> expressions)
        {
            if (expressions.Count == 0)
            {
                throw new Exception("Unable to make addition expression from zero expressions");
            }
            else if (expressions.Count == 1)
            {
                return expressions[0];
            }

            Fraction literalPart = Fraction.Zero;
            List<Expression> variableParts = new List<Expression>();

            for (int i = 0; i < expressions.Count; i++)
            {
                Expression expr = expressions[i];

                if (expr is LiteralExpression literal)
                {
                    literalPart += literal.Value;
                }
                else
                {
                    variableParts.Add(expr);
                }
            }

            if (variableParts.Count == 0)
            {
                return new LiteralExpression(literalPart);
            }
            else if (literalPart != Fraction.Zero)
            {
                variableParts.Add(new LiteralExpression(literalPart));
            }

            return new AdditionExpression(variableParts);
        }
    }
}
