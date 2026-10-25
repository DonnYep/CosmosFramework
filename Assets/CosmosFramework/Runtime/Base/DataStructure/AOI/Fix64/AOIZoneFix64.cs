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
        /// <summary>
        /// 按坐标查询邻居时使用的临时实体；
        /// 只在查询期间插入两条轴表以定位遍历起点，不进入实体字典，查询结束立即按节点摘除。
        /// </summary>
        readonly AOIEntity queryEntity;
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
            queryEntity = new AOIEntity();
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
            if (entityDict.ContainsKey(key))
                return false;
            if (!IsOverlapping(posX, posY))
                return false;
            var entity = AcquireEntity(key, obj);
            entity.ViewDistance = viewDistance;
            entity.PositionX = posX;
            entity.PositionY = posY;
            entity.XNode = xLinks.Add(entity);
            entity.YNode = yLinks.Add(entity);
            entityDict.Add(key, entity);
            CheckEntitysNeighbor(entity);
            return true;
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
            //已存在的 key 复用原实体：先按节点从两条轴表摘除，再改坐标重新插入，
            //避免旧实体残留在轴表中造成重复计数；
            AOIEntity entity;
            if (entityDict.TryGetValue(key, out entity))
            {
                RemoveFromLinks(entity);
                entity.Handle = obj;
            }
            else
            {
                entity = AcquireEntity(key, obj);
                entityDict.Add(key, entity);
            }
            entity.ViewDistance = viewDistance;
            entity.PositionX = posX;
            entity.PositionY = posY;
            entity.XNode = xLinks.Add(entity);
            entity.YNode = yLinks.Add(entity);
            entity.SwapViewEntity();
            CheckEntitysNeighbor(entity);
        }
        public bool Remove(long key, out T value)
        {
            value = default(T);
            AOIEntity entity;
            if (!entityDict.TryGetValue(key, out entity))
                return false;
            entityDict.Remove(key);
            value = entity.Handle;
            RemoveFromLinks(entity);
            entity.SwapViewEntity();
            CheckEntitysNeighbor(entity);
            ReleaseEntity(entity);
            return true;
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
                RemoveFromLinks(entity);
                entity.PositionX = posX;
                entity.PositionY = poxY;
                entity.XNode = xLinks.Add(entity);
                entity.YNode = yLinks.Add(entity);
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
                RemoveFromLinks(entity);
                entity.XNode = xLinks.Add(entity);
                entity.YNode = yLinks.Add(entity);
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
                    if (entity.XNode != null)
                        xLinks.Remove(entity.XNode);
                    entity.XNode = null;
                    entity.PositionX = posX;
                    entity.XNode = xLinks.Add(entity);
                    isMoved = true;
                }
                if ((entity.PositionY - poxY) != Fix64.Zero)
                {
                    if (entity.YNode != null)
                        yLinks.Remove(entity.YNode);
                    entity.YNode = null;
                    entity.PositionY = poxY;
                    entity.YNode = yLinks.Add(entity);
                    isMoved = true;
                }
                if (!isMoved)
                    return;
                entity.SwapViewEntity();
                CheckEntitysNeighbor(entity);
            }
        }
        public void Clear()
        {
            xLinks.Clear();
            yLinks.Clear();
            entityDict.Clear();
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
            if (entities == null)
                return;
            if (!IsOverlapping(posX, posY))
                return;
            var entity = AcquireQueryEntity(posX, posY);
            try
            {
                GetNeighborNodeEntities(entity, viewDistance, ref entities);
            }
            finally
            {
                ReleaseQueryEntity(entity);
            }
        }
        public void GetNeighbors(Fix64 posX, Fix64 poxY, Fix64 viewDistance, ref HashSet<T> values)
        {
            if (values == null)
                return;
            if (!IsOverlapping(posX, poxY))
                return;
            var entity = AcquireQueryEntity(posX, poxY);
            try
            {
                GetNeighborNodeValue(entity, viewDistance, ref values);
            }
            finally
            {
                ReleaseQueryEntity(entity);
            }
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
            var xDistance = Fix64.Abs(lhsEntity.PositionX - rhsEntity.PositionX);
            if (xDistance > lhsViewDistance) return false;
            var yDistance = Fix64.Abs(lhsEntity.PositionY - rhsEntity.PositionY);
            if (yDistance > lhsViewDistance) return false;
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
            var xDistance = Fix64.Abs(lhsEntity.PositionX - rhsEntity.PositionX);
            if (xDistance > lhsEntity.ViewDistance) return false;
            var yDistance = Fix64.Abs(lhsEntity.PositionY - rhsEntity.PositionY);
            if (yDistance > lhsEntity.ViewDistance) return false;
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
        /// <summary>
        /// 按节点把实体从两条轴表中摘除；
        /// 使用实体自身记录的节点，避免按值反查时删到同坐标的其它实体；
        /// </summary>
        void RemoveFromLinks(AOIEntity entity)
        {
            if (entity.XNode != null)
            {
                xLinks.Remove(entity.XNode);
                entity.XNode = null;
            }
            if (entity.YNode != null)
            {
                yLinks.Remove(entity.YNode);
                entity.YNode = null;
            }
        }
        /// <summary>
        /// 准备按坐标查询用的临时实体，并把它插入两条轴表以确定遍历起点；
        /// </summary>
        AOIEntity AcquireQueryEntity(Fix64 posX, Fix64 posY)
        {
            var entity = queryEntity;
            entity.EntityKey = long.MinValue;
            entity.Handle = default(T);
            entity.PositionX = posX;
            entity.PositionY = posY;
            entity.ViewDistance = Fix64.Zero;
            entity.XNode = xLinks.Add(entity);
            entity.YNode = yLinks.Add(entity);
            return entity;
        }
        /// <summary>
        /// 按节点把临时实体从两条轴表摘除；
        /// </summary>
        void ReleaseQueryEntity(AOIEntity entity)
        {
            if (entity.XNode != null)
            {
                xLinks.Remove(entity.XNode);
                entity.XNode = null;
            }
            if (entity.YNode != null)
            {
                yLinks.Remove(entity.YNode);
                entity.YNode = null;
            }
        }
        void CheckEntitysNeighbor(AOIEntity entity)
        {
            if (entity.XNode == null)
                return;
            for (int i = 0; i < 2; i++)
            {
                var curNode = i == 0 ? entity.XNode.Next : entity.XNode.Previous;
                while (curNode != null)
                {
                    if (!curNode.IsFooter() && !curNode.IsHeader())
                    {
                        var xDistance = Fix64.Abs(entity.PositionX - curNode.Value.PositionX);
                        if (xDistance > entity.ViewDistance)
                        {
                            break;
                        }
                        else
                        {
                            var yDistance = Fix64.Abs(entity.PositionY - curNode.Value.PositionY);
                            if (yDistance <= entity.ViewDistance)
                            {
                                if (!ReferenceEquals(curNode.Value, entity))
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
            if (entity.XNode == null)
                return;
            for (int i = 0; i < 2; i++)
            {
                var curNode = i == 0 ? entity.XNode.Next : entity.XNode.Previous;
                while (curNode != null)
                {
                    if (!curNode.IsFooter() && !curNode.IsHeader())
                    {
                        var xDistance = Fix64.Abs(entity.PositionX - curNode.Value.PositionX);
                        if (xDistance > viewDistance)
                        {
                            break;
                        }
                        else
                        {
                            var yDistance = Fix64.Abs(entity.PositionY - curNode.Value.PositionY);
                            if (yDistance <= viewDistance)
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
            if (entity.XNode == null)
                return;
            for (int i = 0; i < 2; i++)
            {
                var curNode = i == 0 ? entity.XNode.Next : entity.XNode.Previous;
                while (curNode != null)
                {
                    if (!curNode.IsFooter() && !curNode.IsHeader())
                    {
                        var xDistance = Fix64.Abs(entity.PositionX - curNode.Value.PositionX);
                        if (xDistance > viewDistance)
                        {
                            break;
                        }
                        else
                        {
                            var yDistance = Fix64.Abs(entity.PositionY - curNode.Value.PositionY);
                            if (yDistance <= viewDistance)
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
