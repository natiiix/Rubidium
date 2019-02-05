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

        public static Expression Build(Fraction baseConstant, List<Expression> expressions)
        {
            if (expressions.Count == 0)
            {
                return new LiteralExpression(baseConstant);
            }

            Fraction constantPart = baseConstant;
            List<Expression> variableParts = new List<Expression>();

            foreach (Expression expr in expressions)
            {
                if (expr is LiteralExpression literal)
                {
                    constantPart += literal.Value;
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
                return new LiteralExpression(constantPart);
            }
            else if (constantPart == Fraction.Zero && variableParts.Count == 1)
            {
                return variableParts[0];
            }

            return new AdditionExpression(constantPart, variableParts);
        }

        public static Expression Build(List<Expression> expressions) => Build(Fraction.Zero, expressions);

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues) =>
            Build(Constant, VariableParts.Select(x => x.SubstituteVariables(variableValues)).ToList());

        public override string ToString() =>
            $"( {Constant} + {string.Join(" + ", VariableParts.Select(x => x.ToString()))} )";
    }
}
