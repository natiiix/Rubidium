using System;
using System.Collections.Generic;

namespace Rubidium
{
    public static class Parser
    {
        public static SyntaxTree ParseSyntaxTree(List<Token> tokens)
        {
            SyntaxTree tree = new SyntaxTree();

            int i = 0;
            for (; i < tokens.Count && TryParseTopLevelExpression(tokens, i, out int length, out ValueExpression expr); i += length)
            {
                tree.TopLevelExpressions.Add(expr);
            }

            if (i < tokens.Count)
            {
                throw new Exception($"Unexpected token \"{tokens[i].StringValue}\" at index {tokens[i].Index}");
            }

            return tree;
        }

        private static bool TryParseTopLevelExpression(List<Token> tokens, int index, out int length, out ValueExpression expr)
        {
            length = default(int);
            expr = default(EqualityExpression);

            if (tokens[index] is SpecialToken firstSpecial && firstSpecial.Terminator)
            {
                bool success = TryParseTopLevelExpression(tokens, index + 1, out int subLen, out expr);
                length = 1 + subLen;
                return success;
            }
            else if (tokens.Count - index >= 3 && tokens[index] is SymbolToken && tokens[index + 1] is SpecialToken special && special.Equality)
            {
                bool success = TryParseOperationExpression(tokens, index + 2, out int opLen, out OperationExpression opExpr);
                length = 2 + opLen;
                expr = new EqualityExpression(new VariableExpression(tokens[index].StringValue), opExpr);
                return success;
            }
            else
            {
                bool success = TryParseOperationExpression(tokens, index, out length, out OperationExpression opExpr);
                expr = opExpr;
                return success;
            }
        }

        private static bool TryParseOperationExpression(List<Token> tokens, int index, out int length, out OperationExpression expr)
        {
            length = default(int);
            expr = default(OperationExpression);

            List<ValueExpression> values = new List<ValueExpression>();
            List<Operation> operations = new List<Operation>();

            if (!TryParseValueExpression(tokens, index, out int firstLen, out ValueExpression firstExpr))
            {
                return false;
            }

            values.Add(firstExpr);

            int i = index + firstLen;

            while (!IsEndOfTokens(tokens, i) && IsOperationToken(tokens[i], out Operation op) &&
                TryParseValueExpression(tokens, i + 1, out int valueLen, out ValueExpression valueExpr))
            {
                operations.Add(op);
                values.Add(valueExpr);

                i += 1 + valueLen;
            }

            if (IsEndOfTokens(tokens, i) || (tokens[i] is SpecialToken special && (special.RightParenthesis || special.ParameterSeparator)))
            {
                length = i - index;
                expr = new OperationExpression(values, operations);
                return true;
            }

            return false;
        }

        private static bool TryParseValueExpression(List<Token> tokens, int index, out int length, out ValueExpression expr)
        {
            length = default(int);
            expr = default(ValueExpression);

            if (IsEndOfTokens(tokens, index))
            {
                return false;
            }

            Token first = tokens[index];

            if (first is IntegerToken integer)
            {
                length = 1;
                expr = new LiteralExpression(integer.IntegerValue);
                return true;
            }
            else if (first is SymbolToken)
            {
                if (!IsEndOfTokens(tokens, index + 1) && tokens[index + 1] is SpecialToken leftSpecial && leftSpecial.LeftParenthesis)
                {
                    List<OperationExpression> parameters = new List<OperationExpression>();
                    int i = index + 2;

                    if (!IsEndOfTokens(tokens, i) && tokens[i] is SpecialToken rightSpecial && rightSpecial.RightParenthesis)
                    {
                        length = i + 1 - index;
                        expr = new FunctionCallExpression(first.StringValue, parameters);
                        return true;
                    }

                    while (!IsEndOfTokens(tokens, i) && TryParseOperationExpression(tokens, i, out int opLen, out OperationExpression opExpr))
                    {
                        parameters.Add(opExpr);
                        i += opLen;

                        if (!IsEndOfTokens(tokens, i) && tokens[i] is SpecialToken nextSpecial)
                        {
                            if (nextSpecial.RightParenthesis)
                            {
                                length = i + 1 - index;
                                expr = new FunctionCallExpression(first.StringValue, parameters);
                                return true;
                            }
                            else if (nextSpecial.ParameterSeparator)
                            {
                                i++;
                                continue;
                            }
                        }

                        break;
                    }
                }

                length = 1;
                expr = new VariableExpression(first.StringValue);
                return true;
            }
            else if (first is SpecialToken special)
            {
                if (special.Subtraction && TryParseValueExpression(tokens, index + 1, out int valueLen, out ValueExpression valueExpr))
                {
                    length = 1 + valueLen;
                    expr = new NegationExpression(valueExpr);
                    return true;
                }
                else if (special.LeftParenthesis && TryParseOperationExpression(tokens, index + 1, out int opLen, out OperationExpression opExpr) &&
                    tokens[index + 1 + opLen] is SpecialToken opEnd && opEnd.RightParenthesis)
                {
                    length = 2 + opLen;
                    expr = opExpr;
                    return true;
                }
            }

            return false;
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

        private static bool IsEndOfTokens(List<Token> tokens, int index) =>
            index >= tokens.Count || (tokens[index] is SpecialToken && tokens[index].StringValue == ";");
    }
}
