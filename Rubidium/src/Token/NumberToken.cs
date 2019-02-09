using System.Numerics;

namespace Rubidium
{
    public class NumberToken : Token
    {
        public Fraction NumericValue { get; }

        public NumberToken(string str, int index) : base(str, index)
        {
            NumericValue = Fraction.Parse(StringValue);
        }
    }
}
