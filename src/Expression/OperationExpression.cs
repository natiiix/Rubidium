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

        public override IEnumerable<string> Variables => Expressions.SelectMany(x => x.Variables).Distinct();

        public OperationExpression(List<Expression> expressions, List<Operation> operations)
        {
            Expressions = expressions;
            Operations = operations;

            if (expressions.Count != operations.Count + 1)
            {
                throw new ArgumentException("Number of values must be exactly one more than number of operations");
            }
        }

        public static Expression Multiply(List<Expression> expressions)
        {
            if (expressions.Count == 0)
            {
                throw new Exception("Unable to make multiplication expression from zero expressions");
            }

            if (expressions.Count == 1)
            {
                return expressions[0];
            }

            List<Operation> operations = new List<Operation>();

            for (int i = 0; i < expressions.Count - 1; i++)
            {
                operations.Add(Operation.Multiplication);
            }

            return new OperationExpression(new List<Expression>(expressions), operations);
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
