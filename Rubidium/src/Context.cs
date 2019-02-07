using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    public class Context
    {
        private bool Verbose { get; }
        private IEnumerable<string> Variables { get; }

        private List<Statement> Statements { get; }
        private Dictionary<string, Expression> VariableExpressions { get; }
        private Dictionary<string, Fraction> VariableValues { get; }

        private IEnumerable<string> FreeVariables => Variables.Where(x => !VariableExpressions.ContainsKey(x) && !VariableValues.ContainsKey(x));

        public Context(List<Statement> initialStatements, bool verbose = true)
        {
            Verbose = verbose;
            Variables = initialStatements.GetVariables();

            Statements = new List<Statement>(initialStatements);
            VariableExpressions = new Dictionary<string, Expression>();
            VariableValues = new Dictionary<string, Fraction>();
        }

        public bool FindNewStatements()
        {
            int newVariablesValues = 0;
            List<Statement> keepStatements = new List<Statement>();
            List<Statement> newStatements = new List<Statement>();
            List<string> expressedVariables = new List<string>();

            foreach (Statement s in Statements)
            {
                if (!s.Left.ContainsVariables && !s.Right.ContainsVariables)
                {
                    PrintIfVerbose($"{s} : {(s.Left - s.Right) is ConstantExpression constant && constant.Value == 0}");
                }
                else if (s.Variables.Any(x => VariableValues.ContainsKey(x)))
                {
                    newStatements.Add(s.SubstituteVariables(VariableValues));
                }
                else if (s.Left is VariableExpression leftVariable &&
                    FreeVariables.Contains(leftVariable.Name))
                {
                    VariableExpressions[leftVariable.Name] = s.Right;
                }
                else if (s.Right is VariableExpression rightVariable)
                {
                    newStatements.Add(new Statement(s.Right, s.Left));
                }
                else if (s.Left is MultiplicationExpression leftMultiplication &&
                    leftMultiplication.IsVariableWithCoefficient &&
                    FreeVariables.Contains(leftMultiplication.VariableName))
                {
                    VariableExpressions[leftMultiplication.VariableName] = s.Right / new ConstantExpression(leftMultiplication.Coefficient);
                }
                else if (s.Left is AdditionExpression leftAddition &&
                    (s.Right is AdditionExpression || !leftAddition.Constant.IsZero || leftAddition.VariableParts.Any(x => IsUsableVariable(x, expressedVariables))))
                {
                    if (s.Right is AdditionExpression rightAddition)
                    {
                        Fraction constant = rightAddition.Constant - leftAddition.Constant;
                        List<Expression> variableParts = new List<Expression>();

                        variableParts.AddRange(leftAddition.VariableParts);
                        variableParts.AddRange(rightAddition.VariableParts.Select(x => -x));

                        newStatements.Add(new Statement(
                            AdditionExpression.Build(variableParts),
                            new ConstantExpression(constant)
                        ));
                    }
                    else if (!leftAddition.Constant.IsZero)
                    {
                        newStatements.Add(new Statement(
                            AdditionExpression.Build(leftAddition.VariableParts),
                            s.Right + new ConstantExpression(-leftAddition.Constant)
                        ));
                    }
                    else
                    {
                        Expression usableVar = leftAddition.VariableParts.Find(x => IsUsableVariable(x, expressedVariables));

                        newStatements.Add(new Statement(
                            usableVar,
                            s.Right - AdditionExpression.Build(leftAddition.Constant, leftAddition.VariableParts.Where(x => x != usableVar))
                        ));

                        expressedVariables.Add(usableVar is VariableExpression varExpr ?
                            varExpr.Name :
                            (usableVar as MultiplicationExpression).VariableName
                        );
                    }
                }
                else if (s.Right.ContainsVariables)
                {
                    newStatements.Add(s.Left.ContainsVariables ?
                        new Statement(s.Left - s.Right, ConstantExpression.Zero) :
                        new Statement(s.Right, s.Left)
                    );
                }
                else
                {
                    keepStatements.Add(s);
                }
            }

            Dictionary<string, Expression> newVarExpressions = new Dictionary<string, Expression>();

            foreach (var varExpr in VariableExpressions)
            {
                if (varExpr.Value is ConstantExpression constant)
                {
                    VariableValues[varExpr.Key] = constant.Value;
                    newVariablesValues++;
                }
                else
                {
                    Expression newExpr = varExpr.Value.SubstituteVariables(VariableValues);

                    if (newExpr is ConstantExpression newConstant)
                    {
                        VariableValues[varExpr.Key] = newConstant.Value;
                        newVariablesValues++;
                    }
                    else
                    {
                        newVarExpressions[varExpr.Key] = newExpr;
                    }
                }
            }

            Statements.Clear();
            Statements.AddRange(keepStatements);
            Statements.AddRange(newStatements);

            VariableExpressions.Clear();

            if (newStatements.Count > 0 || newVarExpressions.Count > 0)
            {
                foreach (Statement s in newStatements)
                {
                    PrintIfVerbose(s);
                }

                foreach (var varExpr in newVarExpressions)
                {
                    VariableExpressions[varExpr.Key] = varExpr.Value;
                    PrintIfVerbose($"{varExpr.Key} = {varExpr.Value}");
                }

                PrintIfVerbose("--------------------------------");
            }

            return (newStatements.Count > 0 || (newVariablesValues > 0 && (keepStatements.Count > 0 || newVarExpressions.Count > 0)));
        }

        private bool IsUsableVariable(Expression expr, List<string> expressedVariables) =>
            (expr is VariableExpression varExpr && FreeVariables.Contains(varExpr.Name) && !expressedVariables.Contains(varExpr.Name)) ||
            (expr is MultiplicationExpression multiExpr && multiExpr.IsVariableWithCoefficient &&
                FreeVariables.Contains(multiExpr.VariableName) && !expressedVariables.Contains(multiExpr.VariableName));

        private void PrintIfVerbose(string str)
        {
            if (Verbose)
            {
                Console.WriteLine(str);
            }
        }

        private void PrintIfVerbose(object obj) => PrintIfVerbose(obj.ToString());

        public override string ToString()
        {
            const string LEFT_PADDING = "  ";
            string str = string.Empty;

            if (Statements.Count > 0)
            {
                str += "Statements:" + Environment.NewLine +
                    string.Join(Environment.NewLine, Statements.Select(x => $"{LEFT_PADDING}{x}"));
            }

            if (VariableExpressions.Count > 0)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    str += Environment.NewLine + Environment.NewLine;
                }

                str += "Variable expresisons:" + Environment.NewLine +
                    string.Join(Environment.NewLine, VariableExpressions.Select(x => $"{LEFT_PADDING}{x.Key} = {x.Value}"));
            }

            if (VariableValues.Count > 0)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    str += Environment.NewLine + Environment.NewLine;
                }

                str += "Variable values:" + Environment.NewLine +
                    string.Join(Environment.NewLine, VariableValues.Select(x => $"{LEFT_PADDING}{x.Key} = {x.Value}" + (x.Value.Denominator == Fraction.One ? string.Empty : $" = {(double)x.Value:g}")));
            }

            return str;
        }
    }
}
