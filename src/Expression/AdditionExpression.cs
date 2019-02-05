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
                return new ConstantExpression(baseConstant);
            }

            Fraction constantPart = baseConstant;
            List<Expression> variableParts = new List<Expression>();

            foreach (Expression expr in expressions)
            {
                if (expr is ConstantExpression constant)
                {
                    constantPart += constant;
                }
                else if (expr is AdditionExpression addition)
                {
                    constantPart += addition.Constant;
                    variableParts.AddRange(addition.VariableParts);
                }
                else
                {
                    variableParts.Add(expr);
                }
            }

            if (variableParts.Count == 0)
            {
                return new ConstantExpression(constantPart);
            }
            else if (constantPart.IsZero && variableParts.Count == 1)
            {
                return variableParts[0];
            }

            return new AdditionExpression(constantPart, variableParts);
        }

        public static Expression Build(IEnumerable<Expression> expressions) => Build(Fraction.Zero, expressions);

        public static Expression Build(params Expression[] expressions) => Build(expressions as IEnumerable<Expression>);

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues) =>
            Build(Constant, VariableParts.Select(x => x.SubstituteVariables(variableValues)));

        public override string ToString() =>
            "(" + (Constant.IsZero ? string.Empty : $"{Constant} + ") + string.Join(" + ", VariableParts.Select(x => x.ToString())) + ")";
    }
}
