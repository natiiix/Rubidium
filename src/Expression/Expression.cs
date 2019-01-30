using System.Collections.Generic;

namespace Rubidium
{
    public abstract class Expression : ICanContainVariables
    {
        public abstract IEnumerable<string> Variables { get; }
        public abstract bool ContainsVariables { get; }

        public abstract Expression SubstituteVariables(Dictionary<string, Fraction> variableValues);
    }
}
