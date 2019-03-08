using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Rubidium
{
    public class FunctionCallExpression : Expression
    {
        public string FunctionName { get; }
        public List<Expression> Arguments { get; }

        public override IEnumerable<string> Variables { get; }

        private FunctionCallExpression(string funcName, List<Expression> args)
        {
            FunctionName = funcName;
            Arguments = args;

            Variables = args.GetVariables();
        }

        public static Expression Build(string funcName, List<Expression> args)
        {
            // If all the arguments are constant expressions,
            // call the function with the given arguments.
            if (args.All(x => x is ConstantExpression))
            {
                return CallFunction(funcName, args.Select(x => x as ConstantExpression).ToList());
            }
            // Otherwise construct a function call expression and wait for the arguments to be evaluated.
            else
            {
                return new FunctionCallExpression(funcName, args.ToList());
            }
        }

        private static ConstantExpression CallFunction(string name, List<ConstantExpression> args)
        {
            // pi(): Returns approximate value of PI converted to Fraction.
            if (name == "pi" && args.Count == 0)
            {
                return (Fraction)Math.PI;
            }
            // e(): Returns approximate value of E converted to Fraction.
            else if (name == "e" && args.Count == 0)
            {
                return (Fraction)Math.E;
            }
            // abs(x): Returns absolute value of argument.
            else if (name == "abs" && args.Count == 1)
            {
                return args[0].Value.AbsoluteValue;
            }
            // round(x): Returns argument value rounded to nearest whole number.
            else if (name == "round" && args.Count == 1)
            {
                return (Fraction)args[0].Value.NearestWholeNumber;
            }
            // approx(x): Returns approximate value of argument.
            else if (name == "approx" && args.Count == 1)
            {
                return args[0].Value.ApproximateValue;
            }
            // sqrt(x): Returns square root of argument.
            else if (name == "sqrt" && args.Count == 1)
            {
                return args[0].Value.SquareRoot;
            }
            // fact(x): Returns factorial of argument.
            // Argument must be whole positive number or zero.
            else if (name == "fact" && args.Count == 1)
            {
                if (args[0].Value.IsWholeNumber && !args[0].Value.Negative)
                {
                    throw new ArgumentException("fact(x) argument must be whole positive number or zero");
                }

                BigInteger result = 1;

                for (BigInteger i = 2; i < args[0].Value.Numerator; i++)
                {
                    result += i;
                }

                return (Fraction)result;
            }
            // sin(x): Returns sine of argument.
            else if (name == "sin" && args.Count == 1)
            {
                return args[0].Value.CallFunction(Math.Sin);
            }
            // cos(x): Returns cosine of argument.
            else if (name == "cos" && args.Count == 1)
            {
                return args[0].Value.CallFunction(Math.Cos);
            }
            // tan(x): Returns tangent of argument.
            else if (name == "tan" && args.Count == 1)
            {
                return args[0].Value.CallFunction(Math.Tan);
            }
            // ln(x): Returns natural logarithm of argument.
            else if (name == "ln" && args.Count == 1)
            {
                return args[0].Value.CallFunction(Math.Log);
            }
            // log(x): Returns base 10 logarithm of argument.
            else if (name == "log" && args.Count == 1)
            {
                return args[0].Value.CallFunction(Math.Log10);
            }
            // log10(x): Returns logarithm with base 10 of argument.
            else if (name == "log10" && args.Count == 1)
            {
                return args[0].Value.CallFunction(Math.Log10);
            }
            // log2(x): Returns logarithm with base 2 of argument.
            else if (name == "log2" && args.Count == 1)
            {
                return args[0].Value.CallFunction(Math.Log2);
            }
            // log(x, y): Returns logarithm with base Y of argument X.
            else if (name == "log" && args.Count == 2)
            {
                return args[0].Value.CallFunction(x => Math.Log(x, (double)args[1].Value));
            }
            // min(x[, ...]): Returns argument with the lowest value.
            else if (name == "min" && args.Count > 0)
            {
                return args.Min(x => x.Value);
            }
            // max(x[, ...]): Returns argument with the greatest value.
            else if (name == "max" && args.Count > 0)
            {
                return args.Max(x => x.Value);
            }
            // sum([...]): Returns sum of arguments.
            else if (name == "sum")
            {
                return Fraction.Sum(args.Select(x => x.Value));
            }
            // mean(x[, ...]): Returns mean value of arguments.
            else if (name == "mean" && args.Count > 0)
            {
                return Fraction.Mean(args.Select(x => x.Value));
            }
            // median(x[, ...]): Returns median of arguments.
            else if (name == "median" && args.Count > 0)
            {
                return Fraction.Median(args.Select(x => x.Value));
            }

            throw new NotImplementedException($"Function call not implemented: function \"{name}\" with {args.Count} arguments");
        }

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues, Dictionary<string, Expression> variableExpressions) =>
            Build(FunctionName, Arguments.Select(x => x.SubstituteVariables(variableValues, variableExpressions)).ToList());

        public override string ToString() => $"{FunctionName}({string.Join(", ", Arguments)})";
    }
}
