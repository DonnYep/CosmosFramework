using System;
using System.Collections.Generic;

namespace Cosmos
{
    /// <summary>
    ///跳表实现的AOIZone； 
    /// </summary>
    public partial class AOIZone<T>
        where T : class
    {
        Dictionary<long, AOIEntity> entityDict = new Dictionary<long, AOIEntity>();
        Queue<AOIEntity> entityCacheQueue = new Queue<AOIEntity>();
        /// <summary>
        /// X轴跳表；
        /// </summary>
        readonly AOISkipList<AOIEntity, float> xLinks;
        /// <summary>
        /// Y轴跳表；
        /// </summary>
        readonly AOISkipList<AOIEntity, float> yLinks;
        /// <summary>
        /// 当前AOI的矩形区域；
        /// </summary>
        public Rectangle ZoneSquare { get; private set; }
        public AOIZone(int width, int height) : this(width, height, 0, 0) { }
        public AOIZone(int sideLength, float centerX, float centerY)
            : this(sideLength, sideLength, centerX, centerY) { }
        public AOIZone(int sideLength) : this(sideLength, sideLength, 0, 0) { }
        public AOIZone(int width, int height, float centerX, float centerY)
        {
            ZoneSquare = new Rectangle(centerX, centerY, width, height);
            xLinks = new AOISkipList<AOIEntity, float>(t => t.PositionX);
            yLinks = new AOISkipList<AOIEntity, float>(t => t.PositionY);
        }
        public bool IsOverlapping(float posX, float posY)
        {
            if (posX < ZoneSquare.Left || posX > ZoneSquare.Right) return false;
            if (posY < ZoneSquare.Bottom || posY > ZoneSquare.Top) return false;
            return true;
        }
        public bool Add(long key, T obj)
        {
            return Add(key, obj, 1);
        }
        public bool Add(long key, T obj, float viewDistance)
        {
            return Add(key, obj, 0, 0, viewDistance);
        }
        public bool Add(long key, T obj, float viewDistance, float posX, float posY)
        {
            if (!entityDict.ContainsKey(key))
            {
                if (!IsOverlapping(posX, posY))
                    return false;
                var entity = AcquireEntity(key, obj);

                xLinks.Add(entity);
                yLinks.Add(entity);

                var xNode = xLinks.FindLowest(entity);
                var yNode = yLinks.FindLowest(entity);

                entity.XNode = xNode;
                entity.YNode = yNode;

                entityDict.Add(key, entity);

                entity.PositionX = posX;
                entity.PositionY = posY;

                entity.ViewDistance = viewDistance;
                #region xLink
                for (int i = 0; i < 2; i++)
                {
                    var curNode = i == 0 ? xNode.Next : xNode.Previous;
                    while (curNode != null)
                    {
                        var distance = AbsDistance(xNode, curNode);
                        if (distance > entity.ViewDistance)
                        {
                            break;
                        }
                        else
                        {
                            entity.ViewEntities.Add(entity);
                        }
                        curNode = i == 0 ? curNode.Next : curNode.Previous;
                    }
                }
                #endregion

                #region yLink
                for (int i = 0; i < 2; i++)
                {
                    var curNode = i == 0 ? yNode.Next : yNode.Previous;
                    while (curNode != null)
                    {
                        var distance = AbsDistance(yNode, curNode);
                        if (distance > entity.ViewDistance)
                        {
                            break;
                        }
                        else
                        {
                            entity.ViewEntities.Add(entity);
                        }
                        curNode = i == 0 ? curNode.Next : curNode.Previous;
                    }
                }
                #endregion

                return true;
            }
            return false;
        }
        public bool Remove(long key, out T value)
        {
            value = default(T);
            if (entityDict.TryRemove(key, out var entity))
            {
                value = entity.Handle;
                var xNode = xLinks.FindLowest(entity);
                var yNode = yLinks.FindLowest(entity);

                entity.XNode = xNode;
                entity.YNode = yNode;

                #region xLink
                for (int i = 0; i < 2; i++)
                {
                    var curNode = i == 0 ? xNode.Next : xNode.Previous;
                    while (curNode != null)
                    {
                        var distance = AbsDistance(xNode, curNode);
                        if (distance > entity.ViewDistance)
                        {
                            break;
                        }
                        else
                        {
                            entity.ViewEntities.Add(entity);
                        }
                        curNode = i == 0 ? curNode.Next : curNode.Previous;
                    }
                }
                #endregion

                #region yLink
                for (int i = 0; i < 2; i++)
                {
                    var curNode = i == 0 ? yNode.Next : yNode.Previous;
                    while (curNode != null)
                    {
                        var distance = AbsDistance(yNode, curNode);
                        if (distance > entity.ViewDistance)
                        {
                            break;
                        }
                        else
                        {
                            entity.ViewEntities.Add(entity);
                        }
                        curNode = i == 0 ? curNode.Next : curNode.Previous;
                    }
                }
                #endregion

                xLinks.Remove(entity);
                yLinks.Remove(entity);
                ReleaseEntity(entity);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取值；
        /// </summary>
        /// <param name="key">实体的key</param>
        /// <param name="value">获取到的值</param>
        /// <returns>是否存在</returns>
        public bool PeekValue(long key, out T value)
        {
            value = default(T);
            var rst = entityDict.TryGetValue(key, out var entity);
            value = entity.Handle;
            return rst;
        }
        /// <summary>
        /// 获取值；
        /// </summary>
        /// <param name="key">实体的key</param>
        /// <param name="entity">获取到的实体</param>
        /// <returns>是否存在</returns>
        public bool PeekEntity(long key, out AOIEntity entity)
        {
            return entityDict.TryGetValue(key, out entity);
        }
        /// <summary>
        /// 是否存在实体；
        /// </summary>
        /// <param name="key">实体的key</param>
        /// <returns>是否存在</returns>
        public bool Contains(long key)
        {
            return entityDict.ContainsKey(key);
        }
        /// <summary>
        /// 移动；
        /// </summary>
        /// <param name="key">实体的key</param>
        /// <param name="posX">X方向新的位置点</param>
        /// <param name="poxY">Y方向新的位置点</param>
        public void Move(long key, float posX, float poxY)
        {
            if (entityDict.TryRemove(key, out var entity))
            {
                bool isMoved = false;
                if (!IsOverlapping(posX, poxY))
                    return;
                if (Math.Abs(entity.PositionX - posX) > 0)
                {
                    entity.PositionX = posX;
                    xLinks.Update(entity);
                    isMoved = true;
                }
                if (Math.Abs(entity.PositionY - poxY) > 0)
                {
                    entity.PositionY = poxY;
                    yLinks.Update(entity);
                    isMoved = true;
                }

                if (!isMoved)
                    return;

                entity.SwapViewEntity();

                var xNode = xLinks.FindLowest(entity);
                var yNode = yLinks.FindLowest(entity);

                #region xLink
                for (int i = 0; i < 2; i++)
                {
                    var curNode = i == 0 ? xNode.Next : xNode.Previous;
                    while (curNode != null)
                    {
                        var distance = AbsDistance(xNode, curNode);
                        if (distance > entity.ViewDistance)
                        {
                            break;
                        }
                        else
                        {
                            //X轴距离之内的实体
                            entity.ViewEntities.Add(curNode.Value);
                        }
                        curNode = i == 0 ? curNode.Next : curNode.Previous;
                    }
                }
                #endregion

                #region yLink
                for (int i = 0; i < 2; i++)
                {
                    var curNode = i == 0 ? yNode.Next : yNode.Previous;
                    while (curNode != null)
                    {
                        var distance = AbsDistance(yNode, curNode);
                        if (distance > entity.ViewDistance)
                        {
                            break;
                        }
                        else
                        {
                            //Y轴距离之内的实体
                            entity.ViewEntities.Add(curNode.Value);
                        }
                        curNode = i == 0 ? curNode.Next : curNode.Previous;
                    }
                }
                #endregion
            }
        }
        public void Move(long key, float viewDistance, float posX, float poxY)
        {
            if (entityDict.TryRemove(key, out var entity))
            {
                bool isMoved = false;
                if (!IsOverlapping(posX, poxY))
                    return;
                if (Math.Abs(entity.PositionX - posX) > 0)
                {
                    entity.PositionX = posX;
                    xLinks.Update(entity);
                    isMoved = true;
                }
                if (Math.Abs(entity.PositionY - poxY) > 0)
                {
                    entity.PositionY = poxY;
                    yLinks.Update(entity);
                    isMoved = true;
                }

                if (!isMoved)
                    return;
                entity.ViewDistance = viewDistance;
                entity.SwapViewEntity();

                var xNode = xLinks.FindLowest(entity);
                var yNode = yLinks.FindLowest(entity);

                #region xLink
                for (int i = 0; i < 2; i++)
                {
                    var curNode = i == 0 ? xNode.Next : xNode.Previous;
                    while (curNode != null)
                    {
                        var distance = AbsDistance(xNode, curNode);
                        if (distance > entity.ViewDistance)
                        {
                            break;
                        }
                        else
                        {
                            //X轴距离之内的实体
                            entity.ViewEntities.Add(curNode.Value);
                        }
                        curNode = i == 0 ? curNode.Next : curNode.Previous;
                    }
                }
                #endregion

                #region yLink
                for (int i = 0; i < 2; i++)
                {
                    var curNode = i == 0 ? yNode.Next : yNode.Previous;
                    while (curNode != null)
                    {
                        var distance = AbsDistance(yNode, curNode);
                        if (distance > entity.ViewDistance)
                        {
                            break;
                        }
                        else
                        {
                            //Y轴距离之内的实体
                            entity.ViewEntities.Add(curNode.Value);
                        }
                        curNode = i == 0 ? curNode.Next : curNode.Previous;
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// 获取临近的对象；
        /// </summary>
        /// <param name="key">查找的key</param>
        /// <param name="viewDistance">可视距离</param>
        /// <param name="values">临近对象值集合</param>
        public void GetNeighbors(long key, float viewDistance, ref HashSet<T> values)
        {
            if (values == null)
                return;
            if (entityDict.TryGetValue(key, out var entity))
            {
                var xNode = xLinks.FindLowest(entity);
                var yNode = yLinks.FindLowest(entity);
                #region xLink
                for (int i = 0; i < 2; i++)
                {
                    var curNode = i == 0 ? xNode.Next : xNode.Previous;
                    while (curNode != null)
                    {
                        var distance = AbsDistance(xNode, curNode);
                        if (distance > viewDistance)
                        {
                            break;
                        }
                        else
                        {
                            values.Add(curNode.Value.Handle);
                        }
                        curNode = i == 0 ? curNode.Next : curNode.Previous;
                    }
                }
                #endregion

                #region yLink
                for (int i = 0; i < 2; i++)
                {
                    var curNode = i == 0 ? yNode.Next : yNode.Previous;
                    while (curNode != null)
                    {
                        var distance = AbsDistance(yNode, curNode);
                        if (distance > viewDistance)
                        {
                            break;
                        }
                        else
                        {
                            values.Add(curNode.Value.Handle);
                        }
                        curNode = i == 0 ? curNode.Next : curNode.Previous;
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// 获取临近的对象；
        /// </summary>
        /// <param name="key">查找的key</param>
        /// <param name="viewDistance">可视距离</param>
        /// <param name="entities">临近对象实体集合</param>
        public void GetNeighbors(long key, float viewDistance, ref HashSet<AOIEntity> entities)
        {
            if (entities == null)
                return;
            if (entityDict.TryGetValue(key, out var entity))
            {
                var xNode = xLinks.FindLowest(entity);
                var yNode = yLinks.FindLowest(entity);
                #region xLink
                for (int i = 0; i < 2; i++)
                {
                    var curNode = i == 0 ? xNode.Next : xNode.Previous;
                    while (curNode != null)
                    {
                        var distance = AbsDistance(xNode, curNode);
                        if (distance > viewDistance)
                        {
                            break;
                        }
                        else
                        {
                            entities.Add(curNode.Value);
                        }
                        curNode = i == 0 ? curNode.Next : curNode.Previous;
                    }
                }
                #endregion

                #region yLink
                for (int i = 0; i < 2; i++)
                {
                    var curNode = i == 0 ? yNode.Next : yNode.Previous;
                    while (curNode != null)
                    {
                        var distance = AbsDistance(yNode, curNode);
                        if (distance > viewDistance)
                        {
                            break;
                        }
                        else
                        {
                            entities.Add(curNode.Value);
                        }
                        curNode = i == 0 ? curNode.Next : curNode.Previous;
                    }
                }
                #endregion
            }
        }
        double AbsDistance(AOISkipList<AOIEntity, float>.AOISkipListNode a, AOISkipList<AOIEntity, float>.AOISkipListNode b)
        {
            var xDiff = Math.Abs(a.Value.PositionX - b.Value.PositionX);
            var yDiff = Math.Abs(a.Value.PositionY - b.Value.PositionY);
            return Math.Pow(xDiff * xDiff + yDiff * yDiff, 0.5);
        }
        AOIEntity AcquireEntity(long key, T value)
        {
            AOIEntity entity = null;
            if (entityCacheQueue.Count > 0)
            {
                entity = entityCacheQueue.Dequeue();
                entity.Handle = value;
                entity.EntityKey = key;
                return entity;
            }
            entity = new AOIEntity();
            entity.Handle = value;
            entity.EntityKey = key;
            return entity;
        }
        void ReleaseEntity(AOIEntity entity)
        {
            entity.Dispose();
            entityCacheQueue.Enqueue(entity);
        }
    }
}