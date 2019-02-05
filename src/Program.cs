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
                "2x + y + 3z = 1",
                "2x + 6y + 8z = 3",
                "6x + 8y + 18z = 5"
            };

            List<Token> tokens = Lexer.Tokenize(string.Join(';', query));
            List<Statement> statements = Parser.ParseStatements(tokens);
            Context context = new Context(statements);

            while (context.FindNewStatements()) ;

            Console.WriteLine(context);
        }
    }
}
