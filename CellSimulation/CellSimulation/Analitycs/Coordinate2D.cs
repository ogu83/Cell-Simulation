
namespace CellSimulation
{
    public class Coordinate2D : Vector2D
    {
        public static double Distance(Coordinate2D p1, Coordinate2D p2) { return (p2 - p1).Length; }
    }
}