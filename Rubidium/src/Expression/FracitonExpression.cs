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
            Numerator = numerator;
            Denominator = denominator;

            Variables = new Expression[] { numerator, denominator }.GetVariables();
        }

        public static Expression Build(Expression numerator, Expression denominator)
        {
            if (denominator is ConstantExpression denomConst)
            {
                return denomConst == Fraction.One ? numerator : numerator * ~denomConst.Value;
            }
            else if (numerator is FractionExpression numerFract)
            {
                return Build(numerFract.Numerator, numerFract.Denominator * denominator);
            }
            else if (denominator is FractionExpression denomFract)
            {
                return Build(numerator * denomFract.Denominator, denomFract.Numerator);
            }
            else
            {
                return new FractionExpression(numerator, denominator);
            }
        }

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues, Dictionary<string, Expression> variableExpressions) =>
            Build(Numerator.SubstituteVariables(variableValues, variableExpressions), Denominator.SubstituteVariables(variableValues, variableExpressions));

        public override string ToString() => $"({Numerator} / {Denominator})";
    }
}