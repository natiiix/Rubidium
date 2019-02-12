using System;
using Xunit;
using Rubidium;

namespace Rubidium.Tests
{
    public static class FunctionTests
    {
        [Fact]
        public static void TestFunctionAbs()
        {
            Context c = Program.Evaluate("x = abs(-42); y = abs(12.34)");

            Assert.Empty(c.Statements);
            Assert.Empty(c.VariableExpressions);

            Assert.Equal(2, c.VariableValues.Count);

            Assert.True(c.VariableValues.ContainsKey("x"));
            Assert.Equal(42, c.VariableValues["x"]);

            Assert.True(c.VariableValues.ContainsKey("y"));
            Assert.Equal((Fraction)12.34, c.VariableValues["y"]);
        }

        [Fact]
        public static void TestFunctionSqrt()
        {
            Context c = Program.Evaluate("x = sqrt(65536); y = sqrt(0.01)");

            Assert.Empty(c.Statements);
            Assert.Empty(c.VariableExpressions);

            Assert.Equal(2, c.VariableValues.Count);

            Assert.True(c.VariableValues.ContainsKey("x"));
            Assert.Equal(256, c.VariableValues["x"]);

            Assert.True(c.VariableValues.ContainsKey("y"));
            Assert.Equal((Fraction)0.1, c.VariableValues["y"]);
        }

        [Fact]
        public static void TestFunctionMin()
        {
            Context c = Program.Evaluate("x = min(10, -20, 42, -785); y = min(0.12)");

            Assert.Empty(c.Statements);
            Assert.Empty(c.VariableExpressions);

            Assert.Equal(2, c.VariableValues.Count);

            Assert.True(c.VariableValues.ContainsKey("x"));
            Assert.Equal(-785, c.VariableValues["x"]);

            Assert.True(c.VariableValues.ContainsKey("y"));
            Assert.Equal((Fraction)0.12, c.VariableValues["y"]);
        }

        [Fact]
        public static void TestFunctionMax()
        {
            Context c = Program.Evaluate("x = max(10, -20, 42, -785); y = max(0.12)");

            Assert.Empty(c.Statements);
            Assert.Empty(c.VariableExpressions);

            Assert.Equal(2, c.VariableValues.Count);

            Assert.True(c.VariableValues.ContainsKey("x"));
            Assert.Equal(42, c.VariableValues["x"]);

            Assert.True(c.VariableValues.ContainsKey("y"));
            Assert.Equal((Fraction)0.12, c.VariableValues["y"]);
        }

        [Fact]
        public static void TestFunctionSum()
        {
            Context c = Program.Evaluate("x = sum(-20, -10, 0, 0.0, 10, 20, 30); y = sum(); z = sum(42.42)");

            Assert.Empty(c.Statements);
            Assert.Empty(c.VariableExpressions);

            Assert.Equal(3, c.VariableValues.Count);

            Assert.True(c.VariableValues.ContainsKey("x"));
            Assert.Equal(30, c.VariableValues["x"]);

            Assert.True(c.VariableValues.ContainsKey("y"));
            Assert.Equal(Fraction.Zero, c.VariableValues["y"]);

            Assert.True(c.VariableValues.ContainsKey("z"));
            Assert.Equal((Fraction)42.42, c.VariableValues["z"]);
        }

        [Fact]
        public static void TestFunctionAvg()
        {
            Context c = Program.Evaluate("x = avg(-20, -10, 0, 0.0, 10, 20, 30); y = avg(42.42, -42.42); z = avg(42.42)");

            Assert.Empty(c.Statements);
            Assert.Empty(c.VariableExpressions);

            Assert.Equal(3, c.VariableValues.Count);

            Assert.True(c.VariableValues.ContainsKey("x"));
            Assert.Equal(new Fraction(30, 7), c.VariableValues["x"]);

            Assert.True(c.VariableValues.ContainsKey("y"));
            Assert.Equal(Fraction.Zero, c.VariableValues["y"]);

            Assert.True(c.VariableValues.ContainsKey("z"));
            Assert.Equal((Fraction)42.42, c.VariableValues["z"]);
        }
    }
}
