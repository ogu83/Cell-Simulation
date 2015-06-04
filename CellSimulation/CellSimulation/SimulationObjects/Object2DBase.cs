using System;
using System.Windows;

namespace CellSimulation
{
    public abstract class Object2DBase
    {
        public Object2DBase() { }
        public Object2DBase(int id) { Id = id; }

        public int Id { get; set; }

        public double Mass { get; set; }
        public bool IsExhausted { get { return Mass <= 0; } }
        public Vector2D Velocity { get; set; }
        public Vector2D Accerelation { get; set; }
        public Coordinate2D Position { get; set; }

        public double Energy { get { return .5 * Mass * Math.Pow(Velocity.Length, 2); } }
        public Vector2D Momentum { get { return Velocity * Mass; } }

        public virtual bool AddObject(Object2DBase obj)
        {
            ////Momentum Preserving Universe
            //Velocity = obj.Momentum / Mass;
            //Mass += obj.Mass;

            ////Energy Preserving Universe
            //Velocity = Vector2D.Sqrt(Vector2D.Pow(obj.Velocity, 2) * obj.Mass / Mass);
            var m1 = Mass;
            var m2 = obj.Mass;
            var m = m1 + m2;
            var v1 = Velocity;
            var v2 = obj.Velocity;
            //var v = Vector2D.Sqrt((Vector2D.Pow(v1, 2) * m1 + Vector2D.Pow(v2, 2) * m2) / m);
            ///Calculate Direction ov velocity
            var vd = (v1 * m1 + v2 * m2) / m;
            //if (vd.X < 0) v.X *= -1;
            //if (vd.Y < 0) v.Y *= -1;
            var v = vd;

            Velocity = v;
            Mass = m;
            obj.Mass = 0;
            return true;
        }
        public virtual bool DropObject(Object2DBase obj)
        {
            //Momentum Preserving Universe
            if (obj.Mass > Mass)
                return false;
            //else if (obj.Energy > Energy)
            //    return false;
            else
            {
                var m = Mass;
                var v = Velocity;
                var m2 = obj.Mass;
                var v2 = obj.Velocity;
                var m1 = m - m2;
                var v1 = (v * m - v2 * m2) / m1;
                Velocity = v1;
                Mass = m1;
                return true;
            }
        }

        public override int GetHashCode() { return Id; }
        public override bool Equals(object obj)
        {
            var obj2d = obj as Object2DBase;
            if (obj2d == null)
                return false;
            else
                return obj2d.Id == Id;
        }

        public virtual Rect BoundingBox() { return new Rect(); }

        public static Object2DBase FindBigObject(Object2DBase o1, Object2DBase o2)
        {
            if (o1.Mass > o2.Mass)
                return o1;
            else if (o1.Mass < o2.Mass)
                return o2;
            else if (o1.Energy > o2.Energy)
                return o1;
            else if (o1.Energy < o2.Energy)
                return o2;
            else
                return null;
        }

        public Object2DBase Clone()
        {
            return this.MemberwiseClone() as Object2DBase;
        }

        public virtual void Move()
        {
            Position.X += Velocity.X;
            Position.Y += Velocity.Y;
        }

        public virtual void BoundryCollision(Rect boundry) { }
    }
}
