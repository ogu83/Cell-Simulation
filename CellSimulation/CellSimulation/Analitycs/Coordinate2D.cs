
namespace CellSimulation
{
    public class Coordinate2D : Vector2D
    {
        public Coordinate2D() { }
        public Coordinate2D(Vector2D vector)
            : base(vector.X, vector.Y) { }
        public static double Distance(Coordinate2D p1, Coordinate2D p2) { return (p2 - p1).Length; }
    }
}