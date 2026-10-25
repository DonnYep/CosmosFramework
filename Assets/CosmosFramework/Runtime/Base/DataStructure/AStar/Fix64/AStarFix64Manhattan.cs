using FixMath.NET;
using System;
using System.Collections.Generic;
namespace Cosmos
{
    public class AStarFix64Manhattan : AStarFix64
    {
        public AStarFix64Manhattan(Fix64 gridCenterX, Fix64 gridCenterY, int xCount, int yCount, Fix64 nodeSideLength)
            : base(gridCenterX, gridCenterY, xCount, yCount, nodeSideLength) { }
        public override IList<Node> GetNeighboringNodes(Node node)
        {
            var nodeList = SpawnNodeList();
            FillNeighboringNodes(node, nodeList);
            var dstNodes = nodeList.ToArray();
            DespawnNodeList(nodeList);
            return dstNodes;
        }
        /// <summary>
        /// 十字邻域直接填入复用缓冲，展开节点时零分配；
        /// </summary>
        protected override void FillNeighboringNodes(Node node, List<Node> buffer)
        {
            buffer.Clear();
            if (node == null)
                return;
            var x = node.IndexX;
            var y = node.IndexY;
            if (x - 1 >= 0)
                buffer.Add(nodeArray[x - 1, y]);
            if (x + 1 <= GridSizeX - 1)
                buffer.Add(nodeArray[x + 1, y]);
            if (y + 1 <= GridSizeY - 1)
                buffer.Add(nodeArray[x, y + 1]);
            if (y - 1 >= 0)
                buffer.Add(nodeArray[x, y - 1]);
        }
        protected override int GetDistance(Node a, Node b)
        {
            return GetManhattanDistance(a, b);
        }
    }
}
