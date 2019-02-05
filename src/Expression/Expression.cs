using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public abstract class Expression : ICanContainVariables
    {
        public abstract IEnumerable<string> Variables { get; }
        public bool ContainsVariables => Variables.Count() > 0;

        public abstract Expression SubstituteVariables(Dictionary<string, Fraction> variableValues);

        public static Expression operator -(Expression expr) =>
            NegatedExpression.Build(expr);

        public static Expression operator +(Expression first, Expression second) =>
            AdditionExpression.Build(first, second);

        public static Expression operator -(Expression first, Expression second) =>
            AdditionExpression.Build(first, -second);

        public static Expression operator *(Expression first, Expression second) =>
            MultiplicationExpression.Build(first, second);

        public static Expression operator /(Expression expr, Fraction value) =>
            expr * new LiteralExpression(~value);
    }
}
