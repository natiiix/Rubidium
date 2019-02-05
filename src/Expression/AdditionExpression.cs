using System.Collections.Generic;

namespace Rubidium
{
    public class AdditionExpression : OperationExpression
    {
        internal AdditionExpression(List<Expression> expressions) : base(expressions, Operation.Addition) { }
    }
}
