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
        /// <summary>
        /// 十字邻域直接填入复用缓冲，展开节点时零分配；
        /// </summary>
        protected override void FillNeighborNodes(Node node, List<Node> buffer)
        {
            FillCrossNeighborNodes(node, buffer);
        }
        protected override int GetDistance(Node a, Node b)
        {
            return GetManhattanDistance(a, b);
        }
    }
}
