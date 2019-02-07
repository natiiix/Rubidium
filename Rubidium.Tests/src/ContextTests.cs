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
    }
}
