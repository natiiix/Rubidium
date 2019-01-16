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
                Token token = ParseToken(str, index);
                tokens.Add(token);
                index += token.StringValue.Length;
            }

            return tokens;
        }

        public static Token ParseToken(string str, int index)
        {
            char first = str[index];

            if (first == ' ')
            {
                return ParseToken(str, index + 1);
            }
            else if (char.IsDigit(first))
            {
                int length = 1;

                while (index + length < str.Length && char.IsDigit(str[index + length]))
                {
                    length++;
                }

                return new IntegerToken(str.Substring(index, length), index);
            }
            else if (char.IsLetter(first))
            {
                int length = 1;

                while (index + length < str.Length && char.IsLetterOrDigit(str[index + length]))
                {
                    length++;
                }

                return new SymbolToken(str.Substring(index, length), index);
            }
            else
            {
                return new SpecialToken(first.ToString(), index);
            }
        }
    }
}
