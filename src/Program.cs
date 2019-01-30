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
                "a = 2"
            };

            List<Token> tokens = Lexer.Tokenize(string.Join(';', query));
            List<Statement> statements = Parser.ParseStatements(tokens);
            Context context = new Context(statements);

            while (context.FindNewStatements()) ;
        }
    }
}
