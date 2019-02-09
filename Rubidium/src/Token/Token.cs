namespace Rubidium
{
    /// <summary>
    /// Abstract class that encapsulates all token types.
    /// </summary>
    public abstract class Token
    {
        /// <summary>
        /// Original string from which the token was constructed.
        /// </summary>
        public string StringValue { get; }
        /// <summary>
        /// Index in the original query string at which the token was parsed.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Base token class constructor, which sets the StringValue and Index properties.
        /// </summary>
        /// <param name="str">Token string.</param>
        /// <param name="index">Index of token in query string.</param>
        public Token(string str, int index)
        {
            StringValue = str;
            Index = index;
        }

        /// <summary>
        /// Converts the token into a string representation.
        /// The best representation of a token is its string value,
        /// since that is the string from which the token has been parsed.
        /// </summary>
        /// <returns>Returns the StringValue property of the token.</returns>
        public override string ToString() => StringValue;
    }
}
