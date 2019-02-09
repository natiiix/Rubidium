using System;
using Xunit;
using Rubidium;
using System.Collections.Generic;

namespace Rubidium.Tests
{
    public static class LexerTests
    {
        [Fact]
        public static void TestFloat()
        {
            List<Token> tokens = Lexer.Tokenize("x = 3.14159 - 123456.7890");

            Assert.Equal(5, tokens.Count);

            Assert.IsType<SymbolToken>(tokens[0]);
            Assert.Equal("x", (tokens[0] as SymbolToken).StringValue);

            Assert.IsType<SpecialToken>(tokens[1]);
            Assert.True((tokens[1] as SpecialToken).Equality);

            Assert.IsType<NumberToken>(tokens[2]);
            Assert.Equal((Fraction)3.14159, (tokens[2] as NumberToken).NumericValue);

            Assert.IsType<SpecialToken>(tokens[3]);
            Assert.True((tokens[3] as SpecialToken).Subtraction);

            Assert.IsType<NumberToken>(tokens[4]);
            Assert.Equal((Fraction)123456.7890, (tokens[4] as NumberToken).NumericValue);
        }
    }
}
