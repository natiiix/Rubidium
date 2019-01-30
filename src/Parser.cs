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

            Expression left = ParseExpression(tokens.GetRange(0, equalityIndex));
            Expression right = ParseExpression(tokens.GetRange(equalityIndex + 1, tokens.Count - equalityIndex - 1));

            return new Statement(left, right);
        }

        private static Expression ParseExpression(List<Token> tokens)
        {
            if (tokens.Count == 0)
            {
                throw new Exception("Unable to parse expression from zero tokens");
            }

            if (tokens.Count == 1)
            {
                if (tokens[0] is IntegerToken integer)
                {
                    return new LiteralExpression(integer.IntegerValue);
                }
                else if (tokens[0] is SymbolToken symbol)
                {
                    return new VariableExpression(symbol.StringValue);
                }

                throw new Exception("Invalid single-token expression");
            }

            List<Expression> expressions = new List<Expression>();
            List<Operation> operations = new List<Operation>();

            List<Expression> parts = new List<Expression>();

            for (int i = 0; i < tokens.Count; i++)
            {
                if (IsOperationToken(tokens[i], out Operation op))
                {
                    expressions.Add(OperationExpression.CreateMultiplication(parts));
                    parts.Clear();
                    operations.Add(op);
                }
                else
                {
                    parts.Add(ParseExpression(tokens.GetRange(i, 1)));
                }
            }

            expressions.Add(OperationExpression.CreateMultiplication(parts));
            return new OperationExpression(expressions, operations);
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
