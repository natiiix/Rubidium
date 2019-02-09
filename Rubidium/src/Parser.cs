using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public static class Parser
    {
        public static List<Statement> ParseStatements(List<Token> tokens)
        {
            List<Statement> statements = new List<Statement>();

            int index = 0;

            while (index < tokens.Count)
            {
                List<Token> statementTokens = new List<Token>();

                while (index < tokens.Count)
                {
                    if (tokens[index] is SpecialToken special && special.Terminator)
                    {
                        index++;
                        break;
                    }

                    statementTokens.Add(tokens[index++]);
                }

                if (statementTokens.Count > 0)
                {
                    statements.Add(ParseStatement(statementTokens));
                }
            }

            return statements;
        }

        private static Statement ParseStatement(List<Token> tokens)
        {
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

            Expression left = ParseAddition(tokens.GetRange(0, equalityIndex), 0, out int leftLen);
            Expression right = ParseAddition(tokens, equalityIndex + 1, out int rightLen);

            if (leftLen + 1 + rightLen != tokens.Count)
            {
                throw new Exception($"Unable to parse statement beginning at index {tokens[0].Index}");
            }

            return new Statement(left, right);
        }

        private static Expression ParseAddition(List<Token> tokens, int start, out int length)
        {
            List<Expression> parts = new List<Expression>();

            Expression firstPart = ParseMultiplication(tokens, start, out int firstLen);
            parts.Add(firstPart);

            int end = start + firstLen;

            while (end < tokens.Count && tokens[end] is SpecialToken special && (special.Addition || special.Subtraction))
            {
                Expression nextPart = ParseMultiplication(tokens, end + 1, out int nextLen);
                parts.Add(special.Subtraction ? NegatedExpression.Build(nextPart) : nextPart);
                end += 1 + nextLen;
            }

            length = end - start;
            return AdditionExpression.Build(parts);
        }

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

        private static bool IsOperationToken(Token token, out Operation op)
        {
            op = default(Operation);

            if (token is SpecialToken special)
            {
                if (special.Addition)
                {
                    op = Operation.Addition;
                }
                else if (special.Subtraction)
                {
                    op = Operation.Subtraction;
                }
                else if (special.Multiplication)
                {
                    op = Operation.Multiplication;
                }
                else if (special.Division)
                {
                    op = Operation.Division;
                }
                else if (special.Power)
                {
                    op = Operation.Power;
                }
                else
                {
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}
