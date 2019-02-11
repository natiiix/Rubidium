using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public class FunctionCallExpression : Expression
    {
        public string FunctionName { get; }
        public List<Expression> Arguments { get; }

        public override IEnumerable<string> Variables { get; }

        private FunctionCallExpression(string funcName, List<Expression> args)
        {
            FunctionName = funcName;
            Arguments = args;

            Variables = args.GetVariables();
        }

        public static Expression Build(string funcName, List<Expression> args)
        {
            if (args.Any(x => x.ContainsVariables))
            {
                return new FunctionCallExpression(funcName, args.ToList());
            }

            throw new NotImplementedException("Function call not implemented");
        }

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues, Dictionary<string, Expression> variableExpressions) =>
            Build(FunctionName, Arguments.Select(x => x.SubstituteVariables(variableValues, variableExpressions)).ToList());
    }
}
