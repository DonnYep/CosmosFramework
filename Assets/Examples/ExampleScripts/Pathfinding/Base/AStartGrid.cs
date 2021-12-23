using System;
using System.Collections.Generic;
using System.Linq;
namespace Cosmos
{
    /// <summary>
    /// AStart 网格；
    /// </summary>
    public partial class AStartGrid
    {
        Node[,] nodeArray;
        /// <summary>
        /// 单位节点的边长；
        /// </summary>
        public float NodeSideLength { get; private set; }
        /// <summary>
        /// 网格的X位置；
        /// </summary>
        public float GridCenterX { get; private set; }
        /// <summary>
        /// 网格的Y位置；
        /// </summary>
        public float GridCenterY { get; private set; }
        public float GridTop { get; private set; }
        public float GridBottom { get; private set; }
        public float GridLeft { get; private set; }
        public float GridRight { get; private set; }
        /// <summary>
        /// 网格宽；
        /// </summary>
        public float GridWidth { get; private set; }
        /// <summary>
        /// 网格高；
        /// </summary>
        public float GridHeight { get; private set; }
        /// <summary>
        /// 网格X轴节点数量；
        /// </summary>
        public int GridSizeX { get; private set; }
        /// <summary>
        /// 网格Y轴节点数量；
        /// </summary>
        public int GridSizeY { get; private set; }
        public float GridOffsetX { get; private set; }
        public float GridOffsetY { get; private set; }
        public AStartGrid(float gridCenterX, float gridCenterY, int xCount, int yCount, float nodeSideLength)
        {
            GridCenterX = gridCenterX;
            GridCenterY = gridCenterY;

            nodeArray = new Node[xCount, yCount];
            GridSizeX = xCount;
            GridSizeY = yCount;
            NodeSideLength = nodeSideLength;
            GridWidth = xCount * nodeSideLength;
            GridHeight = yCount * nodeSideLength;

            var halfWidth = GridWidth / 2;
            var halfHeight = GridHeight / 2;

            GridOffsetX = gridCenterX - halfWidth;
            GridOffsetY = gridCenterY - halfHeight;

            GridLeft = gridCenterX - halfWidth;
            GridRight = gridCenterX + halfWidth;

            GridTop = gridCenterY + halfHeight;
            GridBottom = gridCenterY - halfHeight;

            CreateNodes(xCount, yCount, nodeSideLength);
        }
        public bool IsOverlapping(float posX, float posY)
        {
            if (posX < GridLeft || posX > GridRight) return false;
            if (posY < GridBottom || posY > GridTop) return false;
            return true;
        }
        public Node GetNode(float posX, float posY)
        {
            if (!IsOverlapping(posX, posY))
                return null;
            var col = (posX - GridOffsetX) / NodeSideLength;
            var row = (posY - GridOffsetY) / NodeSideLength;
            return nodeArray[(int)col, (int)row];
        }
        public IList<Node> FindPath(float srcPosX, float srcPosY, float dstPosX, float dstPosY)
        {
            var srcNode = GetNode(srcPosX, srcPosY);
            var dstNode = GetNode(dstPosX, dstPosY);
            if (srcNode == null || dstNode == null)
                return null;
            List<Node> openList = new List<Node>();
            HashSet<Node> closedList = new HashSet<Node>();
            openList.Add(srcNode);
            while (openList.Count > 0)
            {
                var currentNode = openList[0];
                var openLength = openList.Count;
                for (int i = 1; i < openLength; i++)
                {
                    var openNode = openList[i];
                    if (openNode.FCost < currentNode.FCost || openNode.FCost == currentNode.FCost && openNode.HCost < currentNode.HCost)
                    {
                        currentNode = openNode;
                    }
                }
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if (currentNode == dstNode)
                {
                    return GetFinalPath(srcNode, dstNode);
                }
                var nodeNeighbors = GetNeighboringNodes(currentNode);
                foreach (var neighborNode in nodeNeighbors)
                {
                    if (neighborNode.IsObstacle || closedList.Contains(neighborNode))
                    {
                        continue;
                    }
                    int moveCost = currentNode.GCost + GetManhattanDistance(currentNode, neighborNode);
                    if (moveCost < neighborNode.GCost || !openList.Contains(neighborNode))
                    {
                        neighborNode.GCost = moveCost;
                        neighborNode.HCost = GetManhattanDistance(neighborNode, dstNode);
                        neighborNode.ParentNode = currentNode;
                        if (!openList.Contains(neighborNode))
                        {
                            openList.Add(neighborNode);
                        }
                    }
                }
            }
            return null;
        }
        public IList<Node> GetAllNodes()
        {
            var nodes = new Node[GridSizeX * GridSizeY];
            int idx = 0;
            foreach (var node in nodeArray)
            {
                nodes[idx] = node;
                idx++;
            }
            return nodes;
        }
        public virtual IList<Node> GetNeighboringNodes(float posX, float posY)
        {
            var node = GetNode(posX, posY);
            return GetNeighboringNodes(node);
        }
        /// <summary>
        /// 获取临近的节点；
        /// 默认使用Square方格节点获取；
        /// </summary>
        /// <param name="node">被查找的节点</param>
        /// <returns>目标节点数组</returns>
        public virtual IList<Node> GetNeighboringNodes(Node node)
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
        protected int GetManhattanDistance(Node a, Node b)
        {
            var x = Math.Abs(a.IndexX - b.IndexX);
            var y = Math.Abs(a.IndexY - b.IndexY);
            return (x + y) * 10;
        }
        protected int GetDiagonalDistance(Node a, Node b)
        {
            var x = Math.Abs(a.IndexX - b.IndexX);
            var y = Math.Abs(a.IndexY - b.IndexY);
            if (x > y)
                return 14 * y + 10 * (x - y);
            else
                return 14 * x + 10 * (y - x);
        }
        protected int GetEuclideanDistance(Node a, Node b)
        {
            var x = a.IndexX - b.IndexX;
            var y = a.IndexY - b.IndexY;
            return (int)Math.Floor(Math.Sqrt(x * x + y * y)) * 10;
        }
        protected IList<Node> GetFinalPath(Node src, Node dst)
        {
            List<Node> nodePath = new List<Node>();
            Node currentNode = dst;
            while (currentNode != src)
            {
                nodePath.Add(currentNode);
                currentNode = currentNode.ParentNode;
            }
            nodePath.Reverse();
            return nodePath;
        }
        /// <summary>
        /// 创建节点；
        /// </summary>
        /// <param name="xCount">X轴数量</param>
        /// <param name="yCount">Y轴数量</param>
        /// <param name="nodeSideLength">节点的边长</param>
        protected void CreateNodes(int xCount, int yCount, float nodeSideLength)
        {
            var centerOffsetX = nodeSideLength / 2 + GridOffsetX;
            var centerOffsetY = nodeSideLength / 2 + GridOffsetY;
            //笛卡尔二维坐标系；
            //Cartesian coordinates
            for (int y = 0; y < yCount; y++)
            {
                for (int x = 0; x < xCount; x++)
                {
                    var nodeCenterX = nodeSideLength * x + centerOffsetX;
                    var nodeCenterY = nodeSideLength * y + centerOffsetY;
                    nodeArray[x, y] = new Node(x, y, nodeCenterX, nodeCenterY, nodeSideLength, false);
                }
            }
        }
    }
}
