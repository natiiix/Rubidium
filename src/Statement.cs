using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public class Statement
    {
        public Expression Left { get; }
        public Expression Right { get; }

        public IEnumerable<string> Variables => new Expression[] { Left, Right }.SelectMany(x => x.Variables).Distinct();

        public Statement(Expression left, Expression right)
        {
            Left = left;
            Right = right;
        }

        public override string ToString() => $"{Left} = {Right}";
    }
}
