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
    }
}
