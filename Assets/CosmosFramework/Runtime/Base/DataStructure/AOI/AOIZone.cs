using System;
using System.Collections.Generic;

namespace Cosmos
{
    public partial class AOIZone<T>
        where T : IComparable
    {
        Dictionary<T, AOIEntity> entityDict = new Dictionary<T, AOIEntity>();
        float viewDistance;
        /// <summary>
        /// X轴跳表；
        /// </summary>
        readonly SkipList<float> xLinks;
        /// <summary>
        /// Y轴跳表；
        /// </summary>
        readonly SkipList<float> yLinks;
        /// <summary>
        /// 当前AOI的矩形区域；
        /// </summary>
        public Rectangle ZoneSquare { get; private set; }
        /// <summary>
        /// AOIZone宽；
        /// </summary>
        public float Width { get; private set; }
        /// <summary>
        /// AOIZone高；
        /// </summary>
        public float Height { get; private set; }
        /// <summary>
        /// 中心X轴偏移量；
        /// </summary>
        public float OffsetX { get; private set; }
        /// <summary>
        /// 中心Y轴偏移量；
        /// </summary>
        public float OffsetY { get; private set; }
        /// <summary>
        /// 节点间可视距离；
        /// </summary>
        public float ViewDistance
        {
            get { return viewDistance; }
            set
            {
                if (value <= 0)
                    viewDistance = 0;
                viewDistance = value;
            }
        }

        IObjectHelper objectHelper;
        public AOIZone(int width, int height, float offsetX, float offsetY, IObjectHelper objectHelper)
        {
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
            this.objectHelper = objectHelper;
            var centerX = offsetX + width / 2;
            var centerY = offsetY + height / 2;
            ZoneSquare = new Rectangle(centerX, centerY, width, height);
            xLinks = new SkipList<float>((t) => objectHelper.GetCenterX(t));
            yLinks = new SkipList<float>((t) => objectHelper.GetCenterX(t));
        }
        public AOIZone(int sideLength, float offsetX, float offsetY, IObjectHelper objectHelper)
            : this(sideLength, sideLength, offsetX, offsetY, objectHelper) { }

        public bool IsOverlapping(T obj)
        {
            var posX = objectHelper.GetCenterX(obj);
            var posY = objectHelper.GetCenterY(obj);
            return IsOverlapping(posX, posY);
        }
        public bool IsOverlapping(float posX, float posY)
        {
            if (posX < ZoneSquare.Left || posX > ZoneSquare.Right) return false;
            if (posY < ZoneSquare.Bottom || posY > ZoneSquare.Top) return false;
            return true;
        }
        public bool Insert(T obj)
        {
            if (!entityDict.ContainsKey(obj))
            {
                xLinks.Add(obj);
                yLinks.Add(obj);
                var xNode = xLinks.Find(obj);
                var yNode = yLinks.Find(obj);
                //TODO
                //UNDONE
                return true;
            }
            return false;
        }
        public bool Remove(T obj)
        {
            if (entityDict.TryRemove(obj, out var entity))
            {
                xLinks.Remove(obj);
                yLinks.Remove(obj);
                var views = entity.ViewEntity;
                foreach (var ve in views)
                {
                    ve.OnEntityExit(entity);
                }
                //TODO
                return true;
            }
            return false;
        }
        public bool Contains(T obj)
        {
            return entityDict.ContainsKey(obj);
        }
        /// <summary>
        /// 轮询刷新节点；
        /// </summary>
        public void Refresh()
        {

        }
        /// <summary>
        /// 获取临近的对象；
        /// </summary>
        /// <param name="obj">查找的对象</param>
        /// <returns>获取到的临近对象</returns>
        public T[] GetNeighbors(T obj,float viewDistance)
        {
            if (entityDict.TryGetValue(obj, out var entity))
            {

            }

            #region xLinks
            var xNode = xLinks.Find(obj);
            for (int i = 0; i < 2; i++)
            {
                var cur = i == 0 ? xNode.Next : xNode.Previous;
                while (cur!=null)
                {
                    //超出距离则break；
                    if (Distance(cur, xNode) > viewDistance)
                    {
                        break;
                    }
 
                }
            }
            #endregion

            #region yLinks
            var yNode = yLinks.Find(obj);


            #endregion


            return null;
        }
        public Rectangle GetArea(T obj)
        {
            return Rectangle.Zero;
        }
        float Distance(SkipListNode a,SkipListNode b)
        {
            return 0;
        }
    }
}
