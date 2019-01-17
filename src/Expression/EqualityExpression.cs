namespace Rubidium
{
    public class EqualityExpression : ValueExpression
    {
        public VariableExpression Variable { get; }
        public ValueExpression Value { get; }

        public EqualityExpression(VariableExpression variable, ValueExpression value)
        {
            Variable = variable;
            Value = value;
        }
    }
}
