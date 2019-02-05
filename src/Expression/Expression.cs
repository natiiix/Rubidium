using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public abstract class Expression : ICanContainVariables
    {
        public abstract IEnumerable<string> Variables { get; }
        public bool ContainsVariables => Variables.Count() > 0;

        public abstract Expression SubstituteVariables(Dictionary<string, Fraction> variableValues);
    }
}
