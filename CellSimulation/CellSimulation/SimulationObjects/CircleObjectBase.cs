using System;

namespace CellSimulation
{
    public abstract class CircleObjectBase : Object2DBase
    {
        public CircleObjectBase() { }
        public CircleObjectBase(int id, double radius)
            : base(id)
        {
            Radius = radius;
            GenerateMassFromRadius();
        }

        public double Radius { get; set; }

        public void GenerateMassFromRadius()
        {
            Mass = Math.PI * Math.Pow(Radius, 2);
        }
        public void GenerateRadiusFromMass()
        {
            Radius = Math.Sqrt(Mass / Math.PI);
        }

        public bool IsColided(CircleObjectBase obj)
        {
            var totalRadius = obj.Radius + Radius;
            var distance = Coordinate2D.Distance(obj.Position, Position);
            return distance <= totalRadius;
        }
    }
}
