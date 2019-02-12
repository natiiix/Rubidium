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
            Context c = Program.Evaluate("x = abs(-42)");

            Assert.Empty(c.Statements);
            Assert.Empty(c.VariableExpressions);

            Assert.Single(c.VariableValues);
            Assert.True(c.VariableValues.ContainsKey("x"));
            Assert.Equal(42, c.VariableValues["x"]);
        }

        [Fact]
        public static void TestFunctionMax()
        {
            Context c = Program.Evaluate("x = max(10, -20, 42, -785)");

            Assert.Empty(c.Statements);
            Assert.Empty(c.VariableExpressions);

            Assert.Single(c.VariableValues);
            Assert.True(c.VariableValues.ContainsKey("x"));
            Assert.Equal(42, c.VariableValues["x"]);
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
    }
}
