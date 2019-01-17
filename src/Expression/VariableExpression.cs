namespace Rubidium
{
    public class VariableExpression : ValueExpression
    {
        public string Name { get; }

        public VariableExpression(string name)
        {
            Name = name;
        }
    }
}
