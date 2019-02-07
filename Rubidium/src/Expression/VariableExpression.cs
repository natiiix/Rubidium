using System.Collections.Generic;

namespace Rubidium
{
    public class VariableExpression : Expression
    {
        public string Name { get; }

        public override IEnumerable<string> Variables { get; }

        public VariableExpression(string name)
        {
            Name = name;

            Variables = new string[] { Name };
        }

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues, Dictionary<string, Expression> variableExpressions)
        {
            if (variableValues.ContainsKey(Name))
            {
                return new ConstantExpression(variableValues[Name]);
            }
            else if (variableExpressions.ContainsKey(Name))
            {
                return variableExpressions[Name];
            }
            else
            {
                return this;
            }
        }

        public override string ToString() => Name;
    }
}
