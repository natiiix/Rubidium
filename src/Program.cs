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
                "c = (((3a))) + b",
                "b = a",
                "2 = a",
                "7 = 12",
                "4 = 4"
            };

            List<Token> tokens = Lexer.Tokenize(string.Join(';', query));
            List<Statement> statements = Parser.ParseStatements(tokens);
            Context context = new Context(statements);

            while (context.FindNewStatements()) ;
        }
    }
}
