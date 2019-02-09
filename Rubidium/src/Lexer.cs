using System.Collections.Generic;

namespace Rubidium
{
    public static class Lexer
    {
        public static List<Token> Tokenize(string str)
        {
            List<Token> tokens = new List<Token>();
            int index = 0;

            while (index < str.Length)
            {
                Token token = ParseToken(str, ref index);
                tokens.Add(token);
            }

            return tokens;
        }

        private static Token ParseToken(string str, ref int index)
        {
            char first = str[index];

            if (char.IsWhiteSpace(first))
            {
                index++;
                return ParseToken(str, ref index);
            }
            else if (char.IsDigit(first))
            {
                int length = 1;
                bool decimalSeparator = false;

                while (index + length < str.Length && (char.IsDigit(str[index + length]) ||
                    (!decimalSeparator && (decimalSeparator = str[index + length] == Fraction.DecimalSeparator))))
                {
                    length++;
                }

                NumberToken token = new NumberToken(str.Substring(index, length), index);
                index += length;
                return token;
            }
            else if (char.IsLetter(first))
            {
                int length = 1;

                while (index + length < str.Length && char.IsLetterOrDigit(str[index + length]))
                {
                    length++;
                }

                SymbolToken token = new SymbolToken(str.Substring(index, length), index);
                index += length;
                return token;
            }
            else
            {
                return new SpecialToken(first.ToString(), index++);
            }
        }
    }
}
