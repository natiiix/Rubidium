using System.Collections.Generic;

namespace Rubidium
{
    public class VariableExpression : ValueExpression
    {
        public string Name { get; }

        public VariableExpression(string name)
        {
            Name = name;
        }

        public override Fraction Evaluate(Dictionary<string, Fraction> variables) => variables[Name];
    }
}
