using System.Collections.Generic;

namespace Rubidium
{
    /// <summary>
    /// Class used to tokenize the input query.
    /// </summary>
    public static class Lexer
    {
        /// <summary>
        /// Takes a query string and converts it into a list of tokens.
        /// </summary>
        /// <param name="query">Input query.</param>
        /// <returns>Returns a list of parsed tokens.</returns>
        public static List<Token> Tokenize(string query)
        {
            List<Token> tokens = new List<Token>();
            int index = 0;

            // Keep parsing tokens until the end of query string is reached.
            while (index < query.Length)
            {
                Token token = ParseToken(query, ref index);
                tokens.Add(token);
            }

            return tokens;
        }

        /// <summary>
        /// Parses a single token beginning at the specified index.
        /// Once the token is parsed, the index will be incremented
        /// to match the end of the parsed token.
        /// </summary>
        /// <param name="query">Input query.</param>
        /// <param name="index">Reference to next token index variable.</param>
        /// <returns>Returns the parsed token.</returns>
        private static Token ParseToken(string query, ref int index)
        {
            char first = query[index];

            // Ignore whitespace.
            if (char.IsWhiteSpace(first))
            {
                index++;
                return ParseToken(query, ref index);
            }
            // Number token. /\d+\.?\d*/
            else if (char.IsDigit(first))
            {
                int length = 1;
                bool decimalSeparator = false;

                while (index + length < query.Length && (char.IsDigit(query[index + length]) ||
                    (!decimalSeparator && (decimalSeparator = query[index + length] == Fraction.DecimalSeparator))))
                {
                    length++;
                }

                NumberToken token = new NumberToken(query.Substring(index, length), index);
                index += length;
                return token;
            }
            // Symbol token. /[a-zA-Z][a-zA-Z0-9]*/
            else if (char.IsLetter(first))
            {
                int length = 1;

                while (index + length < query.Length && char.IsLetterOrDigit(query[index + length]))
                {
                    length++;
                }

                SymbolToken token = new SymbolToken(query.Substring(index, length), index);
                index += length;
                return token;
            }
            // Special token.
            else
            {
                return new SpecialToken(first.ToString(), index++);
            }
        }
    }
}
