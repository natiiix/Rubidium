using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public class OperationExpression : Expression
    {
        private static readonly Operation[] OperationPrecedence =
        {
            Operation.Power,
            Operation.Division,
            Operation.Multiplication,
            Operation.Subtraction,
            Operation.Addition
        };

        public List<Expression> Expressions { get; }
        public List<Operation> Operations { get; }

        public override IEnumerable<string> Variables { get; }
        public override bool ContainsVariables { get; }

        public OperationExpression(List<Expression> expressions, List<Operation> operations)
        {
            Expressions = expressions;
            Operations = operations;

            if (expressions.Count != operations.Count + 1)
            {
                throw new ArgumentException("Number of values must be exactly one more than number of operations");
            }

            Variables = Expressions.SelectMany(x => x.Variables).Distinct();
            ContainsVariables = Variables.Count() > 0;
        }

        public static Expression Multiply(List<Expression> expressions)
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

            if (coefficient == Fraction.Zero)
            {
                return LiteralExpression.Zero;
            }
            else if (coefficient != Fraction.One || newExpressions.Count == 0)
            {
                newExpressions.Add(new LiteralExpression(coefficient));
            }

            List<Operation> operations = new List<Operation>();

            for (int i = 0; i < newExpressions.Count - 1; i++)
            {
                operations.Add(Operation.Multiplication);
            }

            return new OperationExpression(newExpressions, operations);
        }

        public static Expression Subtract(Expression first, Expression second)
        {
            if (first is LiteralExpression firstLiteral && second is LiteralExpression secondLiteral)
            {
                return new LiteralExpression(firstLiteral.Value - secondLiteral.Value);
            }

            return new OperationExpression(new List<Expression>() { first, second }, new List<Operation>() { Operation.Subtraction });
        }

        public override string ToString()
        {
            string str = string.Empty;

            for (int i = 0; i < Expressions.Count; i++)
            {
                str += Expressions[i].ToString();

                if (i < Expressions.Count - 1)
                {
                    Operation op = Operations[i];

                    switch (op)
                    {
                        case Operation.Addition:
                            str += " + ";
                            break;

                        case Operation.Subtraction:
                            str += " - ";
                            break;

                        case Operation.Multiplication:
                            str += " * ";
                            break;

                        case Operation.Division:
                            str += " / ";
                            break;

                        case Operation.Power:
                            str += "^";
                            break;

                        default:
                            throw new Exception();
                    }
                }
            }

            return $"( {str} )";
        }
    }
}
