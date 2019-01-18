using System;
using System.Collections.Generic;

namespace Rubidium
{
    public class Program
    {
        private static void Main(string[] args)
        {
            // Console.WriteLine((0).ToString("e17"));
            // Console.WriteLine((-1).ToString("e17"));
            // Console.WriteLine((-1.23456).ToString("e15"));
            // Console.WriteLine((-1.23456e30).ToString("e15"));
            // Console.WriteLine((-1.23456e-30).ToString("e15"));
            // Console.WriteLine(new Fraction(420, 69));
            // Console.WriteLine(new Fraction(0, 69));

            // foreach (double d in new double[] { -1.23456789012345678e20, 1.23456789012345678e-20 })
            // {
            //     Fraction f = Fraction.FromDouble(d);

            //     Console.WriteLine(d.ToString("e15"));
            //     Console.WriteLine(((double)f).ToString("e15"));

            //     Console.WriteLine(f);
            // }

            List<Token> tokens = Lexer.Tokenize("x = 12 ; y = x");
            SyntaxTree tree = Parser.ParseSyntaxTree(tokens);
            Runtime.Run(tree);
        }
    }
}
