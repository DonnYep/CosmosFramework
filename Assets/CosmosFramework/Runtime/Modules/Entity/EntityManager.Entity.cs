using System;
using System.Collections.Generic;
using System.Linq;
namespace Cosmos.Entity
{
    internal sealed partial class EntityManager
    {
        /// <summary>
        /// 实体（注册项）；
        /// <para>一个实体对应一组可复用实例，实例载体由注册时决定：</para>
        /// <para>1、预制体注册：载体是 <see cref="EntityObject"/>（持有 GameObject）；</para>
        /// <para>2、逻辑注册：载体是 <see cref="EntityLogic"/>（不持有 GameObject）。</para>
        /// <para>刷新列表按载体是否实现刷新接口按需登记，未实现刷新接口的实例不进入列表。</para>
        /// </summary>
        private class Entity
        {
            static readonly Pool<Entity> entityPool = new Pool<Entity>(() => { return new Entity(); }, e => { e.Release(); });
            static readonly Action<IEntityObject> emptyReleaser = e => { };

            /// <summary>
            /// 实例池：隐藏的实例留在池中复用，只有 Release 才真正销毁；
            /// </summary>
            readonly Pool<IEntityObject> entityObjectPool;
            /// <summary>
            /// 存活实例；key=>实体实例编号；
            /// </summary>
            readonly Dictionary<int, IEntityObject> entityObjectDict;
            /// <summary>
            /// 刷新列表（逐帧 / 固定帧 / 帧末）；只有实现了对应接口的实例才会在其中，
            /// 列表顺序即派发顺序。
            /// </summary>
            readonly List<IEntityTickable> tickList;
            readonly Dictionary<int, int> tickIndexDict;
            readonly List<IEntityFixedTickable> fixedTickList;
            readonly Dictionary<int, int> fixedTickIndexDict;
            readonly List<IEntityLateTickable> lateTickList;
            readonly Dictionary<int, int> lateTickIndexDict;
            /// <summary>
            /// 刷新期间产生的待回收实例：刷新中隐藏不立即摘表/入池，避免打乱正在遍历的列表；
            /// 刷新结束后统一处理。保证"派发顺序稳定 + 隐藏后本帧不再被派发"。
            /// </summary>
            readonly List<IEntityObject> pendingRecycleList;
            readonly HashSet<int> pendingRecycleIdSet;

            Func<IEntityObject> objectGenerator;
            Action<IEntityObject> objectReleaser;

            int spawnIndex;
            bool refreshing;

            public string EntityName { get; private set; }
            public Type EntityObjectType { get; private set; }
            public string EntityGroupName { get; set; }
            public int EntityObjectCount { get { return entityObjectDict.Count; } }

            public Entity()
            {
                entityObjectPool = new Pool<IEntityObject>(GenerateEntityObject, DespawnEntityObject);
                entityObjectDict = new Dictionary<int, IEntityObject>();
                tickList = new List<IEntityTickable>();
                tickIndexDict = new Dictionary<int, int>();
                fixedTickList = new List<IEntityFixedTickable>();
                fixedTickIndexDict = new Dictionary<int, int>();
                lateTickList = new List<IEntityLateTickable>();
                lateTickIndexDict = new Dictionary<int, int>();
                pendingRecycleList = new List<IEntityObject>();
                pendingRecycleIdSet = new HashSet<int>();
            }
            /// <summary>
            /// 生成一个实例并登记到刷新列表。
            /// </summary>
            public IEntityObject ShowEntityObject()
            {
                var entityObject = entityObjectPool.Spawn();
                entityObjectDict[entityObject.EntityObjectId] = entityObject;
                AddRefresh(entityObject);
                return entityObject;
            }
            /// <summary>
            /// 隐藏（回收进池）指定实例；刷新期间调用会被延迟到本次刷新结束。
            /// </summary>
            /// <param name="entityObjectId">实体实例编号</param>
            /// <returns>是否成功</returns>
            public bool HideEntityObject(int entityObjectId)
            {
                if (!entityObjectDict.Remove(entityObjectId, out var entityObject))
                    return false;
                entityObject.OnHide();
                if (refreshing)
                {
                    pendingRecycleList.Add(entityObject);
                    pendingRecycleIdSet.Add(entityObjectId);
                    return true;
                }
                RemoveRefresh(entityObjectId);
                entityObjectPool.Despawn(entityObject);
                return true;
            }
            /// <summary>
            /// 隐藏此实体下所有存活实例。
            /// </summary>
            public void HideAllEntityObject()
            {
                var count = entityObjectDict.Count;
                if (count == 0)
                    return;
                var ids = new int[count];
                entityObjectDict.Keys.CopyTo(ids, 0);
                for (int i = 0; i < count; i++)
                    HideEntityObject(ids[i]);
            }
            /// <summary>
            /// 是否存在指定实例。
            /// </summary>
            public bool HasEntityObject(int entityObjectId)
            {
                return entityObjectDict.ContainsKey(entityObjectId);
            }
            /// <summary>
            /// 获取指定实例。
            /// </summary>
            public bool GetEntityObject(int entityObjectId, out IEntityObject entityObject)
            {
                return entityObjectDict.TryGetValue(entityObjectId, out entityObject);
            }
            /// <summary>
            /// 逐帧刷新；
            /// <para>单个实例抛异常不会中断同批其他实例：本帧全部派发完成后以 AggregateException 上抛，
            /// 且刷新状态与待回收队列在 finally 中复位，不会污染下一帧。</para>
            /// </summary>
            public void Refresh(float deltaTime)
            {
                refreshing = true;
                List<Exception> exceptions = null;
                try
                {
                    var count = tickList.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var tickable = tickList[i];
                        if (tickable.Paused)
                            continue;
                        //本帧中途被隐藏的实例，后续不应再被派发
                        if (pendingRecycleIdSet.Count > 0 && pendingRecycleIdSet.Contains(tickable.EntityObjectId))
                            continue;
                        try
                        {
                            tickable.OnEntityTick(deltaTime);
                        }
                        catch (Exception e)
                        {
                            if (exceptions == null)
                                exceptions = new List<Exception>();
                            exceptions.Add(e);
                        }
                    }
                }
                finally
                {
                    refreshing = false;
                    FlushPendingRecycle();
                }
                ThrowIfAny(exceptions);
            }
            /// <summary>
            /// 固定帧刷新。
            /// </summary>
            public void FixedRefresh(float deltaTime)
            {
                refreshing = true;
                List<Exception> exceptions = null;
                try
                {
                    var count = fixedTickList.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var tickable = fixedTickList[i];
                        if (tickable.Paused)
                            continue;
                        if (pendingRecycleIdSet.Count > 0 && pendingRecycleIdSet.Contains(tickable.EntityObjectId))
                            continue;
                        try
                        {
                            tickable.OnEntityFixedTick(deltaTime);
                        }
                        catch (Exception e)
                        {
                            if (exceptions == null)
                                exceptions = new List<Exception>();
                            exceptions.Add(e);
                        }
                    }
                }
                finally
                {
                    refreshing = false;
                    FlushPendingRecycle();
                }
                ThrowIfAny(exceptions);
            }
            /// <summary>
            /// 帧末刷新。
            /// </summary>
            public void LateRefresh(float deltaTime)
            {
                refreshing = true;
                List<Exception> exceptions = null;
                try
                {
                    var count = lateTickList.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var tickable = lateTickList[i];
                        if (tickable.Paused)
                            continue;
                        if (pendingRecycleIdSet.Count > 0 && pendingRecycleIdSet.Contains(tickable.EntityObjectId))
                            continue;
                        try
                        {
                            tickable.OnEntityLateTick(deltaTime);
                        }
                        catch (Exception e)
                        {
                            if (exceptions == null)
                                exceptions = new List<Exception>();
                            exceptions.Add(e);
                        }
                    }
                }
                finally
                {
                    refreshing = false;
                    FlushPendingRecycle();
                }
                ThrowIfAny(exceptions);
            }
            /// <summary>
            /// 冻结 / 解冻此实体下所有存活实例。
            /// </summary>
            public void SetGroupPaused(bool paused)
            {
                foreach (var entityObject in entityObjectDict.Values)
                    entityObject.SetGroupPaused(paused);
            }
            /// <summary>
            /// 获取实体信息。
            /// </summary>
            public EntityInfo GetEntityInfo()
            {
                var entityInfo = new EntityInfo(EntityName, entityObjectDict.Count, entityObjectDict.Keys.ToArray());
                return entityInfo;
            }
            /// <summary>
            /// 清空并归还对象池；存活实例与池内实例都会交给释放器处理。
            /// </summary>
            public void Release()
            {
                var releaser = objectReleaser;
                if (releaser != null)
                {
                    foreach (var entityObject in entityObjectDict.Values)
                        releaser(entityObject);
                    foreach (var entityObject in entityObjectPool)
                        releaser(entityObject);
                }
                entityObjectDict.Clear();
                ClearRefreshList(tickList, tickIndexDict);
                ClearRefreshList(fixedTickList, fixedTickIndexDict);
                ClearRefreshList(lateTickList, lateTickIndexDict);
                pendingRecycleList.Clear();
                pendingRecycleIdSet.Clear();
                entityObjectPool.Clear();

                EntityName = string.Empty;
                EntityObjectType = null;
                EntityGroupName = string.Empty;
                objectGenerator = null;
                objectReleaser = null;
                spawnIndex = 0;
                refreshing = false;
            }
            /// <summary>
            /// 创建实体（注册项）。
            /// </summary>
            public static Entity Create(string entityName, string entityGroupName, Type entityObjectType, Func<IEntityObject> objectGenerator, Action<IEntityObject> objectReleaser)
            {
                var entity = entityPool.Spawn();
                entity.EntityName = entityName;
                entity.EntityGroupName = entityGroupName;
                entity.EntityObjectType = entityObjectType;
                entity.objectGenerator = objectGenerator;
                entity.objectReleaser = objectReleaser ?? emptyReleaser;
                return entity;
            }
            /// <summary>
            /// 归还实体（注册项）到池。
            /// </summary>
            public static void Release(Entity entity)
            {
                entityPool.Despawn(entity);
            }
            /// <summary>
            /// 清空实体池；模块终止时调用，避免静态池继续持有已释放的注册项。
            /// </summary>
            public static void ClearPool()
            {
                entityPool.Clear();
            }
            IEntityObject GenerateEntityObject()
            {
                var entityObject = objectGenerator();
                entityObject.OnInit(EntityName, spawnIndex++, EntityGroupName);
                return entityObject;
            }
            void DespawnEntityObject(IEntityObject entityObject)
            {
                entityObject.OnRecycle();
            }
            void FlushPendingRecycle()
            {
                var count = pendingRecycleList.Count;
                if (count == 0)
                    return;
                for (int i = 0; i < count; i++)
                {
                    var entityObject = pendingRecycleList[i];
                    RemoveRefresh(entityObject.EntityObjectId);
                    entityObjectPool.Despawn(entityObject);
                }
                pendingRecycleList.Clear();
                pendingRecycleIdSet.Clear();
            }
            void AddRefresh(IEntityObject entityObject)
            {
                var entityObjectId = entityObject.EntityObjectId;
                if (entityObject is IEntityTickable tickable)
                {
                    tickIndexDict[entityObjectId] = tickList.Count;
                    tickList.Add(tickable);
                }
                if (entityObject is IEntityFixedTickable fixedTickable)
                {
                    fixedTickIndexDict[entityObjectId] = fixedTickList.Count;
                    fixedTickList.Add(fixedTickable);
                }
                if (entityObject is IEntityLateTickable lateTickable)
                {
                    lateTickIndexDict[entityObjectId] = lateTickList.Count;
                    lateTickList.Add(lateTickable);
                }
            }
            void RemoveRefresh(int entityObjectId)
            {
                RemoveFromRefreshList(tickList, tickIndexDict, entityObjectId);
                RemoveFromRefreshList(fixedTickList, fixedTickIndexDict, entityObjectId);
                RemoveFromRefreshList(lateTickList, lateTickIndexDict, entityObjectId);
            }
            static void RemoveFromRefreshList<T>(List<T> list, Dictionary<int, int> indexDict, int entityObjectId) where T : IEntityObject
            {
                if (!indexDict.TryGetValue(entityObjectId, out var index))
                    return;
                indexDict.Remove(entityObjectId);
                var lastIndex = list.Count - 1;
                if (index != lastIndex)
                {
                    var lastItem = list[lastIndex];
                    list[index] = lastItem;
                    indexDict[lastItem.EntityObjectId] = index;
                }
                list.RemoveAt(lastIndex);
            }
            static void ClearRefreshList<T>(List<T> list, Dictionary<int, int> indexDict)
            {
                list.Clear();
                indexDict.Clear();
            }
            static void ThrowIfAny(List<Exception> exceptions)
            {
                if (exceptions == null)
                    return;
                var array = exceptions.ToArray();
                exceptions.Clear();
                throw new AggregateException(array);
            }
        }
    }
}
