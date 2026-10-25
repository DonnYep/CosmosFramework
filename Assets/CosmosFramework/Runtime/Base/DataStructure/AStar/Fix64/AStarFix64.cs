using System;
using System.Collections.Generic;
using FixMath.NET;
namespace Cosmos
{
    public abstract class AStarFix64
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
        /// 开/闭集缓冲队列；
        /// 用 HashSet 做 O(1) 的归属判定，避免 List.Contains 的全表线性查找。
        /// </summary>
        readonly Queue<HashSet<Node>> nodeSetQueue = new Queue<HashSet<Node>>();
        /// <summary>
        /// 二维节点数组；
        /// </summary>
        protected Node[,] nodeArray;
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
        public AStarFix64(Fix64 gridCenterX, Fix64 gridCenterY, int xCount, int yCount, Fix64 nodeSideLength)
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

            GridOffsetX = gridCenterX - halfWidth;
            GridOffsetY = gridCenterY - halfHeight;

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
            //空网格 / 已 Clear：直接判空，避免夹取后索引空数组
            if (GridSizeX <= 0 || GridSizeY <= 0)
                return null;
            if (!IsOverlapping(posX, posY))
                return null;
            int col = (int)((posX - GridOffsetX) / NodeSideLength);
            int row = (int)((posY - GridOffsetY) / NodeSideLength);
            //重叠判定为闭区间，右/上边界会算到数组外，需夹到最后一个格子
            if (col < 0) col = 0;
            else if (col >= GridSizeX) col = GridSizeX - 1;
            if (row < 0) row = 0;
            else if (row >= GridSizeY) row = GridSizeY - 1;
            return nodeArray[col, row];
        }
        public IList<Node> FindPath(Fix64 srcPosX, Fix64 srcPosY, Fix64 dstPosX, Fix64 dstPosY)
        {
            var srcNode = GetNode(srcPosX, srcPosY);
            var dstNode = GetNode(dstPosX, dstPosY);
            if (srcNode == null || dstNode == null)
                return null;
            var openList = SpawnNodeList();
            //邻居列表实例级复用：每次展开 Clear 后重填，不再 ToArray
            var neighborBuffer = SpawnNodeList();
            //openSet/closedSet 只做 O(1) 归属判定，遍历顺序仍由 openList 决定，保证选点行为不变
            var openSet = SpawnNodeSet();
            var closedSet = SpawnNodeSet();
            try
            {
                openList.Add(srcNode);
                openSet.Add(srcNode);
                while (openList.Count > 0)
                {
                    int bestIndex = 0;
                    var currentNode = openList[0];
                    for (int i = 1; i < openList.Count; i++)
                    {
                        var openNode = openList[i];
                        //取 F 最小；F 相同再取 H 更小
                        if (openNode.FCost < currentNode.FCost
                            || (openNode.FCost == currentNode.FCost && openNode.HCost < currentNode.HCost))
                        {
                            currentNode = openNode;
                            bestIndex = i;
                        }
                    }
                    openList.RemoveAt(bestIndex);
                    openSet.Remove(currentNode);
                    closedSet.Add(currentNode);

                    if (currentNode == dstNode)
                        return GetFinalPath(srcNode, dstNode);

                    FillNeighboringNodes(currentNode, neighborBuffer);
                    for (int i = 0; i < neighborBuffer.Count; i++)
                    {
                        var neighborNode = neighborBuffer[i];
                        if (neighborNode == null || closedSet.Contains(neighborNode))
                            continue;
                        if (neighborNode.IsObstacle)
                        {
                            closedSet.Add(neighborNode);
                            continue;
                        }
                        int moveCost = currentNode.GCost + GetDistance(currentNode, neighborNode);
                        bool isInOpen = openSet.Contains(neighborNode);
                        if (moveCost < neighborNode.GCost || !isInOpen)
                        {
                            neighborNode.GCost = moveCost;
                            neighborNode.HCost = GetDistance(neighborNode, dstNode);
                            neighborNode.ParentNode = currentNode;
                            if (!isInOpen)
                            {
                                openList.Add(neighborNode);
                                openSet.Add(neighborNode);
                            }
                        }
                    }
                }
                return null;
            }
            finally
            {
                DespawnNodeList(openList);
                DespawnNodeList(neighborBuffer);
                DespawnNodeSet(openSet);
                DespawnNodeSet(closedSet);
            }
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
        public IList<Node> GetNeighboringNodes(Fix64 posX, Fix64 posY)
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
            if (node == null)
                return Array.Empty<Node>();
            var nodeList = SpawnNodeList();
            FillSquareNeighboringNodes(node, nodeList);
            var dstNodes = nodeList.ToArray();
            DespawnNodeList(nodeList);
            return dstNodes;
        }
        /// <summary>
        /// 把临近节点填入复用缓冲，供 FindPath 使用（避免每次展开都分配新集合）。
        /// 默认实现走虚方法 GetNeighboringNodes，因此只重写 GetNeighboringNodes 的子类依然会生效；
        /// 希望做到零分配的子类可以重写本方法直接填充 buffer。
        /// </summary>
        /// <param name="node">被查找的节点</param>
        /// <param name="buffer">复用缓冲，方法内会先 Clear</param>
        protected virtual void FillNeighboringNodes(Node node, List<Node> buffer)
        {
            buffer.Clear();
            if (node == null)
                return;
            var neighbors = GetNeighboringNodes(node);
            if (neighbors == null)
                return;
            for (int i = 0; i < neighbors.Count; i++)
                buffer.Add(neighbors[i]);
        }
        /// <summary>
        /// 九宫格临近节点（禁止斜向切角）填入复用缓冲，不产生分配；
        /// </summary>
        /// <param name="node">被查找的节点</param>
        /// <param name="buffer">复用缓冲，方法内会先 Clear</param>
        protected void FillSquareNeighboringNodes(Node node, List<Node> buffer)
        {
            buffer.Clear();
            if (node == null)
                return;
            var w = GridSizeX - 1;
            var h = GridSizeY - 1;
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
                        //斜向禁止切角：两个正交格任一为障碍就跳过，避免从墙缝穿过
                        if (x != 0 && y != 0)
                        {
                            if (nodeArray[node.IndexX + x, node.IndexY].IsObstacle
                                || nodeArray[node.IndexX, node.IndexY + y].IsObstacle)
                                continue;
                        }
                        buffer.Add(nodeArray[idxX, idxY]);
                    }
                }
            }
        }
        public void Clear()
        {
            nodeArray = new Node[0, 0];
            //网格尺寸必须一起清零：否则 GetNode 仍能通过 IsOverlapping 并索引空数组（与浮点版保持一致）
            GridSizeX = 0;
            GridSizeY = 0;
            nodeListQueue.Clear();
            nodeSetQueue.Clear();
        }
        public bool Equals(AStarFix64 other)
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
            return (int)Math.Floor(Math.Sqrt(x * x + y * y)*14) ;
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
        /// <summary>
        /// 取一个开/闭集缓冲；
        /// </summary>
        HashSet<Node> SpawnNodeSet()
        {
            if (nodeSetQueue.Count > 0)
                return nodeSetQueue.Dequeue();
            return new HashSet<Node>();
        }
        /// <summary>
        /// 归还开/闭集缓冲；
        /// </summary>
        void DespawnNodeSet(HashSet<Node> set)
        {
            if (set == null)
                return;
            set.Clear();
            nodeSetQueue.Enqueue(set);
        }
    }
}
