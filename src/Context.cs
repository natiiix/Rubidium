using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public class Context
    {
        private List<Statement> Statements { get; }
        private Dictionary<string, Fraction> VariableValues { get; }

        public Context(List<Statement> initialStatements)
        {
            Statements = new List<Statement>(initialStatements);
            VariableValues = new Dictionary<string, Fraction>();
        }

        public bool FindNewStatements()
        {
            int newVariablesBound = 0;
            List<Statement> keepStatements = new List<Statement>();
            List<Statement> newStatements = new List<Statement>();

            foreach (Statement s in Statements)
            {
                Console.WriteLine(s);

                if (s.Variables.Any(x => VariableValues.ContainsKey(x)))
                {
                    newStatements.Add(s.SubstituteVariables(VariableValues));
                }
                else if (s.Left is VariableExpression leftVariable)
                {
                    if (s.Right is ConstantExpression rightConst)
                    {
                        if (VariableValues.ContainsKey(leftVariable.Name))
                        {
                            throw new Exception("Variable is already bound to a value");
                        }

                        VariableValues[leftVariable.Name] = rightConst;
                        newVariablesBound++;
                    }
                    else
                    {
                        keepStatements.Add(s);
                    }
                }
                else if (s.Right is VariableExpression rightVariable)
                {
                    newStatements.Add(new Statement(s.Right, s.Left));
                }
                else if (s.Left is MultiplicationExpression leftMultiplication &&
                    leftMultiplication.VariableParts.Count == 1 &&
                    leftMultiplication.VariableParts[0] is VariableExpression)
                {
                    newStatements.Add(new Statement(
                        leftMultiplication.VariableParts[0],
                        s.Right / new ConstantExpression(leftMultiplication.Coefficient)
                    ));
                }
                else if (s.Left is AdditionExpression leftAddition &&
                    (s.Right is AdditionExpression || !leftAddition.Constant.IsZero))
                {
                    if (s.Right is AdditionExpression rightAddition)
                    {
                        Fraction constant = rightAddition.Constant - leftAddition.Constant;
                        List<Expression> variableParts = new List<Expression>();

                        variableParts.AddRange(leftAddition.VariableParts);
                        variableParts.AddRange(rightAddition.VariableParts.Select(x => -x));

                        newStatements.Add(new Statement(
                            AdditionExpression.Build(variableParts),
                            new ConstantExpression(constant)
                        ));
                    }
                    else
                    {
                        newStatements.Add(new Statement(
                            AdditionExpression.Build(leftAddition.VariableParts),
                            s.Right + new ConstantExpression(-leftAddition.Constant)
                        ));
                    }
                }
                else if (s.Right.ContainsVariables)
                {
                    newStatements.Add(s.Left.ContainsVariables ?
                        new Statement(s.Left - s.Right, ConstantExpression.Zero) :
                        new Statement(s.Right, s.Left)
                    );
                }
                else if (!s.Left.ContainsVariables && !s.Right.ContainsVariables)
                {
                    Console.WriteLine($"{s} : {(s.Left - s.Right) is ConstantExpression constant && constant.Value == 0}");
                }
                else
                {
                    keepStatements.Add(s);
                }
            }

            Console.WriteLine("--------------------------------");

            Statements.Clear();
            Statements.AddRange(keepStatements);
            Statements.AddRange(newStatements);

            return (newStatements.Count > 0 || (newVariablesBound > 0 && keepStatements.Count > 0));
        }

        public override string ToString()
        {
            string str = string.Empty;

            if (Statements.Count > 0)
            {
                str += "Statements:" + Environment.NewLine +
                    string.Join(Environment.NewLine, Statements);
            }

            if (VariableValues.Count > 0)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    str += Environment.NewLine;
                }

                str += "Variables:" + Environment.NewLine +
                    string.Join(Environment.NewLine, VariableValues.Select(x => $"{x.Key} = {x.Value}"));
            }

            return str;
        }
    }
}
