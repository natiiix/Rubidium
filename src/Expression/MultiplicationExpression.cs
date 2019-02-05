using System.Collections.Generic;

namespace Rubidium
{
    public class MultiplicationExpression : OperationExpression
    {
        internal MultiplicationExpression(List<Expression> expressions) : base(expressions, Operation.Multiplication) { }
    }
}
