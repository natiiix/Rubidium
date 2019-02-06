using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public static class ExtensionMethods
    {
        internal static IEnumerable<string> GetVariables(this IEnumerable<ICanContainVariables> objects) =>
            objects.SelectMany(x => x.Variables).Distinct();
    }
}
