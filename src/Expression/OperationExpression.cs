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

        private OperationExpression(List<Expression> expressions, List<Operation> operations)
        {
            Expressions = expressions;
            Operations = operations;

            Variables = Expressions.SelectMany(x => x.Variables).Distinct();
            ContainsVariables = Variables.Count() > 0;
        }

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues)
        {
            IEnumerable<Expression> expressions = Expressions.Select(x => x.SubstituteVariables(variableValues));

            if (expressions.All(x => x is LiteralExpression))
            {
                LinkedList<Fraction> values = new LinkedList<Fraction>(expressions.Select(x => (x as LiteralExpression).Value));
                LinkedList<Operation> operations = new LinkedList<Operation>(Operations);

                foreach (Operation op in OperationPrecedence)
                {
                    LinkedListNode<Fraction> leftNode = values.First;
                    LinkedListNode<Operation> opNode = operations.First;

                    while (opNode != null)
                    {
                        if (opNode.Value == op)
                        {
                            LinkedListNode<Fraction> rightNode = leftNode.Next;

                            switch (op)
                            {
                                case Operation.Addition:
                                    leftNode.Value += rightNode.Value;
                                    break;

                                case Operation.Subtraction:
                                    leftNode.Value -= rightNode.Value;
                                    break;

                                case Operation.Multiplication:
                                    leftNode.Value *= rightNode.Value;
                                    break;

                                case Operation.Division:
                                    leftNode.Value /= rightNode.Value;
                                    break;

                                case Operation.Power:
                                    leftNode.Value ^= rightNode.Value;
                                    break;

                                default:
                                    throw new Exception("Invalid operation");
                            }

                            LinkedListNode<Operation> nextOpNode = opNode.Next;

                            values.Remove(rightNode);
                            operations.Remove(opNode);

                            opNode = nextOpNode;
                        }
                        else
                        {
                            leftNode = leftNode.Next;
                            opNode = opNode.Next;
                        }
                    }
                }

                if (values.Count != 1 || operations.Count != 0)
                {
                    throw new Exception("Unexpected post-evaluation state");
                }

                return new LiteralExpression(values.First.Value);
            }
            else
            {
                return new OperationExpression(expressions.ToList(), new List<Operation>(Operations));
            }
        }

        public static Expression Build(List<Expression> expressions, List<Operation> operations)
        {
            if (expressions.Count != operations.Count + 1)
            {
                throw new ArgumentException("Number of values must be exactly one more than number of operations");
            }

            if (expressions.Count == 1)
            {
                return expressions[0];
            }

            return new OperationExpression(expressions, operations);
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

            if (coefficient == Fraction.Zero || newExpressions.Count == 0)
            {
                return new LiteralExpression(coefficient);
            }
            else if (coefficient != Fraction.One)
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

            return new OperationExpression(new List<Expression>() { first, second }, GenerateOperations(Operation.Subtraction, 1));
        }

        protected static List<Operation> GenerateOperations(Operation op, int count)
        {
            List<Operation> operations = new List<Operation>(count);

            for (int i = 0; i < count; i++)
            {
                operations.Add(op);
            }

            return operations;
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
