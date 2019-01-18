using System.Collections.Generic;

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

        public override Fraction Evaluate(Dictionary<string, Fraction> variables)
        {
            variables[Variable.Name] = Value.Evaluate(variables);
            return variables[Variable.Name];
        }
    }
}
