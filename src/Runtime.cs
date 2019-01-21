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
                Fraction value;

                if (expr is EqualityExpression equality)
                {
                    string varName = equality.Variable.Name;
                    value = equality.Value.Evaluate(variables);

                    variables[varName] = value;
                    Console.Write($"{varName} = ");
                }
                else
                {
                    value = expr.Evaluate(variables);
                }

                Console.WriteLine($"{value} = {(double)value:g}");
            }
        }
    }
}
