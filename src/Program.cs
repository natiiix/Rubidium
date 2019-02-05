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
                // x = { 6; -2 }
                // "x1 = (-b + d) / 2a",
                // "x2 = (-b - d) / 2a",
                // "d = (b^2 - 4 a c)^(1/2)",
                // "a = 2; b = -8; c = -24"
                "(-b + d) / 2a = x1",
                "(-b - d) / 2a = x2",
                "(b^2 - 4 a c)^(1/2) = d",
                "2 = a; -8 = b; -24 = c"

                // (x, y, z) = ( 3/10, 2/5, 0 )
                // "2x + y + 3z = 1",
                // "2x + 6y + 8z = 3",
                // "6x + 8y + 18z = 5"

                // "x = 5 + (7 - (5 + 12)) + 4"
                // "x = 5 * (7 * (5 * 12)) * 4"

                // "10x - 5x + 10 = -15x + 20"
            };

            List<Token> tokens = Lexer.Tokenize(string.Join(';', query));
            List<Statement> statements = Parser.ParseStatements(tokens);
            Context context = new Context(statements);

            while (context.FindNewStatements()) ;

            Console.WriteLine(context);
        }
    }
}
