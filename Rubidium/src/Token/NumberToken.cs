namespace Rubidium
{
    /// <summary>
    /// Class representing a token with a literal numeric value.
    /// </summary>
    public class NumberToken : Token
    {
        /// <summary>
        /// Numeric value of the token.
        /// </summary>
        public Fraction NumericValue { get; }

        /// <summary>
        /// NumberToken constructor, which calls the base Token constructor
        /// and then parses the numeric value.
        /// </summary>
        /// <param name="str">Token string, which will be parsed to get the numeric value.</param>
        /// <param name="index">Index of token in query string.</param>
        public NumberToken(string str, int index) : base(str, index)
        {
            NumericValue = Fraction.Parse(StringValue);
        }
    }
}
