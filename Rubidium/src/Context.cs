using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubidium
{
    /// <summary>
    /// Class representing a set of available statements, variable expressions and variable values.
    /// </summary>
    public class Context
    {
        /// <summary>
        /// Indicates if debugging post-iteration summaries
        /// should be printed at the end of each iteration.
        /// </summary>
        private bool Verbose { get; }
        /// <summary>
        /// Enumerable of all distinct variables used in any of the initial statements.
        /// </summary>
        private IEnumerable<string> Variables { get; }

        /// <summary>
        /// List of currently available (not yer fully processed) statements.
        /// </summary>
        public List<Statement> Statements { get; }
        /// <summary>
        /// List of available expressed variables, which still depend on at least one variable
        /// and therefore cannot be fully evaluated to a constant yet.
        /// </summary>
        public Dictionary<string, Expression> VariableExpressions { get; }
        /// <summary>
        /// List of available constant values of variables.
        /// This variables have been finally bound and will never change.
        /// </summary>
        public Dictionary<string, Fraction> VariableValues { get; }

        /// <summary>
        /// Enumerable of variables, which are still free (not bound to a constant or expression).
        /// The algorithm will try various simplifications in order to express these variables.
        /// </summary>
        private IEnumerable<string> FreeVariables => Variables.Where(x => !VariableExpressions.ContainsKey(x) && !VariableValues.ContainsKey(x));

        /// <summary>
        /// Context class constructor.
        /// Takes initial statements entered by user, extracts variables from them and
        /// prepares the Context for statement evaluation.
        /// </summary>
        /// <param name="initialStatements">Initial statements entered by user.
        /// These statements will be evaluated by the Context.</param>
        /// <param name="verbose">Boolean value indicating if the Context should be verbose.
        /// Being verbose means printing a summary at the end of each iteration of statement evaluation.</param>
        public Context(List<Statement> initialStatements, bool verbose = true)
        {
            Verbose = verbose;
            Variables = initialStatements.GetVariables();

            Statements = new List<Statement>(initialStatements);
            VariableExpressions = new Dictionary<string, Expression>();
            VariableValues = new Dictionary<string, Fraction>();
        }

        public bool PerformIteration()
        {
            int newVariablesValues = 0;
            List<Statement> keepStatements = new List<Statement>();
            List<Statement> newStatements = new List<Statement>();
            List<string> expressedVariables = new List<string>();

            foreach (Statement s in Statements)
            {
                if (!s.Left.ContainsVariables)
                {
                    if (s.Right.ContainsVariables)
                    {
                        newStatements.Add(s.SwappedSides);
                    }
                    else
                    {
                        PrintIfVerbose($"{s} : {(s.Left - s.Right) is ConstantExpression constant && constant.Value.IsZero}");
                    }
                }
                else if (s.Variables.Any(x => VariableValues.ContainsKey(x) || VariableExpressions.ContainsKey(x)))
                {
                    newStatements.Add(s.SubstituteVariables(VariableValues, VariableExpressions));
                }
                else if (s.Left is VariableExpression leftVariable &&
                    IsVariableExpressionRightSide(s.Right, leftVariable.Name))
                {
                    VariableExpressions[leftVariable.Name] = s.Right;
                }
                else if (s.Right is VariableExpression rightVariable)
                {
                    newStatements.Add(s.SwappedSides);
                }
                else if (s.Left is MultiplicationExpression leftMultiplication &&
                    leftMultiplication.IsVariableWithCoefficient &&
                    IsVariableExpressionRightSide(s.Right, leftMultiplication.VariableName))
                {
                    VariableExpressions[leftMultiplication.VariableName] = s.Right / leftMultiplication.Coefficient;
                }
                else if (s.Left is AdditionExpression leftAddition &&
                    (s.Right is AdditionExpression || !leftAddition.Constant.IsZero || leftAddition.VariableParts.Any(x => IsVariableExpressionLeftSide(x, expressedVariables))))
                {
                    if (s.Right is AdditionExpression rightAddition)
                    {
                        Fraction constant = rightAddition.Constant - leftAddition.Constant;
                        List<Expression> variableParts = new List<Expression>();

                        variableParts.AddRange(leftAddition.VariableParts);
                        variableParts.AddRange(rightAddition.VariableParts.Select(x => -x));

                        newStatements.Add(new Statement(
                            AdditionExpression.Build(variableParts),
                            constant
                        ));
                    }
                    else if (!leftAddition.Constant.IsZero)
                    {
                        newStatements.Add(new Statement(
                            AdditionExpression.Build(leftAddition.VariableParts),
                            s.Right - leftAddition.Constant
                        ));
                    }
                    else
                    {
                        Expression usableVar = leftAddition.VariableParts.Find(x => IsVariableExpressionLeftSide(x, expressedVariables));

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
                    if (s.Right is AdditionExpression rightAddition)
                    {
                        newStatements.Add(new Statement(
                            s.Left - AdditionExpression.Build(rightAddition.VariableParts),
                            rightAddition.Constant
                        ));
                    }
                    else
                    {
                        newStatements.Add(new Statement(
                            s.Left - s.Right,
                            ConstantExpression.Zero
                        ));
                    }
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
                    VariableValues[varExpr.Key] = constant;
                    newVariablesValues++;
                }
                else
                {
                    Expression newExpr = varExpr.Value.SubstituteVariables(VariableValues, VariableExpressions);

                    if (newExpr is ConstantExpression newConstant)
                    {
                        VariableValues[varExpr.Key] = newConstant;
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
                    PrintIfVerbose($"{varExpr.Key} = " + varExpr.Value.ToString().StripParentheses());
                }

                PrintIfVerbose("--------------------------------");
            }

            return (newStatements.Count > 0 || (newVariablesValues > 0 && (keepStatements.Count > 0 || newVarExpressions.Count > 0)));
        }

        /// <summary>
        /// Determines if the given expression is a single-variable expression, from which the variable can be expressed.
        /// This must either be a direct variable expression or a multiplication expression containing
        /// a single variable expression a constant coefficient.
        /// The variable must also be free (not bound to expression or value) and it must not have been expressed yet in this iteration.
        /// </summary>
        /// <param name="leftExpr">Left-side expression containing a single variable.</param>
        /// <param name="expressedVariables">Names of variables already expressed in this iteration.</param>
        /// <returns>Returns boolean value indicating if the given expression can be used for expressing a variable.</returns>
        private bool IsVariableExpressionLeftSide(Expression leftExpr, List<string> expressedVariables) =>
            (leftExpr is VariableExpression varExpr && FreeVariables.Contains(varExpr.Name) && !expressedVariables.Contains(varExpr.Name)) ||
            (leftExpr is MultiplicationExpression multiExpr && multiExpr.IsVariableWithCoefficient &&
                FreeVariables.Contains(multiExpr.VariableName) && !expressedVariables.Contains(multiExpr.VariableName));

        /// <summary>
        /// Determines if given expression is a valid variable expression.
        /// This means that the variable to be expression is free (not bound to expression or constant value)
        /// and at the same time the right-side expression does not contain a reference to this variable.
        /// </summary>
        /// <param name="rightExpr">Right-side expression.</param>
        /// <param name="varName">Name of left-side variable to be expressed.</param>
        /// <returns>Return boolean value indicating if specified variable can be expressed using given expression.</returns>
        private bool IsVariableExpressionRightSide(Expression rightExpr, string varName) =>
            FreeVariables.Contains(varName) && !rightExpr.Variables.Contains(varName);

        /// <summary>
        /// Prints given string using Console.WriteLine() if Verbose flag is set.
        /// </summary>
        /// <param name="str">String to print.</param>
        private void PrintIfVerbose(string str)
        {
            if (Verbose)
            {
                Console.WriteLine(str);
            }
        }

        /// <summary>
        /// Overload of verbose printing method to avoid having to call object.ToString().
        /// </summary>
        /// <param name="obj">Object to cast to string and print.</param>
        private void PrintIfVerbose(object obj) => PrintIfVerbose(obj.ToString());

        /// <summary>
        /// Override of object.ToString() method.
        /// Generates a string representation of the current state of Context.
        /// It contains all available statements, variable expressions and variable values in a formatted form.
        /// </summary>
        /// <returns>Returns a string representation of the current state of Context.</returns>
        public override string ToString()
        {
            const string LEFT_PADDING = "  ";
            string str = string.Empty;

            // Add statements.
            if (Statements.Count > 0)
            {
                str += "Statements:" + Environment.NewLine +
                    string.Join(Environment.NewLine, Statements.Select(x => $"{LEFT_PADDING}{x}"));
            }

            // Add variable expressions.
            if (VariableExpressions.Count > 0)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    str += Environment.NewLine + Environment.NewLine;
                }

                str += "Variable expresisons:" + Environment.NewLine +
                    string.Join(Environment.NewLine, VariableExpressions.Select(x => $"{LEFT_PADDING}{x.Key} = " + x.Value.ToString().StripParentheses()));
            }

            // Add variable values.
            if (VariableValues.Count > 0)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    str += Environment.NewLine + Environment.NewLine;
                }

                str += "Variable values:" + Environment.NewLine +
                    string.Join(Environment.NewLine, VariableValues.Select(x => $"{LEFT_PADDING}{x.Key} = " + x.Value.ToString().StripParentheses() + (x.Value.Denominator == Fraction.One ? string.Empty : $" = {(double)x.Value:g}")));
            }

            return str;
        }
    }
}
