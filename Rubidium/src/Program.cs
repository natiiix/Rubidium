using System;
using System.Collections.Generic;

namespace Rubidium
{
    public static class Program
    {
        /// <summary>
        /// Application entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        private static void Main(string[] args)
        {
            Context context = null;

            if (args.Length > 0)
            {
                context = new Context(Parser.ParseStatements(
                    Lexer.Tokenize(string.Join(';', args))
                ));
            }
            else
            {
                List<Statement> statements = new List<Statement>();

                while (true)
                {
                    Console.Write("Enter statement (leave empty to finish): ");
                    string line = Console.ReadLine();

                    if (string.IsNullOrEmpty(line))
                    {
                        break;
                    }

                    statements.AddRange(Parser.ParseStatements(
                        Lexer.Tokenize(line)
                    ));
                }

                context = new Context(statements);
            }

            Console.WriteLine($"-- Initial context state --{Environment.NewLine}{context}{Environment.NewLine}");

            while (context.FindNewStatements()) ;

            Console.WriteLine($"-- Final context state --{Environment.NewLine}{context}");
        }

        /// <summary>
        /// Performs all steps of query evaluation in a single call in the quiet mode (no printing to Console).
        /// This method is primarily meant for running complex tests of Context logic.
        /// </summary>
        /// <param name="query">Input query.</param>
        /// <returns>Returns final state of Context.</returns>
        public static Context Evaluate(string query)
        {
            List<Token> tokens = Lexer.Tokenize(query);
            List<Statement> statements = Parser.ParseStatements(tokens);
            Context context = new Context(statements, false);

            while (context.FindNewStatements()) ;

            return context;
        }
    }
}
