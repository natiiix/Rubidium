using System.Numerics;

namespace Rubidium
{
    public class IntegerToken : Token
    {
        public BigInteger IntegerValue => BigInteger.Parse(StringValue);

        public IntegerToken(string str, int index) : base(str, index) { }
    }
}
