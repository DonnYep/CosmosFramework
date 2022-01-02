using FixMath.NET;
using System;
using System.Collections.Generic;
namespace Cosmos
{
    public class AStartFix64Manhattan : AStartFix64
    {
        public AStartFix64Manhattan(Fix64 gridCenterX, Fix64 gridCenterY, int xCount, int yCount, Fix64 nodeSideLength)
            : base(gridCenterX, gridCenterY, xCount, yCount, nodeSideLength) { }
        public override IList<Node> GetNeighboringNodes(Node node)
        {
            Node[] srcNodes = new Node[4];
            var x = node.IndexX;
            var y = node.IndexY;
            var leftX = x - 1;
            var rightX = x + 1;
            var upY = y + 1;
            var downY = y - 1;
            int idx = 0;
            if (leftX >= 0)
                srcNodes[idx++] = nodeArray[leftX, y];
            if (rightX <= GridSizeX - 1)
                srcNodes[idx++] = nodeArray[rightX, y];
            if (upY <= GridSizeY - 1)
                srcNodes[idx++] = nodeArray[x, upY];
            if (downY >= 0)
                srcNodes[idx++] = nodeArray[x, downY];
            var dstNodes = new Node[idx];
            Array.Copy(srcNodes, 0, dstNodes, 0, idx);
            return dstNodes;
        }
        protected override int GetDistance(Node a, Node b)
        {
            return GetManhattanDistance(a, b);
        }
    }
}
