using System.Collections.Generic;

namespace Rubidium
{
    public class VariableExpression : Expression
    {
        public override bool IsBound => false;

        public string Name { get; }

        public VariableExpression(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;
    }
}
