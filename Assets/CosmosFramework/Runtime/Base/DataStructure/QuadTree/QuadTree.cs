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
        public QuadRectangle Area { get; private set; }
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
            Area = new QuadRectangle(x, y, width, height);
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
            if (CurrentDepth > MaxDepth) return;
            int nextDepth = CurrentDepth + 1;
            hasChildren = true;
            treeTR = new QuadTree<T>(Area.X + Area.HalfWidth * 0.5f, Area.Y + Area.HalfHeight * 0.5f, Area.HalfWidth, Area.HalfHeight, objectRectangleBound, ObjectCapacity, MaxDepth, nextDepth);
            treeTL = new QuadTree<T>(Area.X - Area.HalfWidth * 0.5f, Area.Y + Area.HalfHeight * 0.5f, Area.HalfWidth, Area.HalfHeight, objectRectangleBound, ObjectCapacity, MaxDepth, nextDepth);
            treeBL = new QuadTree<T>(Area.X - Area.HalfWidth * 0.5f, Area.Y - Area.HalfHeight * 0.5f, Area.HalfWidth, Area.HalfHeight, objectRectangleBound, ObjectCapacity, MaxDepth, nextDepth);
            treeBR = new QuadTree<T>(Area.X + Area.HalfWidth * 0.5f, Area.Y - Area.HalfHeight * 0.5f, Area.HalfWidth, Area.HalfHeight, objectRectangleBound, ObjectCapacity, MaxDepth, nextDepth);
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
        public T[] GetAreaObjects(QuadRectangle rect)
        {
            List<T> foundObjects = new List<T>();
            if (hasChildren)
            {
                foundObjects.AddRange(treeTR.GetAreaObjects(rect));
                foundObjects.AddRange(treeTL.GetAreaObjects(rect));
                foundObjects.AddRange(treeBR.GetAreaObjects(rect));
                foundObjects.AddRange(treeBL.GetAreaObjects(rect));
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
        /// <param name="go">树中的对象</param>
        /// <returns>相同级别的对象</returns>
        public T[] GetAreaObjects(T go)
        {
            return GetAreaObjects(new QuadRectangle(objectRectangleBound.GetCenterX(go), objectRectangleBound.GetCenterY(go), objectRectangleBound.GetWidth(go), objectRectangleBound.GetHeight(go)));
        }
        public T[] GetAllObjects()
        {
            List<T> foundObjects = new List<T>();
            if (hasChildren)
            {
                foundObjects.AddRange(treeTR.GetAllObjects());
                foundObjects.AddRange(treeTL.GetAllObjects());
                foundObjects.AddRange(treeBR.GetAllObjects());
                foundObjects.AddRange(treeBL.GetAllObjects());
            }
            else
            {
                foundObjects.AddRange(objectSet);
            }
            return foundObjects.ToArray();
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
        public QuadRectangle GetObjectGrid(T go)
        {
            var rect = new QuadRectangle(objectRectangleBound.GetCenterX(go), objectRectangleBound.GetCenterY(go), objectRectangleBound.GetWidth(go), objectRectangleBound.GetHeight(go));
            if (hasChildren)
            {
                var tlRect = treeTL.GetObjectGrid(go);
                var trRect = treeTR.GetObjectGrid(go);
                var blRect = treeBL.GetObjectGrid(go);
                var brRect = treeBR.GetObjectGrid(go);
                if (tlRect != QuadRectangle.Zero) return tlRect;
                if (trRect != QuadRectangle.Zero) return trRect;
                if (blRect != QuadRectangle.Zero) return blRect;
                if (brRect != QuadRectangle.Zero) return brRect;
            }
            else
            {
                if (IsOverlapping(rect))
                {
                    return Area;
                }
            }
            return QuadRectangle.Zero;
        }
        public QuadRectangle[] GetGrid()
        {
            List<QuadRectangle> grid = new List<QuadRectangle> { Area };
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
        bool IsOverlapping(QuadRectangle rect)
        {
            if (rect.Right < Area.Left || rect.Left > Area.Right) return false;
            if (rect.Top < Area.Bottom || rect.Bottom > Area.Top) return false;
            return true;
        }

        /// <summary>
        /// 对象是否存在于当前rect中；
        /// </summary>
        bool IsObjectInside(T go)
        {
            var centerX = objectRectangleBound.GetCenterX(go);
            var centerY = objectRectangleBound.GetCenterY(go);
            var width = objectRectangleBound.GetWidth(go);
            var height = objectRectangleBound.GetHeight(go);
            var halfHeight = height * 0.5f;
            var halfWidth = width * 0.5f;
            var top = centerY + halfHeight;
            var bottom = centerY - halfHeight;
            var left = centerX - halfWidth;
            var right = centerX + halfWidth;
            if (top < Area.Bottom) return false;
            if (bottom > Area.Top) return false;
            if (left > Area.Right) return false;
            if (right < Area.Left) return false;
            return true;
        }
    }
}