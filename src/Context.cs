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
            return false;
        }
    }
}
