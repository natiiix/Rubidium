using System;
using System.Linq;
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
            if (FunctionName == "count")
            {
                return Parameters.Count;
            }
            if (FunctionName == "abs" && Parameters.Count == 1)
            {
                return Parameters[0].Evaluate(variables).AbsoluteValue;
            }
            else if (FunctionName == "max" && Parameters.Count > 0)
            {
                return Parameters.Select(x => x.Evaluate(variables)).Max();
            }
            else if (FunctionName == "min" && Parameters.Count > 0)
            {
                return Parameters.Select(x => x.Evaluate(variables)).Min();
            }
            else if (FunctionName == "sum")
            {
                return Fraction.Sum(Parameters.Select(x => x.Evaluate(variables)));
            }
            else if (FunctionName == "avg" && Parameters.Count > 0)
            {
                return Fraction.Average(Parameters.Select(x => x.Evaluate(variables)));
            }

            throw new NotImplementedException($"Function \"{FunctionName}\" with {Parameters.Count} parameters is not implemented");
        }
    }
}
