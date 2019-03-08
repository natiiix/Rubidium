namespace Rubidium
{
    /// <summary>
    /// Class representing special tokens (operators, statement terminator, etc.).
    /// Properties indicate the type of the special token.
    /// </summary>
    public class SpecialToken : Token
    {
        public bool Terminator => StringValue == ";";
        public bool ArgumentSeparator => StringValue == ",";
        public bool Equality => StringValue == "=";
        public bool LeftParenthesis => StringValue == "(";
        public bool RightParenthesis => StringValue == ")";
        public bool Addition => StringValue == "+";
        public bool Subtraction => StringValue == "-";
        public bool Multiplication => StringValue == "*";
        public bool Division => StringValue == "/";
        public bool Power => StringValue == "^";
        public bool Derivative => StringValue == "'";

        /// <summary>
        /// SpecialToken constructor, which just calls the base Token constructor.
        /// </summary>
        /// <param name="str">Token string, used to determine the type of special token.</param>
        /// <param name="index">Index of token in query string.</param>
        public SpecialToken(string str, int index) : base(str, index) { }
    }
}
