namespace Rubidium
{
    public class Statement
    {
        public Expression Left { get; }
        public Expression Right { get; }

        public Statement(Expression left, Expression right)
        {
            Left = left;
            Right = right;
        }

        public override string ToString() => $"{Left} = {Right}";
    }
}
