using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public class Statement : Expression
    {
        public Expression Left { get; }
        public Expression Right { get; }

        public override IEnumerable<string> Variables { get; }

        public override bool ContainsVariables { get; }

        public Statement(Expression left, Expression right)
        {
            Left = left;
            Right = right;

            Variables = new Expression[] { left, right }.SelectMany(x => x.Variables).Distinct();
            ContainsVariables = Variables.Count() > 0;
        }

        public override string ToString() => $"{Left} = {Right}";

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues)
            => new Statement(Left.SubstituteVariables(variableValues), Right.SubstituteVariables(variableValues));
    }
}
