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

        protected OperationExpression(List<Expression> expressions, List<Operation> operations)
        {
            Expressions = expressions;
            Operations = operations;

            Variables = Expressions.SelectMany(x => x.Variables).Distinct();
            ContainsVariables = Variables.Count() > 0;
        }

        protected OperationExpression(List<Expression> expressions, Operation op) :
            this(expressions, GenerateOperations(op, expressions.Count - 1))
        { }

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues) =>
            Build(Expressions.Select(x => x.SubstituteVariables(variableValues)).ToList(), Operations);

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

            if (expressions.All(x => x is LiteralExpression))
            {
                return Evaluate(expressions.Select(x => x as LiteralExpression), operations);
            }
            else if (operations.All(x => x == Operation.Addition))
            {
                return AdditionExpression.Build(expressions);
            }
            else if (operations.All(x => x == Operation.Addition || x == Operation.Subtraction))
            {
                List<Expression> newExpressions = new List<Expression>(expressions);

                for (int i = 0; i < operations.Count; i++)
                {
                    if (operations[i] == Operation.Subtraction)
                    {
                        int exprIndex = i + 1;
                        newExpressions[exprIndex] = NegatedExpression.Build(newExpressions[exprIndex]);
                    }
                }

                return AdditionExpression.Build(newExpressions);
            }
            else if (operations.All(x => x == Operation.Multiplication))
            {
                return MultiplicationExpression.Build(expressions);
            }

            return new OperationExpression(expressions, operations);
        }

        public static Expression Subtract(Expression first, Expression second) =>
            AdditionExpression.Build(new List<Expression>() { first, NegatedExpression.Build(second) });

        private static LiteralExpression Evaluate(IEnumerable<LiteralExpression> expressions, IEnumerable<Operation> operations)
        {
            LinkedList<Fraction> values = new LinkedList<Fraction>(expressions.Select(x => x.Value));
            LinkedList<Operation> opList = new LinkedList<Operation>(operations);

            foreach (Operation op in OperationPrecedence)
            {
                LinkedListNode<Fraction> leftNode = values.First;
                LinkedListNode<Operation> opNode = opList.First;

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
                        opList.Remove(opNode);

                        opNode = nextOpNode;
                    }
                    else
                    {
                        leftNode = leftNode.Next;
                        opNode = opNode.Next;
                    }
                }
            }

            if (values.Count != 1 || opList.Count != 0)
            {
                throw new Exception("Unexpected post-evaluation state");
            }

            return new LiteralExpression(values.First.Value);
        }

        private static List<Operation> GenerateOperations(Operation op, int count)
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
