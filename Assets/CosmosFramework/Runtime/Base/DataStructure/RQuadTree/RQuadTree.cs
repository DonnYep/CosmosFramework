using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos.QuadTree
{
    public partial class RQuadTree<T>
    {
        readonly static QuadTreePool<RQuadTreeNode> nodePool;
        readonly static QuadTreePool<List<T>> listPool;
        readonly static QuadTreePool<HashSet<T>> hashSetPool;
        static RQuadTree()
        {
            nodePool = new QuadTreePool<RQuadTreeNode>(() => new RQuadTreeNode(), node => { node.Release(); });
            listPool = new QuadTreePool<List<T>>(() => new List<T>(), lst => { lst.Clear(); });
            hashSetPool = new QuadTreePool<HashSet<T>>(() => new HashSet<T>(), hs => { hs.Clear(); });
        }
        public static RQuadTree<T> Create(float x, float y, float width, float height, IObjecRectangletBound<T> quadTreebound, int nodeObjectCapacity = 10, int maxDepth = 5)
        {
            var tree = new RQuadTree<T>(new QuadRectangle(x, y, width, height), quadTreebound, nodeObjectCapacity, maxDepth);
            return tree;
        }
        /// <summary>
        /// 释放当前树的静态数据；
        /// </summary>
        public static void Dispose()
        {
            nodePool.Clear();
            listPool.Clear();
            hashSetPool.Clear();
        }
        static RQuadTreeNode CreateNode(float x, float y, float width, float height, RQuadTreeNode parent, int nodeDepth)
        {
            var node = nodePool.Spawn();
            node.Area = new QuadRectangle(x, y, width, height);
            node.Parent = parent;
            node.NodeDepth = nodeDepth;
            node.HasChild = false;
            return node;
        }
        //HashSet<RQuadTreeNode> qtNodeSet;


        HashSet<T> objectCache = new HashSet<T>();

        Dictionary<T, RQuadTreeNode> qtObjectDict;
        /// <summary>
        /// 四叉树中对象的有效边界获取接口；
        /// </summary>
        IObjecRectangletBound<T> objectRectangleBound;
        /// <summary>
        /// 一个节点最大的对象数量；
        /// </summary>
        public int NodeObjectCapacity { get; private set; }
        /// <summary>
        /// 节点数量；
        /// </summary>
        public int NodeCount { get; private set; }
        /// <summary>
        /// 树的深度；
        /// </summary>
        public int TreeCurrentDepth { get; private set; }
        /// <summary>
        /// 树的最大深度；
        /// </summary>
        public int TreeMaxDepth { get; private set; }
        /// <summary>
        /// 根结点；
        /// </summary>
        RQuadTreeNode rootNode;
        private RQuadTree(QuadRectangle rectArea, IObjecRectangletBound<T> boundHelper, int nodeObjectCapacity, int maxDepth)
        {
            NodeObjectCapacity = nodeObjectCapacity;
            objectRectangleBound = boundHelper;
            TreeMaxDepth = maxDepth;
            rootNode = nodePool.Spawn();
            rootNode.Area = rectArea;
            rootNode.NodeDepth = 0;
            qtObjectDict = new Dictionary<T, RQuadTreeNode>();
        }
        private RQuadTree() { }
        public void Insert(T obj)
        {
            if (obj == null) throw new ArgumentNullException($"{nameof(obj)} is invalid !");
            if (qtObjectDict.ContainsKey(obj))
                throw new ArgumentException($"{nameof(obj)} is already existed !");
            var objectBound = GetObjectBound(obj);
            InsertObject(rootNode, objectBound, obj);
        }
        public void Remove(T obj)
        {
            if (obj == null) throw new ArgumentNullException($"{nameof(obj)} is invalid !");
            if (!qtObjectDict.ContainsKey(obj))
                throw new ArgumentException($"{nameof(obj)} is not existed !");
            RemoveObject(obj);
        }
        public QuadRectangle GetObjectBound(T go)
        {
            var rect = new QuadRectangle(objectRectangleBound.GetCenterX(go), objectRectangleBound.GetCenterY(go), objectRectangleBound.GetWidth(go), objectRectangleBound.GetHeight(go));
            return rect;
        }
        public void CheckObjectRect()
        {
            objectCache.Clear();
            foreach (var qtObj in qtObjectDict)
            {
                var obj = qtObj.Key;
                var node = qtObj.Value;
                var objBound = GetObjectBound(obj);
                if (!node.IsRectOverlapping(objBound))
                {
                    objectCache.Add(obj);
                }
            }
            foreach (var obj in objectCache)
            {
                RemoveObject(obj);
                Insert(obj);
            }
        }
        public QuadRectangle[] GetNodeGrids()
        {
            return qtObjectDict.Values.Select(n=>n.Area).ToArray();
        }
        public T[] GetAllObjects()
        {
            return qtObjectDict.Keys.ToArray();
        }
        public bool Contains(T obj)
        {
            return qtObjectDict.ContainsKey(obj);
        }
        void InsertObject(RQuadTreeNode node, QuadRectangle objBound, T obj)
        {
            if (node.IsRectOverlapping(objBound))
            {
                if (!node.HasChild)
                {
                    if (node.ObjectSet.Count < NodeObjectCapacity)//小于刚好可满足插入最大元素数量
                    {
                        node.ObjectSet.Add(obj);
                        qtObjectDict[obj] = node;
                    }
                    else//若大于，则四等分节点
                    {
                        if (TreeCurrentDepth < TreeMaxDepth)
                        {
                            Quarter(node);
                            InsertObject(node, objBound, obj);
                        }
                        else
                            OnOverflowMaxDepth(obj);
                    }
                }
                else
                {
                    InsertObject(node.TreeTRNode, objBound, obj);
                    InsertObject(node.TreeTLNode, objBound, obj);
                    InsertObject(node.TreeBLNode, objBound, obj);
                    InsertObject(node.TreeBRNode, objBound, obj);
                }
            }
            else
                OnInsertFailure(obj);
        }
        void RemoveObject(T obj)
        {
            qtObjectDict.Remove(obj, out var node);
            node.ObjectSet.Remove(obj);
            CombineQuad(node);
        }
        /// <summary>
        /// 检测此节点所在的区块是否可以合并；
        /// </summary>
        void CombineQuad(RQuadTreeNode node)
        {
            if (node.ObjectSet.Count == 0)
            {
                var parent = node.Parent;
                //var peerTreeTR = parent.TreeTRNode;
                //var peerTreeTL = parent.TreeTLNode;
                //var peerTreeBL = parent.TreeBLNode;
                //var peerTreeBR = parent.TreeBRNode;
                //var hasChild = peerTreeBL.HasChild || peerTreeBR.HasChild ||peerTreeTL.HasChild || peerTreeTR.HasChild;
                //if (hasChild)
                //    return;
                var count = parent.ObjectCount();
                if (count <= NodeObjectCapacity)
                {
                    parent.HasChild = false;
                    var list = listPool.Spawn();

                    list.AddRange(parent.Objects());
                    var hashSet = hashSetPool.Spawn();

                    list.ForEach(e => hashSet.Add(e));

                    foreach (var obj in hashSet)
                    {
                        if (qtObjectDict.ContainsKey(obj))
                        {
                            qtObjectDict[obj] = parent;
                            parent.ObjectSet.Add(obj);
                        }
                    }

                    listPool.Despawn(list);
                    hashSetPool.Despawn(hashSet);

                    //qtNodeSet.Remove(peerTreeTR);
                    //qtNodeSet.Remove(peerTreeTL);
                    //qtNodeSet.Remove(peerTreeBL);
                    //qtNodeSet.Remove(peerTreeBR);

                    //nodePool.Despawn(peerTreeTR);
                    //nodePool.Despawn(peerTreeTL);
                    //nodePool.Despawn(peerTreeBL);
                    //nodePool.Despawn(peerTreeBR);
                }
            }
        }
        /// <summary>
        /// 将一个节点四等分化；
        /// </summary>
        /// <param name="node">被四等分化的节点</param>
        void Quarter(RQuadTreeNode node)
        {
            int nextNodeDepth = node.NodeDepth + 1;
            if (TreeCurrentDepth < nextNodeDepth)
                TreeCurrentDepth = nextNodeDepth;
            var area = node.Area;
            node.TreeTRNode = CreateNode(area.X + area.HalfWidth * 0.5f, area.Y + area.HalfHeight * 0.5f, area.HalfWidth, area.HalfHeight, node, nextNodeDepth);
            node.TreeTLNode = CreateNode(area.X - area.HalfWidth * 0.5f, area.Y + area.HalfHeight * 0.5f, area.HalfWidth, area.HalfHeight, node, nextNodeDepth);
            node.TreeBLNode = CreateNode(area.X - area.HalfWidth * 0.5f, area.Y - area.HalfHeight * 0.5f, area.HalfWidth, area.HalfHeight, node, nextNodeDepth);
            node.TreeBRNode = CreateNode(area.X + area.HalfWidth * 0.5f, area.Y - area.HalfHeight * 0.5f, area.HalfWidth, area.HalfHeight, node, nextNodeDepth);
            node.HasChild = true;

            //qtNodeSet.Add(node.TreeTRNode);
            //qtNodeSet.Add(node.TreeTLNode);
            //qtNodeSet.Add(node.TreeBLNode);
            //qtNodeSet.Add(node.TreeBRNode);

            foreach (var obj in node.ObjectSet)
            {
                qtObjectDict.Remove(obj);
                Insert(obj);
            }
            node.ObjectSet.Clear();
        }
        /// <summary>
        /// 添加对象时超出最大深度事件
        /// </summary>
        void OnOverflowMaxDepth(T obj)
        {

        }
        void OnInsertFailure(T obj)
        {

        }
    }
}
