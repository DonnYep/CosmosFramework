using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace Cosmos.QuadTree
{
    /// <summary>
    /// 四叉树；
    /// </summary>
    /// <typeparam name="T">四叉树自定义的数据模型</typeparam>
    public class QuadTree<T>
    {
        /// <summary>
        ///当前所在的区块；
        /// </summary>
        public QuadTreeRect Area { get; private set; }
        /// <summary>
        /// 当前树节点的物体对象集合；
        /// </summary>
        readonly HashSet<T> objectSet;
        /// <summary>
        /// 四叉树中对象的有效边界获取接口；
        /// </summary>
        readonly IObjecRectangletBound<T> objectRectangleBound;
        /// <summary>
        /// 是否存在子节点；
        /// </summary>
        bool hasChildren;
        /// <summary>
        /// 当前深度；
        /// </summary>
        public int CurrentDepth { get; private set; }
        /// <summary>
        /// 最大深度；
        /// </summary>
        public int MaxDepth { get; private set; }
        /// <summary>
        /// 当前Rect的对象数量；
        /// </summary>
        public int ObjectCapacity { get; private set; }
        /// <summary>
        /// TopRight Quadrant1
        /// </summary>
        QuadTree<T> treeTR;
        /// <summary>
        /// TopLeft Quadrant2
        /// </summary>
        QuadTree<T> treeTL;
        /// <summary>
        /// BottomLeft Quadrant3
        /// </summary>
        QuadTree<T> treeBL;
        /// <summary>
        /// BottomRight Quadrant4
        /// </summary>
        QuadTree<T> treeBR;
        public QuadTree(float x, float y, float width, float height, IObjecRectangletBound<T> quadTreebound, int objectCapacity = 10, int maxDepth = 5, int currentDepth = 0)
        {
            Area = new QuadTreeRect(x, y, width, height);
            objectSet = new HashSet<T>();
            this.objectRectangleBound = quadTreebound;
            this.CurrentDepth = currentDepth;
            this.MaxDepth = maxDepth;
            this.ObjectCapacity = objectCapacity;
            hasChildren = false;
        }
        public QuadTree(float width, float height, IObjecRectangletBound<T> objectBound, int maxObject = 10, int maxDepth = 5, int currentDepth = 0)
            : this(0, 0, width, height, objectBound, maxObject, maxDepth, currentDepth) { }
        /// <summary>
        /// 切割四等分；
        /// </summary>
        public bool Insert(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (!IsObjectInside(obj)) return false;
            if (hasChildren)
            {
                if (treeTL.Insert(obj)) return true;
                if (treeTR.Insert(obj)) return true;
                if (treeBL.Insert(obj)) return true;
                if (treeBR.Insert(obj)) return true;
            }
            else
            {
                objectSet.Add(obj);
                if (objectSet.Count > ObjectCapacity)
                {
                    Quarter();
                }
            }
            return true;
        }
        public void Quarter()
        {
            if (CurrentDepth >= MaxDepth) return;
            int nextDepth = CurrentDepth + 1;
            hasChildren = true;
            treeTL = new QuadTree<T>(Area.X, Area.Y, Area.HalfWidth, Area.HalfHeight, objectRectangleBound, ObjectCapacity, MaxDepth, nextDepth);
            treeTR = new QuadTree<T>(Area.CenterX, Area.Y, Area.HalfWidth, Area.HalfHeight, objectRectangleBound, ObjectCapacity, MaxDepth, nextDepth);
            treeBL = new QuadTree<T>(Area.X, Area.CenterY, Area.HalfWidth, Area.HalfHeight, objectRectangleBound, ObjectCapacity, MaxDepth, nextDepth);
            treeBR = new QuadTree<T>(Area.CenterX, Area.CenterY, Area.HalfWidth, Area.HalfHeight, objectRectangleBound, ObjectCapacity, MaxDepth, nextDepth);
            foreach (var obj in objectSet)
            {
                Insert(obj);
            }
            objectSet.Clear();
        }
        public void Clear()
        {
            if (hasChildren)
            {
                treeTR.Clear();
                treeTR = null;
                treeTL.Clear();
                treeTL = null;
                treeBR.Clear();
                treeBR = null;
                treeBL.Clear();
                treeBL = null;
            }
            objectSet.Clear();
            hasChildren = false;
            Area.IsOverlapped = false;
        }
        public int Count()
        {
            int count = 0;
            if (hasChildren)
            {
                count += treeTR.Count();
                count += treeTL.Count();
                count += treeBR.Count();
                count += treeBL.Count();
            }
            else
            {
                count = objectSet.Count;
            }
            return count;
        }
        /// <summary>
        /// 获取区块中的所有对象；
        /// </summary>
        /// <param name="rect">rect区块</param>
        /// <returns>区块中的所有对象</returns>
        public T[] FindObjects(QuadTreeRect rect)
        {
            List<T> foundObjects = new List<T>();
            if (hasChildren)
            {
                foundObjects.AddRange(treeTR.FindObjects(rect));
                foundObjects.AddRange(treeTL.FindObjects(rect));
                foundObjects.AddRange(treeBR.FindObjects(rect));
                foundObjects.AddRange(treeBL.FindObjects(rect));
            }
            else
            {
                if (IsOverlapping(rect))
                {
                    foundObjects.AddRange(objectSet);
                }
            }
            HashSet<T> result = new HashSet<T>();
            result.UnionWith(foundObjects);
            return result.ToArray();
        }
        /// <summary>
        /// 获取对象所在区块中所有相同深度的对象；
        /// </summary>
        /// <param name="obj">树中的对象</param>
        /// <returns>相同级别的对象</returns>
        public T[] FindObjects(T obj)
        {
            return FindObjects(new QuadTreeRect(objectRectangleBound.GetPositonX(obj), objectRectangleBound.GetPositonY(obj), objectRectangleBound.GetWidth(obj), objectRectangleBound.GetHeight(obj)));
        }
        /// <summary>
        ///插入一个对象集合； 
        /// </summary>
        public void InsertRange(IEnumerable<T> objects)
        {
            foreach (T obj in objects)
            {
                Insert(obj);
            }
        }
        public QuadTreeRect[] GetGrid()
        {
            List<QuadTreeRect> grid = new List<QuadTreeRect> { Area };
            if (hasChildren)
            {
                grid.AddRange(treeTR.GetGrid());
                grid.AddRange(treeTL.GetGrid());
                grid.AddRange(treeBR.GetGrid());
                grid.AddRange(treeBL.GetGrid());
            }
            return grid.ToArray();
        }
        /// <summary>
        ///是否完全重叠； 
        /// </summary>
        bool IsOverlapping(QuadTreeRect rect)
        {
            if (rect.Right < Area.Left || rect.Left > Area.Right) return false;
            if (rect.Top > Area.Bottom || rect.Bottom < Area.Top) return false;
            Area.IsOverlapped = true;
            return true;
        }
        /// <summary>
        /// 对象是否存在于当前rect中；
        /// </summary>
        bool IsObjectInside(T go)
        {
            var x = objectRectangleBound.GetPositonX(go);
            var y = objectRectangleBound.GetPositonY(go);
            var width = objectRectangleBound.GetWidth(go);
            var height = objectRectangleBound.GetHeight(go);
            var top = y + height * 0.5f;
            var bottom = y - height * 0.5f;
            var left = x - width * 0.5f;
            var right = x + width * 0.5f;
            if (top > Area.Bottom) return false;
            if (bottom < Area.Top) return false;
            if (left >Area.Right) return false;
            if (right < Area.Left) return false;
            return true;
        }
    }
}