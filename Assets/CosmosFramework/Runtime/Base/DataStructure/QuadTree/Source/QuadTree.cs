using System;
using System.Collections.Generic;
namespace Cosmos
{
    public partial class QuadTree<T>
    {
        Action<T> onOutQuadBound;
        public event Action<T> OnOutQuadBound
        {
            add { onOutQuadBound += value; }
            remove { onOutQuadBound -= value; }
        }
        /// <summary>
        /// 树的深度；
        /// </summary>
        int CurrentDepth;
        /// <summary>
        /// 根结点；
        /// </summary>
        Node rootNode;
        HashSet<T> objectRemoveCache = new HashSet<T>();
        Queue<Node> nodeCache = new Queue<Node>();
        /// <summary>
        /// 四叉树中对象的有效边界获取接口；
        /// </summary>
        IObjecBound objectRectangleBound;
        /// <summary>
        /// 一个节点最大的对象数量；
        /// </summary>
        public int NodeObjectCapacity { get; private set; }
        /// <summary>
        /// 节点数量；
        /// </summary>
        public int NodeCount { get { return rootNode.NodeCount(); } }
        /// <summary>
        /// 当前树的最大深度；
        /// </summary>
        public int TreeCurrentDepth { get { return rootNode.NodeMaxDepth(); } }
        /// <summary>
        /// 树的最大深度；
        /// </summary>
        public int TreeMaxDepth { get; private set; }
        public Rectangle QuadTreeArea { get { return rootNode.Area; } }
        public QuadTree(Rectangle rectArea, IObjecBound boundHelper, int nodeObjectCapacity = 10, int maxDepth = 5)
        {
            NodeObjectCapacity = nodeObjectCapacity;
            objectRectangleBound = boundHelper;
            TreeMaxDepth = maxDepth;
            rootNode = new Node();
            rootNode.Area = rectArea;
            rootNode.NodeDepth = 0;
        }
        public QuadTree(float centerX, float centerY, float width, float height, IObjecBound boundHelper, int nodeObjectCapacity = 10, int maxDepth = 5)
        {
            NodeObjectCapacity = nodeObjectCapacity;
            objectRectangleBound = boundHelper;
            TreeMaxDepth = maxDepth;
            rootNode = new Node();
            rootNode.Area = new Rectangle(centerX, centerY, width, height);
            rootNode.NodeDepth = 0;
        }
        public bool Insert(T obj)
        {
            if (obj == null) throw new ArgumentNullException($"{nameof(obj)} is invalid !");
            var objectBound = GetObjectBound(obj);
            return InsertObject(rootNode, objectBound, obj);
        }
        public bool Remove(T obj)
        {
            if (obj == null) throw new ArgumentNullException($"{nameof(obj)} is invalid !");
            if (rootNode.Remove(obj, out var node))
            {
                Merge(node);
                return true;
            }
            return false;
        }
        public void CheckObjectBound()
        {
            objectRemoveCache.Clear();
            var objects = rootNode.Objects();
            var length = objects.Length;
            for (int i = 0; i < length; i++)
            {
                var obj = objects[i];
                if (PeekObjectNode(obj, out var node))
                {
                    var objBound = GetObjectBound(obj);
                    if (node.IsRectOverlapping(objBound))
                        continue;
                    Remove(obj);
                    onOutQuadBound?.Invoke(obj);
                    objectRemoveCache.Add(obj);
                }
                else
                {
                    objectRemoveCache.Add(obj);
                }
            }
            foreach (var obj in objectRemoveCache)
            {
                Insert(obj);
            }
        }
        public Rectangle[] GetAreaGrids()
        {
            return rootNode.NodeAreaGrids();
        }
        /// <summary>
        /// 获取物体所在的区域网格；
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>网格数据</returns>
        public Rectangle GetAreaGrid(T obj)
        {
            var objBound = GetObjectBound(obj);
            return rootNode.NodeAreaGrid(objBound);
        }
        public T[] GetAllObjects()
        {
            return rootNode.Objects();
        }
        public bool Contains(T obj)
        {
            return rootNode.Contains(obj);
        }
        public T[] GetObjectsByObjectBound(T obj)
        {
            if (obj == null) throw new ArgumentNullException($"{nameof(obj)} is invalid !");
            var objBound = GetObjectBound(obj);
            return rootNode.GetObjectsByRect(objBound);
        }
        public bool IsOverlapping(T obj)
        {
            var objBound = GetObjectBound(obj);
            return rootNode.IsRectOverlapping(objBound);
        }
        public void ClearNodeCache()
        {
            nodeCache.Clear();
        }
        bool InsertObject(Node node, Rectangle objBound, T obj)
        {
            if (!node.IsRectOverlapping(objBound))
                return false;
            if (node.HasChild)
            {
                if (InsertObject(node.TreeTRNode, objBound, obj)) return true;
                if (InsertObject(node.TreeTLNode, objBound, obj)) return true;
                if (InsertObject(node.TreeBLNode, objBound, obj)) return true;
                if (InsertObject(node.TreeBRNode, objBound, obj)) return true;
                //四个象限覆盖父节点，理论上必然有子节点可放；兜底留在本节点，绝不丢对象
                return node.Insert(objBound, obj);
            }
            if (node.ObjectCount() < NodeObjectCapacity)//小于刚好可满足插入最大元素数量
                return node.Insert(objBound, obj);
            //按"当前节点深度"判断能否继续分裂：用树级 CurrentDepth 会让全树提前停止分裂（丢对象）
            if (node.NodeDepth < TreeMaxDepth)
            {
                Split(node);
                if (InsertObject(node, objBound, obj))
                    return true;
                return node.Insert(objBound, obj);
            }
            //到达最大深度仍超出容量：留在本节点（叶子可超额），不能静默丢对象
            return node.Insert(objBound, obj);
        }
        /// <summary>
        /// 检测此节点所在的区块是否可以合并；
        /// </summary>
        void Merge(Node node)
        {
            if (!node.HasChild)
            {
                var parent = node.Parent;
                if (parent == null)//根节点
                    return;
                var objects = parent.Objects();
                if (objects.Length <= NodeObjectCapacity)
                {
                    parent.HasChild = false;
                    foreach (var obj in objects)
                    {
                        Insert(obj);
                    }
                    ReleaseNode(node);
                    parent.OnMerge();
                }
            }
        }
        /// <summary>
        /// 将一个节点四等分化；
        /// </summary>
        /// <param name="node">被四等分化的节点</param>
        void Split(Node node)
        {
            int nextNodeDepth = node.NodeDepth + 1;
            if (CurrentDepth < nextNodeDepth)
                CurrentDepth = nextNodeDepth;
            var area = node.Area;
            node.TreeTRNode = CreateNode(area.CenterX + area.HalfWidth * 0.5f, area.CenterY + area.HalfHeight * 0.5f, area.HalfWidth, area.HalfHeight, node, nextNodeDepth);
            node.TreeTLNode = CreateNode(area.CenterX - area.HalfWidth * 0.5f, area.CenterY + area.HalfHeight * 0.5f, area.HalfWidth, area.HalfHeight, node, nextNodeDepth);
            node.TreeBLNode = CreateNode(area.CenterX - area.HalfWidth * 0.5f, area.CenterY - area.HalfHeight * 0.5f, area.HalfWidth, area.HalfHeight, node, nextNodeDepth);
            node.TreeBRNode = CreateNode(area.CenterX + area.HalfWidth * 0.5f, area.CenterY - area.HalfHeight * 0.5f, area.HalfWidth, area.HalfHeight, node, nextNodeDepth);
            var objects = node.Objects();
            node.HasChild = true;
            //先把父节点集合清空，再直接插入子节点：
            //避免"从根重新插入"时再次走回本节点造成重复/丢失
            node.OnSplit();
            foreach (var obj in objects)
            {
                InsertObject(node, GetObjectBound(obj), obj);
            }
        }
        bool PeekObjectNode(T obj, out Node node)
        {
            return rootNode.PeekNode(obj, out node);
        }
        Rectangle GetObjectBound(T go)
        {
            var rect = new Rectangle(objectRectangleBound.GetCenterX(go), objectRectangleBound.GetCenterY(go), objectRectangleBound.GetWidth(go), objectRectangleBound.GetHeight(go));
            return rect;
        }
        Node CreateNode(float x, float y, float width, float height, Node parent, int nodeDepth)
        {
            var node = AcquireNode();
            node.Area = new Rectangle(x, y, width, height);
            node.Parent = parent;
            node.NodeDepth = nodeDepth;
            node.HasChild = false;
            return node;
        }
        Node AcquireNode()
        {
            if (nodeCache.Count > 0)
            {
                return nodeCache.Dequeue();
            }
            else
            {
                return new Node();
            }
        }
        void ReleaseNode(Node node)
        {
            node.Release();
            nodeCache.Enqueue(node);
        }
    }
}
