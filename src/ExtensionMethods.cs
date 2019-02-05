using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public static class ExtensionMethods
    {
        public static IEnumerable<string> GetVariables(this IEnumerable<Expression> expressions) =>
            expressions.SelectMany(x => x.Variables).Distinct();
    }
}
