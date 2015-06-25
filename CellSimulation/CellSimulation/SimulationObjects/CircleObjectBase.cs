using System;
using System.Windows;
using CellSimulation.Analitycs;

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

        public Coordinate2D Center { get { return new Coordinate2D() { X = Position.X + Radius * .5, Y = Position.Y + Radius * .5 }; } }

        public Line Diagonale { get { return new Line { X1 = Position.X, Y1 = Position.Y, X2 = Position.X + Radius, Y2 = Position.Y + Radius }; } }
        public Line InverseDiagonale { get { return new Line { X1 = Position.X + Radius, Y1 = Position.Y, X2 = Position.X, Y2 = Position.Y + Radius }; } }

        public void GenerateMassFromRadius()
        {
            Mass = Math.PI * Radius * Radius * .25;
        }
        public void GenerateRadiusFromMass()
        {
            Radius = 2 * Math.Sqrt(Mass / Math.PI);
        }

        public bool IsColided(CircleObjectBase obj)
        {
            return Coordinate2D.Distance(obj.Center, Center) <= (obj.Radius + Radius) * .5;
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

        public new CircleObjectBase Clone()
        {
            return (CircleObjectBase)base.Clone();
        }
    }
}