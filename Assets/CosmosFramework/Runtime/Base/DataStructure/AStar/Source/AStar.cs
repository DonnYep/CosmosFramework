using System;
using System.Collections.Generic;
namespace Cosmos
{
    /// <summary>
    /// AStart 网格；
    /// </summary>
    public abstract class AStar : IEquatable<AStar>
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
            public float CenterX { get; set; }
            /// <summary>
            /// Y坐标中心位置
            /// </summary>
            public float CenterY { get; set; }
            /// <summary>
            /// 边长；
            /// </summary>
            public float SideLength { get; set; }
            /// <summary>
            /// 是否是障碍物；
            /// </summary>
            public bool IsObstacle { get; set; }
            /// <summary>
            /// 父节点；
            /// </summary>
            public Node ParentNode { get; set; }
            /// <summary>
            /// 从起点到currentNode再到neighbourNode的距离=gCost
            /// </summary>
            public int GCost { get; set; }
            /// <summary>
            ///网格到targetNode的距离
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
            public Node(int indexX, int indexY, float centerX, float centerY, float sideLength, bool isObstacle)
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
        protected Node[,] nodeArray;
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
        public AStar(float gridCenterX, float gridCenterY, int xCount, int yCount, float nodeSideLength)
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
            //var dstNode = GetObstacleNearsetNode(dstPosX, dstPosY);
            if (srcNode == null || dstNode == null)
                return null;
            List<Node> openList = SpawnNodeList();
            List<Node> closedList = SpawnNodeList();
            openList.Add(srcNode);
            while (openList.Count > 0)
            {
                var currentNode = openList[0];
                var openLength = openList.Count;
                for (int i = 0; i < openLength; i++)
                {
                    var openNode = openList[i];
                    if (openNode.FCost <= currentNode.FCost && openNode.HCost < currentNode.HCost)
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
                    if (closedList.Contains(neighborNode))
                        continue;
                    if (neighborNode.IsObstacle)
                    {
                        closedList.Add(neighborNode);
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
        public IList<Node> GetNeighboringNodes(float posX, float posY)
        {
            var node = GetNode(posX, posY);
            return GetNeighboringNodes(node);
        }
        /// <summary>
        /// 获取临近除自身的九宫格节点；
        /// </summary>
        /// <param name="node">被查找的节点</param>
        /// <returns>目标节点数组</returns>
        public virtual IList<Node> GetNeighboringNodes(Node node)
        {
            return GetCrossNeighboringNodes(node);
        }
        public virtual void Clear()
        {
            nodeArray = new Node[0, 0];
            nodeListQueue.Clear();
        }
        public bool Equals(AStar other)
        {
            return other.NodeSideLength == NodeSideLength
                && other.GridCenterX == GridCenterX
                && other.GridCenterY == GridCenterY
                && other.GridSizeX == GridSizeX
                && other.GridSizeY == GridSizeY;
        }
        protected abstract int GetDistance(Node a, Node b);
        protected virtual int GetManhattanDistance(Node a, Node b)
        {
            var x = Math.Abs(a.IndexX - b.IndexX);
            var y = Math.Abs(a.IndexY - b.IndexY);
            return (x + y) * 10;
        }
        protected virtual int GetDiagonalDistance(Node a, Node b)
        {
            var x = Math.Abs(a.IndexX - b.IndexX);
            var y = Math.Abs(a.IndexY - b.IndexY);
            if (x > y)
                return 14 * y + 10 * (x - y);
            else
                return 14 * x + 10 * (y - x);
        }
        protected virtual int GetEuclideanDistance(Node a, Node b)
        {
            var x = Math.Abs(a.IndexX - b.IndexX);
            var y = Math.Abs(a.IndexY - b.IndexY);
            return (int)Math.Ceiling(Math.Sqrt(x * x + y * y) * 14);
        }
        /// <summary>
        /// 十字形临近结点
        /// </summary>
        protected IList<Node> GetCrossNeighboringNodes(Node node)
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
        /// <summary>
        /// 九宫格临近结点
        /// </summary>
        protected IList<Node> GetNineNeighboringNodes(Node node)
        {
            var w = GridSizeX - 1;
            var h = GridSizeY - 1;
            Node[] srcNodes = new Node[8];
            int idx = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (y == 0 && x == 0)
                        continue;
                    int idxX = node.IndexX + x;
                    int idxY = node.IndexY + y;
                    if (idxX <= w && idxX >= 0 && idxY <= h && idxY >= 0)
                    {
                        srcNodes[idx++] = nodeArray[idxX, idxY];
                    }
                }
            }
            var dstNodes = new Node[idx];
            Array.Copy(srcNodes, 0, dstNodes, 0, idx);
            return dstNodes;
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
        protected virtual Node GetObstacleNearsetNode(float posX, float posY)
        {
            var node = GetNode(posX, posY);
            if (!node.IsObstacle)
                return node;
            List<Node> openList = SpawnNodeList();
            Node nearsetNode = null;
            openList.Add(node);
            while (openList.Count > 0)
            {
                var currentNode = openList[0];
                var nodeNeighbors = GetCrossNeighboringNodes(currentNode);
                openList.Remove(currentNode);
                foreach (var neighborNode in nodeNeighbors)
                {
                    if (neighborNode.IsObstacle)
                    {
                        if (!openList.Contains(neighborNode))
                        {
                            openList.Add(neighborNode);
                        }
                    }
                    else
                    {
                        //判断节点是否是被obstacle包围
                        nearsetNode = neighborNode;
                        break;
                    }
                }
                if (nearsetNode != null)
                    break;
            }
            DespawnNodeList(openList);
            return nearsetNode;
        }
        protected List<Node> SpawnNodeList()
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
        protected void DespawnNodeList(List<Node> lst)
        {
            if (lst == null)
                return;
            lst.Clear();
            nodeListQueue.Enqueue(lst);
        }
    }
}
