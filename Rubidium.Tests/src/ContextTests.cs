using System;
using Xunit;
using Rubidium;

namespace Rubidium.Tests
{
    public static class ContextTests
    {
        [Fact]
        public static void TestLinearEquation()
        {
            Context c = Program.Evaluate("10x - 5x + 10 = -15x + 20");

            Assert.Empty(c.Statements);
            Assert.Empty(c.VariableExpressions);

            Assert.Single(c.VariableValues);
            Assert.True(c.VariableValues.ContainsKey("x"));
            Assert.Equal(new Fraction(1, 2), c.VariableValues["x"]);
        }

        [Fact]
        public static void TestSystemOfLinearEquations()
        {
            Context c = Program.Evaluate("2x + y + 3z = 1; 2x + 6y + 8z = 3; 6x + 8y + 18z = 5");

            Assert.Empty(c.Statements);
            Assert.Empty(c.VariableExpressions);

            Assert.Equal(3, c.VariableValues.Count);

            Assert.True(c.VariableValues.ContainsKey("x"));
            Assert.True(c.VariableValues.ContainsKey("y"));
            Assert.True(c.VariableValues.ContainsKey("z"));

            Assert.Equal(new Fraction(3, 10), c.VariableValues["x"]);
            Assert.Equal(new Fraction(2, 5), c.VariableValues["y"]);
            Assert.Equal(Fraction.Zero, c.VariableValues["z"]);
        }

        [Fact]
        public static void TestQuadraticFormula()
        {
            Context c = Program.Evaluate("(-b + d) / 2a = x1; (-b - d) / 2a = x2; (b^2 - 4 a c)^(1/2) = d; 2 = a; -8 = b; -24 = c");

            Assert.Empty(c.Statements);
            Assert.Empty(c.VariableExpressions);

            Assert.Equal(6, c.VariableValues.Count);

            Assert.True(c.VariableValues.ContainsKey("d"));
            Assert.True(c.VariableValues.ContainsKey("a"));
            Assert.True(c.VariableValues.ContainsKey("b"));
            Assert.True(c.VariableValues.ContainsKey("c"));
            Assert.True(c.VariableValues.ContainsKey("x1"));
            Assert.True(c.VariableValues.ContainsKey("x2"));

            Assert.Equal(16, c.VariableValues["d"]);
            Assert.Equal(2, c.VariableValues["a"]);
            Assert.Equal(-8, c.VariableValues["b"]);
            Assert.Equal(-24, c.VariableValues["c"]);
            Assert.Equal(6, c.VariableValues["x1"]);
            Assert.Equal(-2, c.VariableValues["x2"]);
        }

        [Fact]
        public static void TestFunctionCallSingleArg()
        {
            Context c = Program.Evaluate("x = abs(-42)");

            Assert.Empty(c.Statements);
            Assert.Empty(c.VariableExpressions);

            Assert.Single(c.VariableValues);
            Assert.True(c.VariableValues.ContainsKey("x"));
            Assert.Equal(42, c.VariableValues["x"]);
        }
    }
}
