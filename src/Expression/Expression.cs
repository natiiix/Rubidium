using System.Collections.Generic;

namespace Rubidium
{
    public abstract class Expression
    {
        public abstract IEnumerable<string> Variables { get; }
        public abstract bool ContainsVariables { get; }
    }
}
