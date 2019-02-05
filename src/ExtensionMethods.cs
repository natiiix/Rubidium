using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public static class ExtensionMethods
    {
        public static IEnumerable<string> GetVariables(this List<Expression> expressions) =>
            expressions.SelectMany(x => x.Variables).Distinct();
    }
}
