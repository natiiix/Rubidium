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

        /// <summary>
        /// Performs a single iteration of statement optimization / simplification.
        /// Attempts to express as many variables as possible. Variables can be expressed
        /// using an expression (which means it depends on other variables) or as a constant value.
        /// When a variable is successully expressed, it will be substituted
        /// for its expression / value in other statements and expressions.
        /// </summary>
        /// <returns>Return boolean value indicating if any progress has been made in the iteration.
        /// If no progress has been made in an iteration, it is fair to assume that
        /// no progress would be made in any number of further iterations.</returns>
        public bool PerformIteration()
        {
            // Number of variables bound to a constant value in this iteration.
            int newVariablesValues = 0;
            // Statements to be kept untouched.
            List<Statement> keepStatements = new List<Statement>();
            // New statements created in this iteration.
            List<Statement> newStatements = new List<Statement>();
            // List of names of variables, which have been expressed in this iteration.
            List<string> expressedVariables = new List<string>();

            // Iterate through all available statements.
            foreach (Statement s in Statements)
            {
                // Left side is constant.
                if (!s.Left.ContainsVariables)
                {
                    // Right side is variable.
                    if (s.Right.ContainsVariables)
                    {
                        // Swap left and right side of statement
                        // to move variables to the left side
                        // and constants to the right side.
                        newStatements.Add(s.SwappedSides);
                    }
                    // Both sides of statement are constant.
                    else
                    {
                        // Evaluate the statement. Determine if both sides are equal and print the result.
                        // Then throw the statement away, because it will not be useful for anything else.
                        PrintIfVerbose($"{s} : {(s.Left - s.Right) is ConstantExpression constant && constant.Value.IsZero}");
                    }
                }
                // If the statement contains any variable, which can be substituted,
                // substitute variables with their expressions or constant values.
                else if (s.Variables.Any(x => VariableValues.ContainsKey(x) || VariableExpressions.ContainsKey(x)))
                {
                    newStatements.Add(s.SubstituteVariables(VariableValues, VariableExpressions));
                }
                // Left side is a lone variable, which can be expressed and
                // if the right side is suitable for expressing the left-side variable.
                else if (s.Left is VariableExpression leftVariable &&
                    IsVariableExpressionRightSide(s.Right, leftVariable.Name))
                {
                    VariableExpressions[leftVariable.Name] = s.Right;
                }
                // Right side is a lone variable, swap sides of statement.
                else if (s.Right is VariableExpression rightVariable)
                {
                    newStatements.Add(s.SwappedSides);
                }
                // Left side is a single variable with a const coefficient,
                // the variable can be expressed and the right side of statement
                // is suitable for expression of this variable.
                else if (s.Left is MultiplicationExpression leftMultiplication &&
                    leftMultiplication.IsVariableWithCoefficient &&
                    IsVariableExpressionRightSide(s.Right, leftMultiplication.VariableName))
                {
                    VariableExpressions[leftMultiplication.VariableName] = s.Right / leftMultiplication.Coefficient;
                }
                // Left side is an addition, right side is either an addition or left side contains a constant or
                // one of the variable parts of the left-side addition can be used to express a free variable.
                else if (s.Left is AdditionExpression leftAddition &&
                    (s.Right is AdditionExpression || !leftAddition.Constant.IsZero || leftAddition.VariableParts.Any(x => IsVariableExpressionLeftSide(x, expressedVariables))))
                {
                    // Right side is also an addition.
                    if (s.Right is AdditionExpression rightAddition)
                    {
                        // Aggregate the constant and variable parts of both additions together.
                        Fraction constant = rightAddition.Constant - leftAddition.Constant;
                        List<Expression> variableParts = new List<Expression>();

                        variableParts.AddRange(leftAddition.VariableParts);
                        variableParts.AddRange(rightAddition.VariableParts.Select(x => -x));

                        // Create a new statement with the variable parts on the left side
                        // and the constant expression on the right side.
                        newStatements.Add(new Statement(
                            AdditionExpression.Build(variableParts),
                            constant
                        ));
                    }
                    // Left-side addition contains a constant.
                    else if (!leftAddition.Constant.IsZero)
                    {
                        newStatements.Add(new Statement(
                            AdditionExpression.Build(leftAddition.VariableParts),
                            s.Right - leftAddition.Constant
                        ));
                    }
                    // Otherwise (if a variable from the left-side addition can be expressed).
                    else
                    {
                        // Get the first variable expression, from which a variable can be expressed.
                        Expression usableVar = leftAddition.VariableParts.Find(x => IsVariableExpressionLeftSide(x, expressedVariables));

                        // Move the expression to the left side and the remainder
                        // of the left-side addition to the right side of the statement.
                        newStatements.Add(new Statement(
                            usableVar,
                            s.Right - AdditionExpression.Build(leftAddition.Constant, leftAddition.VariableParts.Where(x => x != usableVar))
                        ));

                        // Add the expressed variable to a list of variables expressed in this iteration.
                        // Even though the variable has not really been expressed yet.
                        // It has just been prepared for being expressed in the next iteration.
                        expressedVariables.Add(usableVar is VariableExpression varExpr ?
                            varExpr.Name :
                            (usableVar as MultiplicationExpression).VariableName
                        );
                    }
                }
                // Right side is variable.
                else if (s.Right.ContainsVariables)
                {
                    // Right side is an addition.
                    if (s.Right is AdditionExpression rightAddition)
                    {
                        // Move variable parts from right side to left side.
                        newStatements.Add(new Statement(
                            s.Left - AdditionExpression.Build(rightAddition.VariableParts),
                            rightAddition.Constant
                        ));
                    }
                    // Right side is not an addition.
                    else
                    {
                        // Move the whole right side of statement to the left side.
                        newStatements.Add(new Statement(
                            s.Left - s.Right,
                            ConstantExpression.Zero
                        ));
                    }
                }
                // Any other type of a statement. Keep the statement because
                // it may become useful later when more variables are substitured.
                else
                {
                    keepStatements.Add(s);
                }
            }

            // Create a temporary dictionary for new variable expressions.
            Dictionary<string, Expression> newVarExpressions = new Dictionary<string, Expression>();

            // Iterate through existing / old variable expressions.
            foreach (var varExpr in VariableExpressions)
            {
                // Variables bound to constant expressions can be turned
                // into variables bound to a constant value.
                if (varExpr.Value is ConstantExpression constant)
                {
                    VariableValues[varExpr.Key] = constant;
                    newVariablesValues++;
                }
                // All other kinds of expressions.
                else
                {
                    // Substitute variables in the expression.
                    Expression newExpr = varExpr.Value.SubstituteVariables(VariableValues, VariableExpressions);

                    // If the resulting expression is a constant expression,
                    // bind the variable to the constant value.
                    if (newExpr is ConstantExpression newConstant)
                    {
                        VariableValues[varExpr.Key] = newConstant;
                        newVariablesValues++;
                    }
                    // Otherwise just bind the variable to the new expression.
                    else
                    {
                        newVarExpressions[varExpr.Key] = newExpr;
                    }
                }
            }

            // Replace old statements with statements to be kept and newly created statements.
            Statements.Clear();
            Statements.AddRange(keepStatements);
            Statements.AddRange(newStatements);

            // Clear old variable expression dictionary.
            VariableExpressions.Clear();

            // If there are any new statements or variable expressions, print them.
            if (newStatements.Count > 0 || newVarExpressions.Count > 0)
            {
                foreach (Statement s in newStatements)
                {
                    PrintIfVerbose(s);
                }

                foreach (var varExpr in newVarExpressions)
                {
                    // Copy the new variable expressions to the persistent variable expression dictionary.
                    VariableExpressions[varExpr.Key] = varExpr.Value;
                    PrintIfVerbose($"{varExpr.Key} = " + varExpr.Value.ToString().StripParentheses());
                }

                PrintIfVerbose("--------------------------------");
            }

            // FIXME: False will be returned if a variable in a variable expression has been substituted by another variable expression.
            // In such case, progress has been made, however it will remain underected and the outer loop will break.
            // This is due to the fact that there is currently no way to determine if two expression are equal.
            // Therefore it is impossible to tell if the variable expression has been simplified / optimized in any way in the iteration.

            // Progress has been made if there is at least one new statement or new expressed variable.
            // If all variables have been already expressed, but there are no statements or variable expressions left,
            // there is no need to keep going because no further progress can ever be made.
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
