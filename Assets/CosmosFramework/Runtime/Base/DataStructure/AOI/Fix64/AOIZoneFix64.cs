using System;
using System.Collections.Generic;
using FixMath.NET;
namespace Cosmos
{
    /// <summary>
    ///跳表实现的AOIZone； 
    /// </summary>
    public partial class AOIZoneFix64<T>
        where T : class
    {
        /// <summary>
        /// 实体字典；
        /// </summary>
        readonly Dictionary<long, AOIEntity> entityDict;
        /// <summary>
        /// 实体缓存队列；
        /// </summary>
        readonly Queue<AOIEntity> entityCacheQueue;
        /// <summary>
        /// X轴跳表；
        /// </summary>
        readonly AOISkipList<AOIEntity, Fix64> xLinks;
        /// <summary>
        /// Y轴跳表；
        /// </summary>
        readonly AOISkipList<AOIEntity, Fix64> yLinks;
        public AOISkipList<AOIEntity, Fix64> YLinks { get { return yLinks; } }
        /// <summary>
        /// 当前AOI的矩形区域；
        /// </summary>
        public Rectangle AOIZoneArea { get; private set; }
        public int EntityCount { get { return entityDict.Count; } }
        public IEnumerable<AOIEntity> Entities { get { return entityDict.Values; } }
        public AOIZoneFix64(Fix64 width, Fix64 height) : this(width, height, Fix64.Zero, Fix64.Zero) { }
        public AOIZoneFix64(Fix64 sideLength, Fix64 centerX, Fix64 centerY)
            : this(sideLength, sideLength, centerX, centerY) { }
        public AOIZoneFix64(Fix64 sideLength) : this(sideLength, sideLength, Fix64.Zero, Fix64.Zero) { }
        public AOIZoneFix64(Fix64 width, Fix64 height, Fix64 centerX, Fix64 centerY)
        {
            AOIZoneArea = new Rectangle(centerX, centerY, width, height);
            entityDict = new Dictionary<long, AOIEntity>();
            entityCacheQueue = new Queue<AOIEntity>();
            xLinks = new AOISkipList<AOIEntity, Fix64>(t => t.PositionX);
            yLinks = new AOISkipList<AOIEntity, Fix64>(t => t.PositionY);
        }
        public bool IsOverlapping(Fix64 posX, Fix64 posY)
        {
            if (posX < AOIZoneArea.Left || posX > AOIZoneArea.Right) return false;
            if (posY < AOIZoneArea.Bottom || posY > AOIZoneArea.Top) return false;
            return true;
        }
        public bool TryAdd(long key, T obj)
        {
            return TryAdd(key, obj, Fix64.One);
        }
        public bool TryAdd(long key, T obj, Fix64 viewDistance)
        {
            return TryAdd(key, obj, viewDistance, Fix64.Zero, Fix64.Zero);
        }
        public bool TryAdd(long key, T obj, Fix64 viewDistance, Fix64 posX, Fix64 posY)
        {
            if (!entityDict.ContainsKey(key))
            {
                if (!IsOverlapping(posX, posY))
                    return false;
                var entity = AcquireEntity(key, obj);
                entity.PositionX = posX;
                entity.PositionY = posY;
                xLinks.Add(entity);
                yLinks.Add(entity);
                var xNode = xLinks.FindLowest(entity);
                var yNode = yLinks.FindLowest(entity);
                entity.XNode = xNode;
                entity.YNode = yNode;
                entityDict.Add(key, entity);
                entity.ViewDistance = viewDistance;
                CheckEntitysNeighbor(entity);
                return true;
            }
            return false;
        }
        public void AddOrUpdate(long key, T obj)
        {
            AddOrUpdate(key, obj, Fix64.One);
        }
        public void AddOrUpdate(long key, T obj, Fix64 viewDistance)
        {
            AddOrUpdate(key, obj, viewDistance, Fix64.Zero, Fix64.Zero);
        }
        public void AddOrUpdate(long key, T obj, Fix64 viewDistance, Fix64 posX, Fix64 posY)
        {
            if (!IsOverlapping(posX, posY))
                return;
            var entity = AcquireEntity(key, obj);
            xLinks.Remove(entity);
            entity.PositionX = posX;
            xLinks.Add(entity);
            yLinks.Remove(entity);
            entity.PositionY = posY;
            yLinks.Add(entity);
            var xNode = xLinks.FindLowest(entity);
            var yNode = yLinks.FindLowest(entity);
            entity.XNode = xNode;
            entity.YNode = yNode;
            entityDict[key] = entity;
            entity.ViewDistance = viewDistance;
            CheckEntitysNeighbor(entity);
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
                entity.SwapViewEntity();
                CheckEntitysNeighbor(entity);
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
            if (rst)
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
        /// 刷新位置；
        /// </summary>
        /// <param name="key">实体的key</param>
        /// <param name="posX">X方向新的位置点</param>
        /// <param name="poxY">Y方向新的位置点</param>
        public void Refresh(long key, Fix64 posX, Fix64 poxY)
        {
            if (entityDict.TryGetValue(key, out var entity))
            {
                if (!IsOverlapping(posX, poxY))
                    return;
                xLinks.Remove(entity);
                entity.PositionX = posX;
                xLinks.Add(entity);
                yLinks.Remove(entity);
                entity.PositionY = poxY;
                yLinks.Add(entity);
                entity.SwapViewEntity();
                CheckEntitysNeighbor(entity);
            }
        }
        /// <summary>
        /// 对AOIZone内的实体对象进行强制更新；
        /// </summary>
        public void Refresh()
        {
            foreach (var entity in entityDict.Values)
            {
                xLinks.Remove(entity);
                xLinks.Add(entity);
                yLinks.Remove(entity);
                yLinks.Add(entity);
                entity.SwapViewEntity();
                CheckEntitysNeighbor(entity);
            }
        }
        /// <summary>
        /// 移动；
        /// </summary>
        /// <param name="key">实体的key</param>
        /// <param name="posX">X方向新的位置点</param>
        /// <param name="poxY">Y方向新的位置点</param>
        public void Move(long key, Fix64 posX, Fix64 poxY)
        {
            if (entityDict.TryGetValue(key, out var entity))
            {
                bool isMoved = false;
                if (!IsOverlapping(posX, poxY))
                    return;
                if ((entity.PositionX - posX) != Fix64.Zero)
                {
                    xLinks.Remove(entity);
                    entity.PositionX = posX;
                    xLinks.Add(entity);
                    isMoved = true;
                }
                if ((entity.PositionY - poxY) != Fix64.Zero)
                {
                    yLinks.Remove(entity);
                    entity.PositionY = poxY;
                    yLinks.Add(entity);
                    isMoved = true;
                }
                if (!isMoved)
                    return;
                entity.SwapViewEntity();
                CheckEntitysNeighbor(entity);
            }
        }
        /// <summary>
        /// 获取临近的对象；
        /// </summary>
        /// <param name="key">查找的key</param>
        /// <param name="viewDistance">可视距离</param>
        /// <param name="values">临近对象值集合</param>
        public void GetNeighbors(long key, Fix64 viewDistance, ref HashSet<T> values)
        {
            if (values == null)
                return;
            if (entityDict.TryGetValue(key, out var entity))
            {
                GetNeighborNodeValue(entity, viewDistance, ref values);
                values.Remove(entity.Handle);
            }
        }
        /// <summary>
        /// 获取临近的对象；
        /// </summary>
        /// <param name="key">查找的key</param>
        /// <param name="viewDistance">可视距离</param>
        /// <param name="entities">临近对象实体集合</param>
        public void GetNeighbors(long key, Fix64 viewDistance, ref HashSet<AOIEntity> entities)
        {
            if (entities == null)
                return;
            if (entityDict.TryGetValue(key, out var entity))
            {
                GetNeighborNodeEntities(entity, viewDistance, ref entities);
                entities.Remove(entity);
            }
        }
        public void GetNeighbors(long key, ref HashSet<AOIEntity> entities)
        {
            if (entities == null)
                return;
            if (entityDict.TryGetValue(key, out var entity))
            {
                GetNeighborNodeEntities(entity, entity.ViewDistance, ref entities);
                entities.Remove(entity);
            }
        }
        public void GetNeighbors(long key, ref HashSet<T> values)
        {
            if (values == null)
                return;
            if (entityDict.TryGetValue(key, out var entity))
            {
                GetNeighborNodeValue(entity, entity.ViewDistance, ref values);
                values.Remove(entity.Handle);
            }
        }
        /// <summary>
        ///通过临近坐标获取附近的实体； 
        /// </summary>
        public void GetNeighbors(Fix64 posX, Fix64 posY, Fix64 viewDistance, ref HashSet<AOIEntity> entities)
        {
            TryAdd(long.MinValue, default(T), posX, posY, viewDistance);
            GetNeighbors(long.MinValue, viewDistance, ref entities);
            Remove(long.MinValue, out _);
        }
        public void GetNeighbors(Fix64 posX, Fix64 poxY, Fix64 viewDistance, ref HashSet<T> values)
        {
            TryAdd(long.MinValue, default(T), posX, poxY, viewDistance);
            GetNeighbors(long.MinValue, viewDistance, ref values);
            Remove(long.MinValue, out _);
        }
        /// <summary>
        ///右值实体是否在左值实体的视野范围之内；
        /// </summary>
        /// <param name="lhskey">左值Key</param>
        /// <param name="rhsKey">右值Key</param>
        /// <param name="lhsViewDistance">左值的视野距离</param>
        /// <returns>是否在视野内</returns>
        public bool IsEntityInView(long lhskey, long rhsKey, Fix64 lhsViewDistance)
        {
            if (!PeekEntity(lhskey, out var lhsEntity))
                return false;
            if (!PeekEntity(rhsKey, out var rhsEntity))
                return false;
            var lhsXNode = xLinks.FindLowest(lhsEntity);
            var lhsYNode = yLinks.FindLowest(lhsEntity);
            var rhsXNode = xLinks.FindLowest(rhsEntity);
            var rhsYNode = yLinks.FindLowest(rhsEntity);
            var xDistance = Math.Abs((double)(lhsXNode.Value.PositionX - rhsXNode.Value.PositionX));
            if ((Fix64)xDistance > lhsViewDistance) return false;
            var yDistance = Math.Abs((double)(lhsYNode.Value.PositionY - rhsYNode.Value.PositionY));
            if ((Fix64)yDistance > lhsViewDistance) return false;
            return true;
        }
        /// <summary>
        ///右值实体是否在左值实体的视野范围之内；
        /// </summary>
        /// <param name="lhskey">左值Key</param>
        /// <param name="rhsKey">右值Key</param>
        /// <returns>是否在视野内</returns>
        public bool IsEntityInView(long lhskey, long rhsKey)
        {
            if (!PeekEntity(lhskey, out var lhsEntity))
                return false;
            if (!PeekEntity(rhsKey, out var rhsEntity))
                return false;
            var lhsXNode = xLinks.FindLowest(lhsEntity);
            var lhsYNode = yLinks.FindLowest(lhsEntity);
            var rhsXNode = xLinks.FindLowest(rhsEntity);
            var rhsYNode = yLinks.FindLowest(rhsEntity);
            var xDistance = Math.Abs((double)(lhsXNode.Value.PositionX - rhsXNode.Value.PositionX));
            if ((Fix64)xDistance > lhsEntity.ViewDistance) return false;
            var yDistance = Math.Abs((double)(lhsYNode.Value.PositionY - rhsYNode.Value.PositionY));
            if ((Fix64)yDistance > lhsEntity.ViewDistance) return false;
            return true;
        }
        AOIEntity AcquireEntity(long key, T value)
        {
            AOIEntity entity = null;
            if (entityCacheQueue.Count > 0)
                entity = entityCacheQueue.Dequeue();
            else
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
        void CheckEntitysNeighbor(AOIEntity entity)
        {
            var dstNode = xLinks.FindLowest(entity);
            if (dstNode == null)
                return;
            for (int i = 0; i < 2; i++)
            {
                var curNode = i == 0 ? dstNode.Next : dstNode.Previous;
                while (curNode != null)
                {
                    if (!curNode.IsFooter() && !curNode.IsHeader())
                    {
                        var xDistance = Math.Abs((double)(dstNode.Value.PositionX - curNode.Value.PositionX));
                        if (xDistance > (double)entity.ViewDistance)
                        {
                            break;
                        }
                        else
                        {
                            var yDistance = Math.Abs((double)(dstNode.Value.PositionY - curNode.Value.PositionY));
                            if (yDistance <= (double)entity.ViewDistance)
                            {
                                if (curNode.Value.EntityKey != dstNode.Value.EntityKey)
                                    entity.ViewEntities.Add(curNode.Value);
                            }
                        }
                    }
                    curNode = i == 0 ? curNode.Next : curNode.Previous;
                }
            }
        }
        void GetNeighborNodeEntities(AOIEntity entity, Fix64 viewDistance, ref HashSet<AOIEntity> entities)
        {
            var dstNode = xLinks.FindLowest(entity);
            if (dstNode == null)
                return;
            for (int i = 0; i < 2; i++)
            {
                var curNode = i == 0 ? dstNode.Next : dstNode.Previous;
                while (curNode != null)
                {
                    if (!curNode.IsFooter() && !curNode.IsHeader())
                    {
                        var xDistance = Math.Abs((double)(dstNode.Value.PositionX - curNode.Value.PositionX));
                        if (xDistance > (double)viewDistance)
                        {
                            break;
                        }
                        else
                        {
                            var yDistance = Math.Abs((double)(dstNode.Value.PositionY - curNode.Value.PositionY));
                            if (yDistance < (double)viewDistance)
                            {
                                entities.Add(curNode.Value);
                            }
                        }
                    }
                    curNode = i == 0 ? curNode.Next : curNode.Previous;
                }
            }
        }
        void GetNeighborNodeValue(AOIEntity entity, Fix64 viewDistance, ref HashSet<T> values)
        {
            var dstNode = xLinks.FindLowest(entity);
            if (dstNode == null)
                return;
            for (int i = 0; i < 2; i++)
            {
                var curNode = i == 0 ? dstNode.Next : dstNode.Previous;
                while (curNode != null)
                {
                    if (!curNode.IsFooter() && !curNode.IsHeader())
                    {
                        var xDistance = Math.Abs((double)(dstNode.Value.PositionX - curNode.Value.PositionX));
                        if (xDistance > (double)viewDistance)
                        {
                            break;
                        }
                        else
                        {
                            var yDistance = Math.Abs((double)(dstNode.Value.PositionY - curNode.Value.PositionY));
                            if (yDistance < (double)viewDistance)
                            {
                                values.Add(curNode.Value.Handle);
                            }
                        }
                    }
                    curNode = i == 0 ? curNode.Next : curNode.Previous;
                }
            }
        }
    }
}
