using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public class MultiplicationExpression : Expression
    {
        public Fraction Coefficient { get; }
        public List<Expression> VariableParts { get; }

        public override IEnumerable<string> Variables { get; }

        private MultiplicationExpression(Fraction coefficient, List<Expression> variableParts)
        {
            Coefficient = coefficient;
            VariableParts = variableParts;

            Variables = variableParts.GetVariables();
        }

        public static Expression Build(Fraction baseCoefficient, List<Expression> expressions)
        {
            if (expressions.Count == 0)
            {
                return new LiteralExpression(baseCoefficient);
            }

            Fraction coefficient = baseCoefficient;
            List<Expression> variableParts = new List<Expression>();

            foreach (Expression expr in expressions)
            {
                if (expr is LiteralExpression literal)
                {
                    coefficient *= literal.Value;
                }
                else if (expr is MultiplicationExpression multiplication)
                {
                    coefficient *= multiplication.Coefficient;
                    variableParts.AddRange(multiplication.VariableParts);
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
            else if (coefficient == Fraction.One && variableParts.Count == 1)
            {
                return variableParts[0];
            }

            return new MultiplicationExpression(coefficient, variableParts);
        }

        public static Expression Build(List<Expression> expressions) => Build(Fraction.One, expressions);

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues) =>
            Build(Coefficient, VariableParts.Select(x => x.SubstituteVariables(variableValues)).ToList());

        public override string ToString() =>
            $"( {Coefficient} * {string.Join(" * ", VariableParts.Select(x => x.ToString()))} )";
    }
}
