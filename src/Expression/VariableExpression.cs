using System.Collections.Generic;

namespace Rubidium
{
    public class VariableExpression : Expression
    {
        public string Name { get; }

        public override IEnumerable<string> Variables { get; }
        public override bool ContainsVariables { get; } = true;

        public VariableExpression(string name)
        {
            Name = name;

            Variables = new string[] { Name };
        }

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues)
        {
            if (variableValues.ContainsKey(Name))
            {
                return new LiteralExpression(variableValues[Name]);
            }
            else
            {
                return this;
            }
        }

        public override string ToString() => Name;
    }
}
