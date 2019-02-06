using System.Collections.Generic;

namespace Rubidium
{
    interface ICanContainVariables
    {
        IEnumerable<string> Variables { get; }
        bool ContainsVariables { get; }
    }
}
