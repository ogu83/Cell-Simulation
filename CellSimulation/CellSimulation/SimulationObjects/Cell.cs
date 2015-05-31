
namespace CellSimulation
{
    public class Cell : CircleObjectBase
    {
        public static Cell CollisionResult(Cell c1, Cell c2)
        {
            var big = FindBigObject(c1, c2);
            if (big == null)
                return null;
            if (big.AddObject(c2))
                return (Cell)big;
            else
                return null;
        }
    }
}
