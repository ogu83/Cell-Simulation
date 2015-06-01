using System;
using System.Windows;

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


        public override bool AddObject(Object2DBase obj)
        {
            var result = base.AddObject(obj);
            GenerateRadiusFromMass();
            return result;
        }

        public override bool DropObject(Object2DBase obj)
        {
            var result = base.DropObject(obj);
            GenerateRadiusFromMass();
            return result;
        }

        public override void Move()
        {
            base.Move();
        }

        public override void BoundryCollision(Rect boundry)
        {
            if (Position.X <= boundry.Left || Position.X + Radius >= boundry.Right)
                Velocity.X *= -1;
            if (Position.Y <= boundry.Top || Position.Y + Radius >= boundry.Bottom)
                Velocity.Y *= -1;
        }
    }
}