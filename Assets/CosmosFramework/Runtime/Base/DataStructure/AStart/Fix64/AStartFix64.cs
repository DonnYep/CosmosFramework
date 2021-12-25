using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixMath.NET;
namespace Cosmos
{
    public abstract class AStartFix64
    {
        public class Node
        {
            /// <summary>
            /// 网格中X位的序号；
            /// </summary>
            public int IndexX { get; set; }
            /// <summary>
            /// 网格中Y位的序号；
            /// </summary>
            public int IndexY { get; set; }
            /// <summary>
            /// X坐标中心位置
            /// </summary>
            public Fix64 CenterX { get; set; }
            /// <summary>
            /// Y坐标中心位置
            /// </summary>
            public Fix64 CenterY { get; set; }
            /// <summary>
            /// 边长；
            /// </summary>
            public Fix64 SideLength { get; set; }
            /// <summary>
            /// 是否是障碍物；
            /// </summary>
            public bool IsObstacle { get; set; }
            /// <summary>
            /// 父节点；
            /// </summary>
            public Node ParentNode { get; set; }
            /// <summary>
            /// 与初始点的距离；
            /// </summary>
            public int GCost { get; set; }
            /// <summary>
            ///与终点的距离；
            /// </summary>
            public int HCost { get; set; }
            /// <summary>
            /// 距离的代价；
            /// Manhattan or Euclidean distance；
            /// </summary>
            public int FCost { get { return GCost + HCost; } }
            /// <summary>
            /// 当前节点的代价；
            /// </summary>
            public int Cost { get; set; }
            public Node(int indexX, int indexY, Fix64 centerX, Fix64 centerY, Fix64 sideLength, bool isObstacle)
            {
                IndexX = indexX;
                IndexY = indexY;
                CenterX = centerX;
                CenterY = centerY;
                SideLength = sideLength;
                IsObstacle = isObstacle;
            }
        }
        readonly Queue<List<Node>> nodeListQueue = new Queue<List<Node>>();
        /// <summary>
        /// 二维节点数组；
        /// </summary>
        Node[,] nodeArray;
        /// <summary>
        /// 单位节点的边长；
        /// </summary>
        public Fix64 NodeSideLength { get; private set; }
        /// <summary>
        /// 网格的X位置；
        /// </summary>
        public Fix64 GridCenterX { get; private set; }
        /// <summary>
        /// 网格的Y位置；
        /// </summary>
        public Fix64 GridCenterY { get; private set; }
        public Fix64 GridTop { get; private set; }
        public Fix64 GridBottom { get; private set; }
        public Fix64 GridLeft { get; private set; }
        public Fix64 GridRight { get; private set; }
        /// <summary>
        /// 网格宽；
        /// </summary>
        public Fix64 GridWidth { get; private set; }
        /// <summary>
        /// 网格高；
        /// </summary>
        public Fix64 GridHeight { get; private set; }
        /// <summary>
        /// 网格X轴节点数量；
        /// </summary>
        public int GridSizeX { get; private set; }
        /// <summary>
        /// 网格Y轴节点数量；
        /// </summary>
        public int GridSizeY { get; private set; }
        public Fix64 GridOffsetX { get; private set; }
        public Fix64 GridOffsetY { get; private set; }
        public AStartFix64(Fix64 gridCenterX, Fix64 gridCenterY, int xCount, int yCount, Fix64 nodeSideLength)
        {
            GridCenterX = gridCenterX;
            GridCenterY = gridCenterY;

            nodeArray = new Node[xCount, yCount];
            GridSizeX = xCount;
            GridSizeY = yCount;
            NodeSideLength = nodeSideLength;
            GridWidth = (Fix64)(xCount * (float)nodeSideLength);
            GridHeight = (Fix64)(yCount * (float)nodeSideLength);

            Fix64 halfWidth = (Fix64)((float)GridWidth / 2);
            Fix64 halfHeight = (Fix64)((float)GridHeight / 2);

            GridOffsetX = (Fix64)(gridCenterX - halfWidth);
            GridOffsetY = (Fix64)(gridCenterY - halfHeight);

            GridLeft = gridCenterX - halfWidth;
            GridRight = gridCenterX + halfWidth;

            GridTop = gridCenterY + halfHeight;
            GridBottom = gridCenterY - halfHeight;

            CreateNodes(xCount, yCount, nodeSideLength);
        }
        public bool IsOverlapping(Fix64 posX, Fix64 posY)
        {
            if (posX < GridLeft || posX > GridRight) return false;
            if (posY < GridBottom || posY > GridTop) return false;
            return true;
        }
        public Node GetNode(Fix64 posX, Fix64 posY)
        {
            if (!IsOverlapping(posX, posY))
                return null;
            var col = (posX - GridOffsetX) / NodeSideLength;
            var row = (posY - GridOffsetY) / NodeSideLength;
            return nodeArray[(int)col, (int)row];
        }
        public IList<Node> FindPath(Fix64 srcPosX, Fix64 srcPosY, Fix64 dstPosX, Fix64 dstPosY)
        {
            var srcNode = GetNode(srcPosX, srcPosY);
            var dstNode = GetNode(dstPosX, dstPosY);
            if (srcNode == null || dstNode == null)
                return null;
            List<Node> openList = SpawnNodeList();
            List<Node> closedList = SpawnNodeList();
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
                    var finalPath = GetFinalPath(srcNode, dstNode);
                    DespawnNodeList(openList);
                    DespawnNodeList(closedList);
                    return finalPath;
                }
                var nodeNeighbors = GetNeighboringNodes(currentNode);
                foreach (var neighborNode in nodeNeighbors)
                {
                    if (neighborNode.IsObstacle || closedList.Contains(neighborNode))
                    {
                        continue;
                    }
                    int moveCost = currentNode.GCost + GetDistance(currentNode, neighborNode);
                    if (moveCost < neighborNode.GCost || !openList.Contains(neighborNode))
                    {
                        neighborNode.GCost = moveCost;
                        neighborNode.HCost = GetDistance(neighborNode, dstNode);
                        neighborNode.ParentNode = currentNode;
                        if (!openList.Contains(neighborNode))
                        {
                            openList.Add(neighborNode);
                        }
                    }
                }
            }
            DespawnNodeList(openList);
            DespawnNodeList(closedList);
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
        public virtual IList<Node> GetNeighboringNodes(Fix64 posX, Fix64 posY)
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
        public void Clear()
        {
            nodeListQueue.Clear();
        }
        public bool Equals(AStartFix64 other)
        {
            return other.NodeSideLength == NodeSideLength
                && other.GridCenterX == GridCenterX
                && other.GridCenterY == GridCenterY
                && other.GridSizeX == GridSizeX
                && other.GridSizeY == GridSizeY;
        }
        protected abstract int GetDistance(Node a, Node b);
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
            var nodePath = SpawnNodeList();
            Node currentNode = dst;
            while (currentNode != src)
            {
                nodePath.Add(currentNode);
                currentNode = currentNode.ParentNode;
            }
            nodePath.Reverse();
            var arr = nodePath.ToArray();
            DespawnNodeList(nodePath);
            return arr;
        }
        /// <summary>
        /// 创建节点；
        /// </summary>
        /// <param name="xCount">X轴数量</param>
        /// <param name="yCount">Y轴数量</param>
        /// <param name="nodeSideLength">节点的边长</param>
        protected void CreateNodes(int xCount, int yCount, Fix64 nodeSideLength)
        {
            var fNodeSideLength = (float)nodeSideLength;
            var centerOffsetX = fNodeSideLength / 2 + (float)GridOffsetX;
            var centerOffsetY = fNodeSideLength / 2 + (float)GridOffsetY;
            //笛卡尔二维坐标系；
            //Cartesian coordinates
            for (int y = 0; y < yCount; y++)
            {
                for (int x = 0; x < xCount; x++)
                {
                    var nodeCenterX = fNodeSideLength * x + centerOffsetX;
                    var nodeCenterY = fNodeSideLength * y + centerOffsetY;
                    nodeArray[x, y] = new Node(x, y, (Fix64)nodeCenterX, (Fix64)nodeCenterY, nodeSideLength, false);
                }
            }
        }
        List<Node> SpawnNodeList()
        {
            if (nodeListQueue.Count > 0)
            {
                var obj = nodeListQueue.Dequeue();
                return obj;
            }
            else
            {
                var obj = new List<Node>();
                return obj;
            }
        }
        void DespawnNodeList(List<Node> lst)
        {
            if (lst == null)
                return;
            lst.Clear();
            nodeListQueue.Enqueue(lst);
        }
    }
}
