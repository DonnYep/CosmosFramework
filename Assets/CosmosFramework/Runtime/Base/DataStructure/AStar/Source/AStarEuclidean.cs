using System.Collections.Generic;

namespace Cosmos
{
    public class AStarEuclidean : AStar
    {
        public AStarEuclidean(float gridCenterX, float gridCenterY, int xCount, int yCount, float nodeSideLength)
            : base(gridCenterX, gridCenterY, xCount, yCount, nodeSideLength) { }
        protected override int GetDistance(Node a, Node b)
        {
            return GetEuclideanDistance(a, b);
        }
    }
}
