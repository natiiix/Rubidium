using System.Collections.Generic;

namespace Rubidium
{
    public abstract class ValueExpression
    {
        public abstract Fraction Evaluate(Dictionary<string, Fraction> variables);
    }
}
