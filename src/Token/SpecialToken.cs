namespace Rubidium
{
    public class SpecialToken : Token
    {
        public bool Terminator => StringValue == ";";
        public bool ParameterSeparator => StringValue == ",";
        public bool Equality => StringValue == "=";
        public bool LeftParenthesis => StringValue == "(";
        public bool RightParenthesis => StringValue == ")";
        public bool Addition => StringValue == "+";
        public bool Subtraction => StringValue == "-";
        public bool Multiplication => StringValue == "*";
        public bool Division => StringValue == "/";
        public bool Power => StringValue == "^";

        public SpecialToken(string str, int index) : base(str, index) { }
    }
}
