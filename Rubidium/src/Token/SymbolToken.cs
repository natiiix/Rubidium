namespace Rubidium
{
    /// <summary>
    /// Class representing a single symbol (e.g., variable name).
    /// </summary>
    public class SymbolToken : Token
    {
        /// <summary>
        /// SymbolToken constructor, which just calls the base Token constructor.
        /// </summary>
        /// <param name="str">Token string. This is the value of the symbol.</param>
        /// <param name="index">Index of token in query string.</param>
        public SymbolToken(string str, int index) : base(str, index) { }
    }
}
