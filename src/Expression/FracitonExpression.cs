using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public class FractionExpression : Expression
    {
        public Expression Numerator { get; }
        public Expression Denominator { get; }

        public override IEnumerable<string> Variables { get; }

        private FractionExpression(Expression numerator, Expression denominator)
        {
            Variables = new Expression[] { numerator, denominator }.GetVariables();
        }

        public static Expression Build(Expression numerator, Expression denominator)
        {
            return new FractionExpression(numerator, denominator);
        }

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues) =>
            Build(Numerator.SubstituteVariables(variableValues), Denominator.SubstituteVariables(variableValues));
    }
}
