using System;
using System.Collections.Generic;

namespace Rubidium
{
    public class FunctionCallExpression : ValueExpression
    {
        public string FunctionName { get; }
        public List<OperationExpression> Parameters { get; }

        public FunctionCallExpression(string name, List<OperationExpression> parameters)
        {
            FunctionName = name;
            Parameters = parameters;
        }

        public override Fraction Evaluate(Dictionary<string, Fraction> variables)
        {
            if (FunctionName == "abs" && Parameters.Count == 1)
            {
                return Parameters[0].Evaluate(variables).AbsoluteValue;
            }

            throw new NotImplementedException($"Function \"{FunctionName}\" with {Parameters.Count} parameters is not implemented");
        }
    }
}
