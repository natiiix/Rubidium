using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public class Statement : ICanContainVariables
    {
        public Expression Left { get; }
        public Expression Right { get; }

        public Statement SwappedSides => new Statement(Right, Left);

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

        public Statement SubstituteVariables(Dictionary<string, Fraction> variableValues, Dictionary<string, Expression> variableExpressions) =>
            new Statement(Left.SubstituteVariables(variableValues, variableExpressions), Right.SubstituteVariables(variableValues, variableExpressions));
    }
}
