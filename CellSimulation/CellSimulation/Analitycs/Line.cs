using System.Windows;

namespace CellSimulation.Analitycs
{
    public class Line
    {
        public Line() { }
        public Line(double x1, double x2, double y1, double y2)
            : this()
        {
            X1 = x1;
            X2 = x2;
            Y1 = y1;
            Y2 = y2;
        }
        public Line(Point p1, Point p2)
            : this(p1.X, p2.X, p1.Y, p2.Y) { }

        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }

        public Point P1 { get { return new Point(X1, Y1); } }
        public Point P2 { get { return new Point(X1, Y2); } }
        public double Length { get { return Geometry.Distance(P1, P2); } }
    }
}