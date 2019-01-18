using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public class OperationExpression : ValueExpression
    {
        private static readonly Operation[] OperationPrecedence =
        {
            Operation.Power,
            Operation.Division,
            Operation.Multiplication,
            Operation.Subtraction,
            Operation.Addition
        };

        public List<ValueExpression> Values { get; }
        public List<Operation> Operations { get; }

        public OperationExpression(List<ValueExpression> values, List<Operation> operations)
        {
            Values = values;
            Operations = operations;

            if (values.Count != operations.Count + 1)
            {
                throw new ArgumentException("Number of values must be exactly one more than number of operations");
            }
        }

        public override Fraction Evaluate(Dictionary<string, Fraction> variables)
        {
            LinkedList<Fraction> values = new LinkedList<Fraction>(Values.Select(x => x.Evaluate(variables)));
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

                        values.Remove(rightNode);
                        operations.Remove(opNode);
                    }
                }
            }

            if (values.Count != 1 || operations.Count != 0)
            {
                throw new Exception("Unexpected post-evaluation state");
            }

            return values.First.Value;
        }
    }
}
