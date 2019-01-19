using System;
using System.Collections.Generic;

namespace Rubidium
{
    public class Program
    {
        private static void Main(string[] args)
        {
            string[] query =
            {
                "a = 1",
                "b = 3",
                "c = -4",
                "d = (b^2 - 4 * a * c)^(1/2)",
                "x1 = (-b + d) / (2 * a)",
                "x2 = (-b - d) / (2 * a)",
                "abs = 5 + ----abs(-12 + 5)"
            };

            List<Token> tokens = Lexer.Tokenize(string.Join(';', query));
            SyntaxTree tree = Parser.ParseSyntaxTree(tokens);
            Runtime.Run(tree);
        }
    }
}
