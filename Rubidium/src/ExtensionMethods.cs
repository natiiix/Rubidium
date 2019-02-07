using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public static class ExtensionMethods
    {
        internal static IEnumerable<string> GetVariables(this IEnumerable<ICanContainVariables> objects) =>
            objects.SelectMany(x => x.Variables).Distinct();

        internal static string StripParentheses(this string str) =>
            str.StartsWith('(') && str.EndsWith(')') ? StripParentheses(str.Substring(1, str.Length - 2)) : str;
    }
}
