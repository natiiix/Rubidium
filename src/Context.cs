using System.Collections.Generic;

namespace Rubidium
{
    public class Context
    {
        private List<Statement> Statements { get; }

        public Context(List<Statement> initialStatements)
        {
            Statements = new List<Statement>(initialStatements);
        }

        public bool FindNewStatements()
        {
            return false;
        }
    }
}
