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
        public Vector2D Velocity { get; set; }
        public Vector2D Accerelation { get; set; }
        public Coordinate2D Position { get; set; }

        public double Energy { get { return .5 * Mass * Math.Pow(Velocity.Length, 2); } }
        public Vector2D Momentum { get { return Velocity * Mass; } }

        public bool AddObject(Object2DBase obj)
        {
            Velocity = obj.Momentum / Mass;
            Mass += obj.Mass;
            obj.Mass = 0;
            return true;
        }
        public bool DropObject(Object2DBase obj)
        {
            if (obj.Mass > Mass)
                return false;
            else if (obj.Energy > Energy)
                return false;
            else
            {
                Velocity = obj.Momentum / Mass;
                Mass -= obj.Mass;
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
    }
}
