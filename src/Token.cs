namespace Rubidium
{
    public abstract class Token
    {
        public string StringValue { get; }
        public int Index { get; }

        public Token(string str, int index)
        {
            StringValue = str;
            Index = index;
        }
    }
}
