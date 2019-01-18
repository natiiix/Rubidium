using System;
using System.Collections.Generic;

namespace Rubidium
{
    public static class Runtime
    {
        public static void Run(SyntaxTree tree)
        {
            Dictionary<string, Fraction> variables = new Dictionary<string, Fraction>();

            foreach (ValueExpression expr in tree.TopLevelExpressions)
            {
                if (expr is EqualityExpression equality)
                {
                    string varName = equality.Variable.Name;
                    Fraction value = equality.Value.Evaluate(variables);

                    variables[varName] = value;
                    Console.WriteLine($"{varName} = {value}");
                }
                else
                {
                    Console.WriteLine(expr.Evaluate(variables));
                }
            }
        }
    }
}
