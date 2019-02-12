using System;
using System.Collections.Generic;
using System.Linq;

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
            // abs(x): Returns absolute value of argument.
            if (name == "abs" && args.Count == 1)
            {
                return args[0].Value.AbsoluteValue;
            }
            // sqrt(x): Returns square root of argument.
            else if (name == "sqrt" && args.Count == 1)
            {
                return args[0].Value.SquareRoot;
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

            throw new NotImplementedException($"Function call not implemented: function \"{name}\" with {args.Count} arguments");
        }

        public override Expression SubstituteVariables(Dictionary<string, Fraction> variableValues, Dictionary<string, Expression> variableExpressions) =>
            Build(FunctionName, Arguments.Select(x => x.SubstituteVariables(variableValues, variableExpressions)).ToList());
    }
}
