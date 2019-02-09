using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    /// <summary>
    /// Class used for parsing statements out of tokens.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Parses a list of statements out of a list of tokens.
        /// </summary>
        /// <param name="tokens">Input list of tokens to parse.</param>
        /// <returns>Returns list of parsed statements.</returns>
        public static List<Statement> ParseStatements(List<Token> tokens)
        {
            List<Statement> statements = new List<Statement>();

            int index = 0;

            // Keep parsing while there are tokens left.
            while (index < tokens.Count)
            {
                List<Token> statementTokens = new List<Token>();

                // Get a list of all tokens that are part of this statement.
                // Statements are separated / terminated using a statement terminator.
                while (index < tokens.Count)
                {
                    if (tokens[index] is SpecialToken special && special.Terminator)
                    {
                        index++;
                        break;
                    }

                    statementTokens.Add(tokens[index++]);
                }

                // Unless there are no tokens between the last and the next statement terminator,
                // parse a statement out of the tokens.
                if (statementTokens.Count > 0)
                {
                    statements.Add(ParseStatement(statementTokens));
                }
            }

            return statements;
        }

        /// <summary>
        /// Parse a single statement out of the given list of tokens.
        /// If a statement cannot be parser or if there are unused tokens left,
        /// an exception will be thrown.
        /// Currently, each statement should be in the form of an equation.
        /// Therefore, if the equality token is missing, an excepion will be thrown.
        /// </summary>
        /// <param name="tokens">Input list of tokens to be used for the statement.</param>
        /// <returns>Returns the parsed statement.</returns>
        private static Statement ParseStatement(List<Token> tokens)
        {
            // Find index of the equality token.
            int equalityIndex = -1;

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] is SpecialToken special && special.Equality)
                {
                    if (equalityIndex < 0)
                    {
                        equalityIndex = i;
                    }
                    else
                    {
                        throw new Exception("Unexpected second equality token");
                    }
                }
            }

            if (equalityIndex < 0)
            {
                throw new Exception("Invalid statement - no equality token");
            }

            // Parse both sides of the statement / equation.
            Expression left = ParseAddition(tokens.GetRange(0, equalityIndex), 0, out int leftLen);
            Expression right = ParseAddition(tokens, equalityIndex + 1, out int rightLen);

            // Make sure there are no unused tokens left.
            if (leftLen + 1 + rightLen != tokens.Count)
            {
                throw new Exception($"Unable to parse statement beginning at index {tokens[0].Index}");
            }

            return new Statement(left, right);
        }

        /// <summary>
        /// Parses an addition expression.
        /// If the top-level expression is not an addition,
        /// the proper type of expression will be parsed and returned.
        /// </summary>
        /// <param name="tokens">Input list of tokens.</param>
        /// <param name="start">Index of first token of the expression.</param>
        /// <param name="length">Output variable for expression length (number of tokens used).</param>
        /// <returns>Returns the parsed expression.</returns>
        private static Expression ParseAddition(List<Token> tokens, int start, out int length)
        {
            // Create list for expressions belonging to this addition.
            List<Expression> parts = new List<Expression>()
            {
                // Read the first part / sub-expression of the addition.
                ParseMultiplication(tokens, start, out int firstLen)
            };

            // Index of last processed token's end.
            int end = start + firstLen;

            // Keep going while sub-expressions are being added (or subtracted)
            // and while there are more tokens to process.
            while (end < tokens.Count && tokens[end] is SpecialToken special && (special.Addition || special.Subtraction))
            {
                // Parse next part / sub-expression of the addition.
                Expression nextPart = ParseMultiplication(tokens, end + 1, out int nextLen);
                // Convert subtraction into addition of negated expression.
                parts.Add(special.Subtraction ? NegatedExpression.Build(nextPart) : nextPart);
                // Add the length of the operator and the sub-expression to the end token index.
                end += 1 + nextLen;
            }

            // Calculate the total length of the addition.
            length = end - start;
            return AdditionExpression.Build(parts);
        }

        /// <summary>
        /// Parses a multiplication expression.
        /// If the first expression is not a part of a multiplication,
        /// it will be parsed and returned.
        /// </summary>
        /// <param name="tokens">Input list of tokens.</param>
        /// <param name="start">Index of first token of the expression.</param>
        /// <param name="length">Output variable for expression length (number of tokens used).</param>
        /// <returns>Returns the parsed expression.</returns>
        private static Expression ParseMultiplication(List<Token> tokens, int start, out int length)
        {
            List<Expression> numeratorParts = new List<Expression>() { ConstantExpression.One };
            List<Expression> denominatorParts = new List<Expression>() { ConstantExpression.One };

            Expression firstPart = ParseExponent(tokens, start, out int firstLen);
            numeratorParts.Add(firstPart);

            int end = start + firstLen;
            bool lastWasDivision = false;

            while (end < tokens.Count)
            {
                if (tokens[end] is SpecialToken special && (special.Multiplication || special.Division))
                {
                    Expression nextPart = ParseExponent(tokens, end + 1, out int nextLen);
                    (special.Division ? denominatorParts : numeratorParts).Add(nextPart);
                    end += 1 + nextLen;
                    lastWasDivision = special.Division;
                }
                else if (tokens[end] is SymbolToken || tokens[end] is NumberToken)
                {
                    Expression nextPart = ParseExponent(tokens, end, out int nextLen);
                    (lastWasDivision ? denominatorParts : numeratorParts).Add(nextPart);
                    end += nextLen;
                }
                else
                {
                    break;
                }
            }

            length = end - start;

            return FractionExpression.Build(
                MultiplicationExpression.Build(numeratorParts),
                MultiplicationExpression.Build(denominatorParts)
            );
        }

        /// <summary>
        /// Parses an exponent expression.
        /// If the first expression has no exponent,
        /// that expression will be parsed and returned instead.
        /// </summary>
        /// <param name="tokens">Input list of tokens.</param>
        /// <param name="start">Index of first token of the expression.</param>
        /// <param name="length">Output variable for expression length (number of tokens used).</param>
        /// <returns>Returns the parsed expression.</returns>
        private static Expression ParseExponent(List<Token> tokens, int start, out int length)
        {
            Expression baseValue = ParseExpression(tokens, start, out int baseLen);

            if (start + baseLen < tokens.Count && tokens[start + baseLen] is SpecialToken special && special.Power)
            {
                Expression exponent = ParseExponent(tokens, start + baseLen + 1, out int exponentLen);
                length = baseLen + 1 + exponentLen;
                return ExponentExpression.Build(baseValue, exponent);
            }
            else
            {
                length = baseLen;
                return baseValue;
            }
        }

        /// <summary>
        /// Parses a single expression.
        /// It can be a constant, variable, negation or addition.
        /// </summary>
        /// <param name="tokens">Input list of tokens.</param>
        /// <param name="start">Index of first token of the expression.</param>
        /// <param name="length">Output variable for expression length (number of tokens used).</param>
        /// <returns>Returns the parsed expression.</returns>
        private static Expression ParseExpression(List<Token> tokens, int start, out int length)
        {
            Token first = tokens[start];

            if (first is NumberToken number)
            {
                length = 1;
                return number.NumericValue;
            }
            else if (first is SymbolToken symbol)
            {
                length = 1;
                return new VariableExpression(symbol.StringValue);
            }
            else if (first is SpecialToken firstSpecial)
            {
                if (firstSpecial.Subtraction)
                {
                    Expression expr = ParseExpression(tokens, start + 1, out int exprLen);
                    length = exprLen + 1;
                    return NegatedExpression.Build(expr);
                }
                else if (firstSpecial.LeftParenthesis)
                {
                    Expression expr = ParseAddition(tokens, start + 1, out int exprLen);

                    if (!(tokens[start + 1 + exprLen] is SpecialToken endSpecial && endSpecial.RightParenthesis))
                    {
                        throw new Exception($"Unexpected end of parenthesis expression");
                    }

                    length = exprLen + 2;
                    return expr;
                }
            }

            throw new Exception($"Unexpected token \"{tokens[start].StringValue}\" at index {tokens[start].Index}");
        }
    }
}
