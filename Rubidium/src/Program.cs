using System;
using System.Collections.Generic;

namespace Rubidium
{
    /// <summary>
    /// Core class of the project.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Application entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        private static void Main(string[] args)
        {
            Context context = null;

            // If the query has be supplied through command line arguments.
            if (args.Length > 0)
            {
                // Tokenize query, parse tokens into statements and build Context from statements.
                context = new Context(
                    Parser.ParseStatements(
                        Lexer.Tokenize(string.Join(';', args))
                    )
                );
            }
            // Otherwise, if there are no command line arguments.
            else
            {
                List<Statement> statements = new List<Statement>();

                // Let the user type all their statements one by one via Console / stdin.
                while (true)
                {
                    // Prompt user to enter another statement.
                    Console.Write("Enter statement (leave empty to finish): ");
                    string line = Console.ReadLine();

                    // Stop once the user enters an empty line.
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        break;
                    }

                    // Tokenize query, parse tokens into statement and add statement to list.
                    statements.AddRange(
                        Parser.ParseStatements(
                            Lexer.Tokenize(line)
                        )
                    );
                }

                // Build a Context from the statements.
                context = new Context(statements);
            }

            Console.WriteLine($"-- Initial context state --{Environment.NewLine}{context}{Environment.NewLine}");

            // Keep trying to find new statements until it is no longer possible.
            while (context.PerformIteration()) ;

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

            while (context.PerformIteration()) ;

            return context;
        }
    }
}
