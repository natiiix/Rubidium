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
            else if (FunctionName == "abs" && Parameters.Count == 1)
            {
                return Parameters[0].Evaluate(variables).AbsoluteValue;
            }
            else if (FunctionName == "sqrt" && Parameters.Count == 1)
            {
                return Parameters[0].Evaluate(variables).SquareRoot;
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
            else if (FunctionName == "if" && Parameters.Count >= 2)
            {
                for (int i = 0; i < Parameters.Count - 1; i += 2)
                {
                    Fraction condition = Parameters[i].Evaluate(variables);

                    if (!condition.IsZero)
                    {
                        return Parameters[i + 1].Evaluate(variables);
                    }
                    else if (i + 2 >= Parameters.Count)
                    {
                        return condition;
                    }
                }

                return Parameters.Last().Evaluate(variables);
            }
            else if (FunctionName == "linear" && TrySolveLinearEquationSet(Parameters, variables, out Fraction linearResult))
            {
                return linearResult;
            }

            throw new NotImplementedException($"Function \"{FunctionName}\" with {Parameters.Count} parameters is not implemented");
        }

        private static bool TrySolveLinearEquationSet(List<OperationExpression> parameters, Dictionary<string, Fraction> variables, out Fraction result)
        {
            int numberOfVars = FindNumberOfVariables(parameters.Count);

            if (numberOfVars < 0)
            {
                result = 0;
                return false;
            }

            string[] varNames = new string[numberOfVars];

            for (int i = 0; i < numberOfVars; i++)
            {
                if (parameters[i].Values.Count == 1 && parameters[i].Values[0] is VariableExpression variable)
                {
                    varNames[i] = variable.Name;
                }
                else
                {
                    result = 0;
                    return false;
                }
            }

            Vector[] matrix = new Vector[numberOfVars];

            for (int i = 0; i < numberOfVars; i++)
            {
                Fraction[] values = new Fraction[numberOfVars + 1];

                for (int j = 0; j < values.Length; j++)
                {
                    int index = numberOfVars + (i * values.Length) + j;
                    values[j] = parameters[index].Evaluate(variables);
                }

                matrix[i] = new Vector(values);
            }

            for (int i = 1; i < numberOfVars; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    matrix[i] -= (matrix[i][j] / matrix[j][j]) * matrix[j];
                }
            }

            for (int i = numberOfVars - 2; i >= 0; i--)
            {
                for (int j = numberOfVars - 1; j > i; j--)
                {
                    matrix[i] -= (matrix[i][j] / matrix[j][j]) * matrix[j];
                }
            }

            for (int i = 0; i < numberOfVars; i++)
            {
                matrix[i] /= matrix[i][i];
            }

            for (int i = 0; i < numberOfVars; i++)
            {
                variables[varNames[i]] = matrix[i][numberOfVars];
            }

            result = numberOfVars;
            return true;
        }

        private static int FindNumberOfVariables(int numberOfParams)
        {
            for (int i = 1; i < 1000; i++)
            {
                int iPlus1 = i + 1;

                if ((iPlus1 * iPlus1) - 1 == numberOfParams)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
