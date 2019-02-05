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

        public static Expression Build(Fraction baseCoefficient, IEnumerable<Expression> expressions)
        {
            if (expressions.Count() == 0)
            {
                return new ConstantExpression(baseCoefficient);
            }

            Fraction coefficient = baseCoefficient;
            List<Expression> variableParts = new List<Expression>();

            foreach (Expression expr in expressions)
            {
                if (expr is ConstantExpression constant)
                {
                    coefficient *= constant;
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
                return new ConstantExpression(coefficient);
            }
            else if (coefficient == Fraction.One && variableParts.Count == 1)
            {
                return variableParts[0];
            }

            return new MultiplicationExpression(coefficient, variableParts);
        }

        public static Expression Build(IEnumerable<Expression> expressions) => Build(Fraction.One, expressions);

        public static Expression Build(params Expression[] expressions) => Build(expressions as IEnumerable<Expression>);

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues) =>
            Build(Coefficient, VariableParts.Select(x => x.SubstituteVariables(variableValues)));

        public override string ToString() =>
            $"( {Coefficient} * {string.Join(" * ", VariableParts.Select(x => x.ToString()))} )";
    }
}
