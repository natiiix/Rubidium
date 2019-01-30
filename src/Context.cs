using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public class Context
    {
        private List<Statement> Statements { get; }
        private string[] Variables { get; }
        private Dictionary<string, Fraction> BoundVariables { get; }

        public Context(List<Statement> initialStatements)
        {
            Statements = new List<Statement>(initialStatements);
            Variables = Statements.SelectMany(x => x.Variables).Distinct().ToArray();
            BoundVariables = new Dictionary<string, Fraction>();
        }

        public bool FindNewStatements()
        {
            return false;
        }
    }
}
