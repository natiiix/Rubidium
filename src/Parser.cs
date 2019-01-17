using System;
using System.Collections.Generic;

namespace Rubidium
{
    public static class Parser
    {
        public static SyntaxTree ParseSyntaxTree(List<Token> tokens)
        {
            SyntaxTree tree = new SyntaxTree();

            for (int i = 0; i < tokens.Count && TryParseTopLevelExpression(tokens, i, out int length, out ValueExpression expr); i += length)
            {
                tree.TopLevelExpressions.Add(expr);
            }

            return tree;
        }

        private static bool TryParseTopLevelExpression(List<Token> tokens, int index, out int length, out ValueExpression expr)
        {
            length = default(int);
            expr = default(EqualityExpression);

            if (tokens.Count - index >= 3 && tokens[index] is SymbolToken && tokens[index + 1] is SpecialToken special && special.Equality)
            {
                bool success = TryParseOperationExpression(tokens, index + 2, out int opLen, out OperationExpression opExpr);
                length = 2 + opLen;
                expr = opExpr;
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

            if (IsEndOfTokens(tokens, i) || (tokens[i] is SpecialToken special && special.RightParenthesis))
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

        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%5

        // private static ValueExpression ParseValueExpression(List<Token> tokens, ref int index)
        // {
        //     Token first = tokens[index];
        //     bool isLast = index == tokens.Count - 1;

        //     Token next = isLast ? null : tokens[index + 1];
        //     isLast |= next is SpecialToken special && special.StringValue == ";";

        //     if (isLast)
        //     {
        //         index += next == null ? 1 : 2;

        //         if (first is SymbolToken)
        //         {
        //             return new VariableExpression(first.StringValue);
        //         }
        //         else if (first is IntegerToken integer)
        //         {
        //             return new LiteralExpression(integer.IntegerValue);
        //         }
        //     }
        //     else if (next is SpecialToken special)
        //     {
        //         if (first is SymbolToken && special.StringValue == "=")
        //         {
        //             index += 2;
        //             ValueExpression remainder = ParseValueExpression(tokens, ref index);
        //             return new EqualityExpression(new VariableExpression(first.StringValue), );
        //         }
        //         else if (special.StringValue == "+")
        //         {
        //             if (first is IntegerToken integer)
        //             {

        //             }
        //         }
        //     }

        //     throw new Exception($"Unexpected token \"{tokens[index].StringValue}\" at index {tokens[index].Index}");
        // }

        // private static bool TryParseParenthesisExpression(List<Token> tokens, int index, out int length, out ValueExpression expr)
        // {
        //     for (int i = index + 1; i < tokens.Count; i++)
        //     {
        //         Token token = tokens[i];

        //         if (token is SpecialToken special && special.LeftParenthesis)
        //         {
        //             if (special.LeftParenthesis)
        //             {
        //                 length = i - index + 1;
        //                 expr = null; // TODO
        //                 return true;
        //             }
        //             else if (special.RightParenthesis)
        //             {
        //                 if (TryParseParenthesisExpression(tokens, i, out int sublength, out ValueExpression subexpr))
        //                 {
        //                     // TODO
        //                 }
        //             }
        //         }

        //         // TODO
        //     }

        //     length = 0;
        //     expr = null;
        //     return false;
        // }

        // private static bool IsSpecialToken(Token token, string value) => token is SpecialToken && token.StringValue == value;

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
