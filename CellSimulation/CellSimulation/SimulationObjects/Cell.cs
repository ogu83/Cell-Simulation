
namespace CellSimulation
{   
    public class Cell : CircleObjectBase
    {
        public enum FilterType { Mass, Energy, Momentum, Velocity }

        public static Cell CollisionResult(Cell c1, Cell c2)
        {
            var big = FindBigObject(c1, c2);
            if (big == null)
                return null;
            if (big.AddObject(c1 == big ? c2 : c1))
                return ((Cell)big);
            else
                return null;
        }

        public override bool AddObject(Object2DBase obj)
        {
            var result = base.AddObject(obj);
            Texture = null;
            return result;
        }

        public override bool DropObject(Object2DBase obj)
        {
            var result = base.DropObject(obj);
            Texture = null;
            return result;
        }

        public object Texture { get; set; }

    }
}
