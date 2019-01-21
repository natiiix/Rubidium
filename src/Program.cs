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
                "abs = 5 + ----abs(-12 + 5)",
                "min = min(15, 40/2, 10/-50)",
                "vars = linear(a,b,c," +
                "1,3,-2,5," +
                "3,5,6,7," +
                "2,4,3,8" +
                ")",
                "a;b;c",
                "5 + x = 12 + (y = -4)",
                "x;y"
            };

            List<Token> tokens = Lexer.Tokenize(string.Join(';', query));
            SyntaxTree tree = Parser.ParseSyntaxTree(tokens);
            Runtime.Run(tree);
        }
    }
}
