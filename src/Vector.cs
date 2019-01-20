namespace Rubidium
{
    public class Vector
    {
        public Fraction[] Values { get; }

        public int Size => Values.Length;

        public Fraction this[int index]
        {
            get => Values[index];
            set => Values[index] = value;
        }

        public Vector(params Fraction[] values)
        {
            Values = values;
        }
    }
}
