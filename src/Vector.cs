using System;
using System.Linq;

namespace Rubidium
{
    public class Vector
    {
        public Fraction[] Values { get; }

        public int Size => Values.Length;
        public bool IsZero => Values.All(x => x.IsZero);

        public Fraction this[int index]
        {
            get => Values[index];
            set => Values[index] = value;
        }

        public Vector(params Fraction[] values)
        {
            Values = values;
        }

        public static Vector operator -(Vector v)
        {
            Fraction[] values = new Fraction[v.Size];

            for (int i = 0; i < v.Size; i++)
            {
                values[i] = -v[i];
            }

            return new Vector(values);
        }

        public static Vector operator +(Vector first, Vector second)
        {
            if (first.Size != second.Size)
            {
                throw new ArgumentException();
            }

            Fraction[] values = new Fraction[first.Size];

            for (int i = 0; i < first.Size; i++)
            {
                values[i] = first[i] + second[i];
            }

            return new Vector(values);
        }

        public static Vector operator -(Vector first, Vector second) => first + -second;

        public static Vector operator *(Vector v, Fraction f)
        {
            Fraction[] values = new Fraction[v.Size];

            for (int i = 0; i < v.Size; i++)
            {
                values[i] = v[i] * f;
            }

            return new Vector(values);
        }

        public static Vector operator /(Vector v, Fraction f) => v * ~f;
    }
}
