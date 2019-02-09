using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    /// <summary>
    /// Abstract class encapsulating all type of expressions.
    /// </summary>
    public abstract class Expression : ICanContainVariables
    {
        /// <summary>
        /// Enumerable of variables contained (references) in the expression.
        /// </summary>
        public abstract IEnumerable<string> Variables { get; }
        /// <summary>
        /// Boolean value indicating if the expression contains any variable.
        /// </summary>
        public bool ContainsVariables => Variables.Count() > 0;

        /// <summary>
        /// Substitues available variables with their respective values or expressions.
        /// </summary>
        /// <param name="variableValues">Available variable values.</param>
        /// <param name="variableExpressions">Available variable expressions.</param>
        /// <returns>Returns a version of the expression with available variables replaced.</returns>
        public abstract Expression SubstituteVariables(Dictionary<string, Fraction> variableValues, Dictionary<string, Expression> variableExpressions);

        /// <summary>
        /// Unary subtraction (negation) operation of an expression.
        /// </summary>
        /// <param name="expr">Expression to be negated.</param>
        /// <returns>Returns a negated expression of the input expression.</returns>
        public static Expression operator -(Expression expr) =>
            NegatedExpression.Build(expr);

        /// <summary>
        /// Binary addition of expressions.
        /// </summary>
        /// <param name="first">Left-side expression.</param>
        /// <param name="second">Right-side expression.</param>
        /// <returns>Returns the result of adding the operand expressions.</returns>
        public static Expression operator +(Expression first, Expression second) =>
            AdditionExpression.Build(first, second);

        /// <summary>
        /// Binary subtraction of expressions.
        /// </summary>
        /// <param name="first">Left-side expression.</param>
        /// <param name="second">Right-side expression.</param>
        /// <returns>Returns the result of adding negated version of the second expression to the first expression.</returns>
        public static Expression operator -(Expression first, Expression second) =>
            AdditionExpression.Build(first, -second);

        /// <summary>
        /// Binary multiplication of expressions.
        /// </summary>
        /// <param name="first">Left-side expression.</param>
        /// <param name="second">Right-side expression.</param>
        /// <returns>Returns the result of multiplying the operand expressions.</returns>
        public static Expression operator *(Expression first, Expression second) =>
            MultiplicationExpression.Build(first, second);

        /// <summary>
        /// Binary division of expressions.
        /// </summary>
        /// <param name="first">Left-side expression.</param>
        /// <param name="second">Right-side expression.</param>
        /// <returns>Returns the result of dividing the first expression by the second expression.
        /// If immediate division is not possible, a fraction expression will be returned instead.</returns>
        public static Expression operator /(Expression first, Expression second) =>
            FractionExpression.Build(first, second);
    }
}
