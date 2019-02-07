using System.Collections.Generic;

namespace Rubidium
{
    public class ExponentExpression : Expression
    {
        public Expression BaseValue { get; }
        public Expression Exponent { get; }

        public override IEnumerable<string> Variables { get; }

        private ExponentExpression(Expression baseValue, Expression exponent)
        {
            BaseValue = baseValue;
            Exponent = exponent;

            Variables = new Expression[] { baseValue, exponent }.GetVariables();
        }

        public static Expression Build(Expression baseValue, Expression exponent)
        {
            if (baseValue is ConstantExpression baseConst && exponent is ConstantExpression exponentConst)
            {
                return baseConst.Value ^ exponentConst.Value;
            }
            else
            {
                return new ExponentExpression(baseValue, exponent);
            }
        }

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues, Dictionary<string, Expression> variableExpressions) =>
            Build(BaseValue.SubstituteVariables(variableValues, variableExpressions), Exponent.SubstituteVariables(variableValues, variableExpressions));

        public override string ToString() => $"({BaseValue}^{Exponent})";
    }
}
