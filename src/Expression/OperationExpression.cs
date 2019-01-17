using System.Collections.Generic;

namespace Rubidium
{
    public class OperationExpression : ValueExpression
    {
        public List<ValueExpression> Values { get; }
        public List<Operation> Operations { get; }

        public OperationExpression(List<ValueExpression> values, List<Operation> operations)
        {
            Values = values;
            Operations = operations;
        }
    }
}
