namespace Cosmos
{
    public class AStartEuclidean : AStart
    {
        public AStartEuclidean(float gridCenterX, float gridCenterY, int xCount, int yCount, float nodeSideLength)
            : base(gridCenterX, gridCenterY, xCount, yCount, nodeSideLength){}
        protected override int GetDistance(Node a, Node b)
        {
            return GetEuclideanDistance(a, b);
        }
    }
}
