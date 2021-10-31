using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.QuadTree
{
    public partial class RQuadTree<T>
    {

        struct RQuadTreeObject
        {
            public T obj;
            public RQuadTreeNode OwnerNode;
            public RQuadTreeObject(T obj, RQuadTreeNode ownerNode)
            {
                this.obj = obj;
                OwnerNode = ownerNode;
            }
        }
        class RQuadTreeNode
        {
            /// <summary>
            /// 当前节点的对象容器；
            /// </summary>
            public HashSet<T> ObjectSet = hashSetPool.Spawn();
            /// <summary>
            /// 节点深度； 
            /// </summary>
            public int NodeDepth;
            /// <summary>
            ///当前节点所在的区块；
            /// </summary>
            public QuadRectangle Area;
            /// <summary>
            /// 父节点；
            /// </summary>
            public RQuadTreeNode Parent;

            public bool HasChild { get; set; }
            /// <summary>
            /// TopRight Quadrant1
            /// </summary>
            public RQuadTreeNode TreeTRNode;
            /// <summary>
            /// TopLeft Quadrant2
            /// </summary>
            public RQuadTreeNode TreeTLNode;
            /// <summary>
            /// BottomLeft Quadrant3
            /// </summary>
            public RQuadTreeNode TreeBLNode;
            /// <summary>
            /// BottomRight Quadrant4
            /// </summary>
            public RQuadTreeNode TreeBRNode;
            public bool IsRectOverlapping(QuadRectangle rect)
            {
                if (rect.Right < Area.Left || rect.Left > Area.Right) return false;
                if (rect.Top < Area.Bottom || rect.Bottom > Area.Top) return false;
                return true;
            }
            public int ObjectCount()
            {
                if (!HasChild)
                    return ObjectSet.Count;
                else
                {
                    var blCount = TreeBLNode.ObjectCount();
                    var brCount = TreeBRNode.ObjectCount();
                    var tlCount = TreeTLNode.ObjectCount();
                    var trCount = TreeTRNode.ObjectCount();
                    return trCount + tlCount + brCount + blCount;
                }
            }
            public T[] Objects()
            {
                if (!HasChild)
                    return ObjectSet.ToArray();
                else
                {
                    var list= listPool.Spawn();
                    list.AddRange(TreeBLNode.Objects());
                    list.AddRange(TreeBRNode.Objects());
                    list.AddRange(TreeTLNode.Objects());
                    list.AddRange(TreeTRNode.Objects());
                    var arr= list.ToArray();
                    listPool.Despawn(list);
                    return arr;
                }
            }
            public void Release()
            {
                HasChild = false;
                Parent = null;
                Area = QuadRectangle.Zero;
                NodeDepth = 0;

                ObjectSet.Clear();

                TreeTRNode = null;
                TreeTLNode = null;
                TreeBLNode = null;
                TreeBRNode = null;
            }
        }
    }
}