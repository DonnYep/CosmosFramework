using System.Collections.Generic;

namespace Cosmos
{
    public class AStarManhattan : AStar
    {
        public AStarManhattan(float gridCenterX, float gridCenterY, int xCount, int yCount, float nodeSideLength)
            : base(gridCenterX, gridCenterY, xCount, yCount, nodeSideLength) { }
        public override IList<Node> GetNeighborNodes(Node node)
        {
            return GetCrossNeighborNodes(node);
        }
        protected override int GetDistance(Node a, Node b)
        {
            return GetManhattanDistance(a, b);
        }
    }
}
