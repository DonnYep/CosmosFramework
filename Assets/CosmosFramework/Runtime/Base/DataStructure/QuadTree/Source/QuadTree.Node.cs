using System;
using System.Collections.Generic;
using System.Linq;
namespace Cosmos
{
    public partial class QuadTree<T>
    {
        class Node
        {
            /// <summary>
            /// 当前节点的对象容器；
            /// </summary>
            public HashSet<T> ObjectSet = new HashSet<T>();
            /// <summary>
            /// 节点深度； 
            /// </summary>
            public int NodeDepth;
            /// <summary>
            ///当前节点所在的区块；
            /// </summary>
            public Rectangle Area;
            /// <summary>
            /// 父节点；
            /// </summary>
            public Node Parent;
            /// <summary>
            /// 是否存在子节点；
            /// </summary>
            public bool HasChild { get; set; }
            /// <summary>
            /// TopRight Quadrant1
            /// </summary>
            public Node TreeTRNode;
            /// <summary>
            /// TopLeft Quadrant2
            /// </summary>
            public Node TreeTLNode;
            /// <summary>
            /// BottomLeft Quadrant3
            /// </summary>
            public Node TreeBLNode;
            /// <summary>
            /// BottomRight Quadrant4
            /// </summary>
            public Node TreeBRNode;
            public bool IsRectOverlapping(Rectangle rect)
            {
                if (rect.Right < Area.Left || rect.Left > Area.Right) return false;
                if (rect.Top < Area.Bottom || rect.Bottom > Area.Top) return false;
                return true;
            }
            /// <summary>
            ///获得objBound所在节点的所有 T ; 
            /// </summary>
            public T[] GetObjectsByRect(Rectangle objBound)
            {
                if (!HasChild)
                {
                    if (!IsRectOverlapping(objBound))
                        return new T[0];
                    else
                        return Objects();
                }
                else
                {
                    if (TreeTRNode.IsRectOverlapping(objBound)) return TreeTRNode.GetObjectsByRect(objBound);
                    if (TreeTLNode.IsRectOverlapping(objBound)) return TreeTRNode.GetObjectsByRect(objBound);
                    if (TreeBLNode.IsRectOverlapping(objBound)) return TreeTRNode.GetObjectsByRect(objBound);
                    if (TreeBRNode.IsRectOverlapping(objBound)) return TreeTRNode.GetObjectsByRect(objBound);
                    return new T[0];
                }
            }
            public bool Contains(T obj)
            {
                if (!HasChild)
                {
                    return ObjectSet.Contains(obj);
                }
                else
                {
                    if (TreeTRNode.Contains(obj)) return true;
                    if (TreeTLNode.Contains(obj)) return true;
                    if (TreeBLNode.Contains(obj)) return true;
                    if (TreeBRNode.Contains(obj)) return true;
                    return false;
                }
            }
            public bool Insert(Rectangle objBound, T obj)
            {
                if (!IsRectOverlapping(objBound))
                    return false;
                if (!HasChild)
                {
                    return ObjectSet.Add(obj);
                }
                else
                {
                    if (TreeTRNode.Insert(objBound, obj)) return true;
                    if (TreeTLNode.Insert(objBound, obj)) return true;
                    if (TreeBLNode.Insert(objBound, obj)) return true;
                    if (TreeBRNode.Insert(objBound, obj)) return true;
                    return false;
                }
            }
            public Rectangle[] NodeAreaGrids()
            {
                if (!HasChild)
                    return new Rectangle[] { Area };
                else
                {
                    var trNodes = TreeTRNode.NodeAreaGrids();
                    var tlNodes = TreeTLNode.NodeAreaGrids();
                    var blNodes = TreeBLNode.NodeAreaGrids();
                    var brNodes = TreeBRNode.NodeAreaGrids();

                    var length = trNodes.Length + tlNodes.Length + blNodes.Length + brNodes.Length;
                    var dstArr = new Rectangle[length];

                    Array.Copy(trNodes, 0, dstArr, 0, trNodes.Length);
                    Array.Copy(tlNodes, 0, dstArr, trNodes.Length, tlNodes.Length);
                    Array.Copy(blNodes, 0, dstArr, tlNodes.Length + trNodes.Length, blNodes.Length);
                    Array.Copy(brNodes, 0, dstArr, blNodes.Length + tlNodes.Length + trNodes.Length, brNodes.Length);

                    return dstArr;
                }
            }
            public bool Remove(T obj, out Node node)
            {
                if (!HasChild)
                {
                    if (ObjectSet.Remove(obj))
                    {
                        node = this;
                        return true;
                    }
                    else
                    {
                        node = null;
                        return false;
                    }
                }
                else
                {
                    if (TreeTRNode.Remove(obj, out node)) return true;
                    if (TreeTLNode.Remove(obj, out node)) return true;
                    if (TreeBLNode.Remove(obj, out node)) return true;
                    if (TreeBRNode.Remove(obj, out node)) return true;
                    return false;
                }
            }
            public bool PeekNode(T obj, out Node node)
            {
                if (!HasChild)
                {
                    if (ObjectSet.Contains(obj))
                    {
                        node = this;
                        return true;
                    }
                    else
                    {
                        node = null;
                        return false;
                    }
                }
                else
                {
                    if (TreeTRNode.PeekNode(obj, out node)) return true;
                    if (TreeTLNode.PeekNode(obj, out node)) return true;
                    if (TreeBLNode.PeekNode(obj, out node)) return true;
                    if (TreeBRNode.PeekNode(obj, out node)) return true;
                    return false;
                }
            }
            public int ObjectCount()
            {
                if (!HasChild)
                    return ObjectSet.Count;
                else
                {
                    var trCount = TreeTRNode.ObjectCount();
                    var tlCount = TreeTLNode.ObjectCount();
                    var blCount = TreeBLNode.ObjectCount();
                    var brCount = TreeBRNode.ObjectCount();
                    return trCount + tlCount + brCount + blCount;
                }
            }
            public T[] Objects()
            {
                if (!HasChild)
                    return ObjectSet.ToArray();
                else
                {
                    var trObjects = TreeTRNode.Objects();
                    var tlObjects = TreeTLNode.Objects();
                    var blObjects = TreeBLNode.Objects();
                    var brObjects = TreeBRNode.Objects();

                    var length = trObjects.Length + tlObjects.Length + blObjects.Length + brObjects.Length;
                    var dstArr = new T[length];
                    Array.Copy(trObjects, 0, dstArr, 0, trObjects.Length);
                    Array.Copy(tlObjects, 0, dstArr, trObjects.Length, tlObjects.Length);
                    Array.Copy(blObjects, 0, dstArr, tlObjects.Length + trObjects.Length, blObjects.Length);
                    Array.Copy(brObjects, 0, dstArr, blObjects.Length + tlObjects.Length + trObjects.Length, brObjects.Length);
                    return dstArr;
                }
            }
            public void OnQuarter()
            {
                ObjectSet.Clear();
            }
            public void OnCombineQuad() { }
            public int NodeMaxDepth()
            {
                if (!HasChild)
                {
                    return NodeDepth;
                }
                else
                {
                    var trDepth = TreeTRNode.NodeMaxDepth();
                    var tlDepth = TreeTLNode.NodeMaxDepth();
                    var blDepth = TreeBLNode.NodeMaxDepth();
                    var brDepth = TreeBRNode.NodeMaxDepth();
                    var depthArr = new int[4] { trDepth, tlDepth, blDepth, brDepth };
                    var depth = trDepth;
                    var length = depthArr.Length;
                    for (int i = 0; i < length; i++)
                    {
                        if (depth < depthArr[i])
                        {
                            depth = depthArr[i];
                        }
                    }
                    return depth;
                }
            }
            public void Release()
            {
                HasChild = false;
                Parent = null;
                Area = Rectangle.Zero;
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