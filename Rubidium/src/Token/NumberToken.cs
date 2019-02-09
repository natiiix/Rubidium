using System.Numerics;

namespace Rubidium
{
    public class NumberToken : Token
    {
        public Fraction NumericValue => Fraction.Parse(StringValue);

        public NumberToken(string str, int index) : base(str, index) { }
    }
}
