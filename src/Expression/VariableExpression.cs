using System.Collections.Generic;

namespace Rubidium
{
    public class VariableExpression : Expression
    {
        public string Name { get; }

        public override IEnumerable<string> Variables => new string[] { Name };

        public VariableExpression(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;
    }
}
