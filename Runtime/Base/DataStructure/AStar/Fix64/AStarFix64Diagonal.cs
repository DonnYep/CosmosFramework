using FixMath.NET;
namespace Cosmos
{
    public class AStarFix64Diagonal : AStarFix64
    {
        public AStarFix64Diagonal(Fix64 gridCenterX, Fix64 gridCenterY, int xCount, int yCount, Fix64 nodeSideLength)
            : base(gridCenterX, gridCenterY, xCount, yCount, nodeSideLength) { }
        protected override int GetDistance(Node a, Node b)
        {
            return GetDiagonalDistance(a, b);
        }
    }
}
