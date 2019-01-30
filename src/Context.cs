using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public class Context
    {
        private List<Statement> Statements { get; }
        private HashSet<string> Variables { get; }
        private Dictionary<string, Fraction> BoundVariables { get; }

        public Context(List<Statement> initialStatements)
        {
            Statements = new List<Statement>(initialStatements);

            Variables = new HashSet<string>();

            foreach (Statement s in initialStatements)
            {
                foreach (string v in s.Left.Variables)
                {
                    Variables.Add(v);
                }

                foreach (string v in s.Right.Variables)
                {
                    Variables.Add(v);
                }
            }

            BoundVariables = new Dictionary<string, Fraction>();
        }

        public bool FindNewStatements()
        {
            List<Statement> keepStatements = new List<Statement>();
            List<Statement> newStatements = new List<Statement>();

            foreach (Statement s in Statements)
            {
                Console.WriteLine(s);

                if (s.Right.ContainsVariables)
                {
                    if (!s.Left.ContainsVariables)
                    {
                        newStatements.Add(new Statement(s.Right, s.Left));
                    }
                    else
                    {
                        newStatements.Add(new Statement(OperationExpression.Subtract(s.Left, s.Right), LiteralExpression.Zero));
                    }
                }
                else if (!s.Left.ContainsVariables)
                {
                    Console.WriteLine($"{s} : {OperationExpression.Subtract(s.Left, s.Right) is LiteralExpression literal && literal.Value == 0}");
                }
                else
                {
                    keepStatements.Add(s);
                }
            }

            Statements.Clear();
            Statements.AddRange(keepStatements);
            Statements.AddRange(newStatements);

            return newStatements.Count > 0;
        }
    }
}
