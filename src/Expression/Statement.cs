using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public class Statement : ICanContainVariables
    {
        public Expression Left { get; }
        public Expression Right { get; }

        public IEnumerable<string> Variables { get; }

        public bool ContainsVariables { get; }

        public Statement(Expression left, Expression right)
        {
            Left = left;
            Right = right;

            Variables = new Expression[] { left, right }.GetVariables();
            ContainsVariables = Variables.Count() > 0;
        }

        public override string ToString() => $"{Left} = {Right}";

        public Statement SubstituteVariables(Dictionary<string, Fraction> variableValues) =>
            new Statement(Left.SubstituteVariables(variableValues), Right.SubstituteVariables(variableValues));
    }
}
