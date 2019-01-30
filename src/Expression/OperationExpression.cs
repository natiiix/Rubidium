using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public class OperationExpression : Expression
    {
        public override bool IsBound => Expressions.All(x => x.IsBound);

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

        public OperationExpression(List<Expression> expressions, List<Operation> operations)
        {
            Expressions = expressions;
            Operations = operations;

            if (expressions.Count != operations.Count + 1)
            {
                throw new ArgumentException("Number of values must be exactly one more than number of operations");
            }
        }

        public static OperationExpression CreateMultiplication(List<Expression> expressions)
        {
            if (expressions.Count == 0)
            {
                throw new Exception("Unable to make multiplication expression from zero expressions");
            }

            List<Operation> operations = new List<Operation>();

            for (int i = 0; i < expressions.Count - 1; i++)
            {
                operations.Add(Operation.Multiplication);
            }

            return new OperationExpression(new List<Expression>(expressions), operations);
        }

        // public override Fraction Evaluate(Dictionary<string, Fraction> variables)
        // {
        //     LinkedList<Fraction> values = new LinkedList<Fraction>(Values.Select(x => x.Evaluate(variables)));
        //     LinkedList<Operation> operations = new LinkedList<Operation>(Operations);

        //     foreach (Operation op in OperationPrecedence)
        //     {
        //         LinkedListNode<Fraction> leftNode = values.First;
        //         LinkedListNode<Operation> opNode = operations.First;

        //         while (opNode != null)
        //         {
        //             if (opNode.Value == op)
        //             {
        //                 LinkedListNode<Fraction> rightNode = leftNode.Next;

        //                 switch (op)
        //                 {
        //                     case Operation.Addition:
        //                         leftNode.Value += rightNode.Value;
        //                         break;

        //                     case Operation.Subtraction:
        //                         leftNode.Value -= rightNode.Value;
        //                         break;

        //                     case Operation.Multiplication:
        //                         leftNode.Value *= rightNode.Value;
        //                         break;

        //                     case Operation.Division:
        //                         leftNode.Value /= rightNode.Value;
        //                         break;

        //                     case Operation.Power:
        //                         leftNode.Value ^= rightNode.Value;
        //                         break;

        //                     default:
        //                         throw new Exception("Invalid operation");
        //                 }

        //                 LinkedListNode<Operation> nextOpNode = opNode.Next;

        //                 values.Remove(rightNode);
        //                 operations.Remove(opNode);

        //                 opNode = nextOpNode;
        //             }
        //             else
        //             {
        //                 leftNode = leftNode.Next;
        //                 opNode = opNode.Next;
        //             }
        //         }
        //     }

        //     if (values.Count != 1 || operations.Count != 0)
        //     {
        //         throw new Exception("Unexpected post-evaluation state");
        //     }

        //     return values.First.Value;
        // }

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

            return str;
        }
    }
}
