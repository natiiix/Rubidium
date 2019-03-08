using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public class AdditionExpression : Expression
    {
        public Fraction Constant { get; }
        public List<Expression> VariableParts { get; }

        public override IEnumerable<string> Variables { get; }

        private AdditionExpression(Fraction constant, List<Expression> variableParts)
        {
            Constant = constant;
            VariableParts = variableParts;

            Variables = variableParts.GetVariables();
        }

        public static Expression Build(Fraction baseConstant, IEnumerable<Expression> expressions)
        {
            if (expressions.Count() == 0)
            {
                return baseConstant;
            }

            Fraction constantPart = baseConstant;
            List<Expression> variableParts = new List<Expression>();
            Dictionary<string, Fraction> variableCoefficients = new Dictionary<string, Fraction>();

            if (expressions.Any(x => x is AdditionExpression))
            {
                foreach (Expression expr in expressions)
                {
                    if (expr is AdditionExpression addition)
                    {
                        constantPart += addition.Constant;
                        variableParts.AddRange(addition.VariableParts);
                    }
                    else
                    {
                        variableParts.Add(expr);
                    }
                }

                return Build(constantPart, variableParts);
            }

            foreach (Expression expr in expressions)
            {
                if (expr is ConstantExpression constant)
                {
                    constantPart += constant;
                }
                else if (expr is VariableExpression variable)
                {
                    variableCoefficients[variable.Name] = (variableCoefficients.GetValueOrDefault(variable.Name) ?? Fraction.Zero) + Fraction.One;
                }
                else if (expr is NegatedExpression negated && negated.Expression is VariableExpression negatedVariable)
                {
                    variableCoefficients[negatedVariable.Name] = (variableCoefficients.GetValueOrDefault(negatedVariable.Name) ?? Fraction.Zero) + Fraction.NegativeOne;
                }
                else if (expr is MultiplicationExpression multiplication && multiplication.IsVariableWithCoefficient)
                {
                    variableCoefficients[multiplication.VariableName] = (variableCoefficients.GetValueOrDefault(multiplication.VariableName) ?? Fraction.Zero) + multiplication.Coefficient;
                }
                else
                {
                    variableParts.Add(expr);
                }
            }

            foreach (var coeff in variableCoefficients)
            {
                variableParts.Add(coeff.Value * new VariableExpression(coeff.Key));
            }

            if (variableParts.Count == 0)
            {
                return constantPart;
            }
            else if (constantPart.IsZero && variableParts.Count == 1)
            {
                return variableParts[0];
            }

            return new AdditionExpression(constantPart, variableParts);
        }

        public Expression Multiply(Fraction factor)
        {
            if (factor.IsZero)
            {
                return ConstantExpression.Zero;
            }
            else if (factor == Fraction.One)
            {
                return this;
            }
            else if (factor == Fraction.NegativeOne)
            {
                return -this;
            }
            else
            {
                return Build(Constant * factor, VariableParts.Select(x => x * factor));
            }
        }

        public static Expression Build(IEnumerable<Expression> expressions) => Build(Fraction.Zero, expressions);

        public static Expression Build(params Expression[] expressions) => Build(expressions as IEnumerable<Expression>);

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues, Dictionary<string, Expression> variableExpressions) =>
            Build(Constant, VariableParts.Select(x => x.SubstituteVariables(variableValues, variableExpressions)));

        public override Expression FindDerivative() => Build(VariableParts.Select(x => x.FindDerivative()));

        public override string ToString() =>
            "(" + (Constant.IsZero ? string.Empty : $"{Constant} + ") + string.Join(" + ", VariableParts.Select(x => x.ToString())) + ")";
    }
}
