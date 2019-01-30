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
                "x1 = (-b + d) / (2a)",
                "x2 = (-b - d) / (2a)",
                "d = (b^2 - 4 a c)^(1/2)",
                "a = 2; b = -8; c = -24"
            };

            List<Token> tokens = Lexer.Tokenize(string.Join(';', query));
            List<Statement> statements = Parser.ParseStatements(tokens);
            Context context = new Context(statements);

            while (context.FindNewStatements()) ;

            Console.WriteLine(context);
        }
    }
}
