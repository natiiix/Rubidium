using System;
using System.Collections.Generic;

namespace Rubidium
{
    public static class Runtime
    {
        public static void Run(SyntaxTree tree)
        {
            Dictionary<string, Fraction> variables = new Dictionary<string, Fraction>();

            foreach (OperationExpression expr in tree.TopLevelExpressions)
            {
                Fraction value = expr.Evaluate(variables);

                if (expr.Values.Count == 1 && expr.Values[0] is EqualityExpression equality)
                {
                    Console.Write($"{equality.Variable.Name} = ");
                }

                Console.WriteLine($"{value} = {(double)value:g}");
            }
        }
    }
}
