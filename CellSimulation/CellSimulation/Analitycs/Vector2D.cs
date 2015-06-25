using System;
using System.Windows;

namespace CellSimulation
{
    public class Vector2D
    {
        public Vector2D() { }
        public Vector2D(double x, double y)
            : this()
        {
            X = x;
            Y = y;
        }
        public double X { get; set; }
        public double Y { get; set; }
        public double Length { get { return Math.Sqrt(X * X + Y * Y); } }
        public Vector2D UnitVector { get { return new Vector2D(X / Length, Y / Length); } }

        public Vector2D Subtract(Vector2D v) { return new Vector2D(X - v.X, Y - v.Y); }
        public Vector2D Add(Vector2D v) { return new Vector2D(X + v.X, Y + v.Y); }
        public Vector2D Multiple(double value) { return new Vector2D(X * value, Y * value); }
        public Vector2D Divide(double value) { return new Vector2D(X / value, Y / value); }
        public static Vector2D operator -(Vector2D v1, Vector2D v2) { return v1.Subtract(v2); }
        public static Vector2D operator +(Vector2D v1, Vector2D v2) { return v1.Add(v2); }
        public static Vector2D operator *(Vector2D v1, double v) { return v1.Multiple(v); }
        public static Vector2D operator /(Vector2D v1, double v) { return v1.Divide(v); }

        public Point Point { get { return new Point(X, Y); } }

        public override bool Equals(object obj)
        {
            if (obj as Vector2D == null)
                return false;
            else
                return (obj as Vector2D).X == X && (obj as Vector2D).X == Y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[X:{0}|Y:{1}]|L:{2}", X, Y, Length);
        }

        public static Vector2D Pow(Vector2D vector, double scalar)
        {
            return new Vector2D(Math.Pow(vector.X, scalar), Math.Pow(vector.Y, scalar));
        }

        public static Vector2D Sqrt(Vector2D vector)
        {
            return new Vector2D(Math.Sqrt(vector.X), Math.Sqrt(vector.Y));
        }

        public Vector2D Clone()
        {
            return (Vector2D)MemberwiseClone();
        }
    }
}