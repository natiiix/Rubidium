using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    /// <summary>
    /// Wrapper class for extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Extracts names of variables from an enumerable of objects.
        /// </summary>
        /// <param name="objects">Input enumerable of objects, which can contain variables.</param>
        /// <returns>Returns an enumerable of distinct variable names contained in the objects.</returns>
        internal static IEnumerable<string> GetVariables(this IEnumerable<ICanContainVariables> objects) =>
            objects.SelectMany(x => x.Variables).Distinct();

        /// <summary>
        /// Strips parentheses from the input string and returns the result.
        /// If a string begins with left parenthesis and ends with right parenthesis,
        /// the inner string will be returned.
        /// </summary>
        /// <param name="str">Input string.</param>
        /// <returns>Returns inner string without surrounding parentheses.</returns>
        internal static string StripParentheses(this string str) =>
            str.StartsWith('(') && str.EndsWith(')') ? StripParentheses(str.Substring(1, str.Length - 2)) : str;
    }
}
