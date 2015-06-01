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
        public double Length { get { return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2)); } }
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
    }
}
