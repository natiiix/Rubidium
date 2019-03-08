using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public class MultiplicationExpression : Expression
    {
        public Fraction Coefficient { get; }
        public List<Expression> VariableParts { get; }

        public bool IsVariableWithCoefficient => VariableParts.Count == 1 && VariableParts[0] is VariableExpression;
        public string VariableName => IsVariableWithCoefficient ? (VariableParts[0] as VariableExpression).Name : null;

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
                return baseCoefficient;
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
                else if (expr is NegatedExpression negation)
                {
                    coefficient = -coefficient;
                    variableParts.Add(negation.Expression);
                }
                else
                {
                    variableParts.Add(expr);
                }
            }

            if (variableParts.Count == 0 || coefficient.IsZero)
            {
                return coefficient;
            }
            else if (variableParts.Count == 1)
            {
                if (coefficient == Fraction.One)
                {
                    return variableParts[0];
                }
                else if (coefficient == Fraction.NegativeOne)
                {
                    return -variableParts[0];
                }
                else if (variableParts[0] is AdditionExpression addition)
                {
                    return addition.Multiply(coefficient);
                }
            }

            return new MultiplicationExpression(coefficient, variableParts);
        }

        public static Expression Build(IEnumerable<Expression> expressions) => Build(Fraction.One, expressions);

        public static Expression Build(params Expression[] expressions) => Build(expressions as IEnumerable<Expression>);

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues, Dictionary<string, Expression> variableExpressions) =>
            Build(Coefficient, VariableParts.Select(x => x.SubstituteVariables(variableValues, variableExpressions)));

        public override Expression FindDerivative() =>
            AdditionExpression.Build(VariableParts.Select(x => x.FindDerivative() * MultiplicationExpression.Build(VariableParts.Where(y => y != x)))) * Coefficient;

        public override string ToString() =>
            IsVariableWithCoefficient ?
            $"{Coefficient}{VariableParts[0]}" :
            "(" + (Coefficient == Fraction.One ? string.Empty : $"{Coefficient} * ") + string.Join(" * ", VariableParts.Select(x => x.ToString())) + ")";
    }
}
