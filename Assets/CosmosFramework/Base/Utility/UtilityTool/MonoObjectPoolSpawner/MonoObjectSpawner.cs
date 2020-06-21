using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    /// <summary>
    /// 对象生成脚本，每个脚本自身就是一个Key
    /// </summary>
    public abstract class MonoObjectSpawner : MonoBehaviour
    {
        // TODO 多对象生成需要定义新的数据结构，例如链表。由此可实现单Key的多对象生成。
        public  virtual float CollectDelay { get; }
        public virtual HashSet<MonoObjectBase> UncollectibleHashSet { get;protected set; }
        protected GameObject deactiveObjectMount;
        protected GameObject activeObjectMount;
        protected ObjectKey<MonoObjectItem> objectKey;
        protected ObjectPoolVariable poolVariable;
        public Transform DeactiveObjectMount
        {
            get
            {
                if (deactiveObjectMount == null)
                {
                    deactiveObjectMount = new GameObject(this.gameObject.name + "->>DeactiveObjectMount");
                    deactiveObjectMount.transform.SetParent(Facade.GetModule(CFModules.OBJECTPOOL).ModuleMountObject.transform);
                    deactiveObjectMount.transform.ResetLocalTransform();
                }
                return deactiveObjectMount.transform;
            }
        }
        public Transform ActiveObjectMount
        {
            get
            {
                if (activeObjectMount == null)
                {
                    activeObjectMount = new GameObject(this.gameObject.name + "->>ActiveObjectMount");
                    activeObjectMount.transform.SetParent(Facade.GetModule(CFModules.OBJECTPOOL).ModuleMountObject.transform);
                    activeObjectMount.transform.ResetLocalTransform();
                }
                return activeObjectMount.transform;
            }
        }
        protected virtual void Awake()
        {
            RegisterSpawner();
        }
        protected virtual void OnDestroy()
        {
            DeregisterSpawner();
        }
        /// <summary>
        /// 每次产生都会进行一个概率检测
        /// </summary>
        public abstract void Spawn();
        /// <summary>
        /// SpawnUncollectable的对象，直接实例化，挂载在对象池中，空函数
        /// </summary>
        public virtual void SpawnUncollectible() { }
        /// <summary>
        /// 回收Uncollectable的对象，直接被销毁
        /// </summary>
        public virtual void DespawnUncollectible()
        {
            Cosmos.GameManager.KillObjects(UncollectibleHashSet);
        }
        protected abstract MonoObjectBase Create();
        protected abstract void RegisterSpawner();
        /// <summary>
        /// 注销并销毁
        /// </summary>
        protected virtual void DeregisterSpawner()
        {
            Facade.DeregisterObjectSpawnPool(objectKey);
            GameManager.KillObject(deactiveObjectMount);
        }
        protected virtual void SpawnHandler(IObject obj)
        {
            var go = obj as MonoObjectItem;
            if (go == null)
                return;
            Facade.StartCoroutine(EnumCollect(CollectDelay, () => Facade.DespawnObject(objectKey, go)));
            Utility.DebugLog("MonoObjectSpawner SpawnHandler");
        }
        protected virtual  void DespawnHandler(IObject obj)
        {
            var go = obj as MonoObjectItem;
            if (go == null)
                return;
            go.transform.SetParent(DeactiveObjectMount);
            go.transform.ResetLocalTransform();
            Utility.DebugLog("MonoObjectSpawner  DespawnHandler");

        }
        public virtual void ClearAll()
        {
            Facade.ClearObjectSpawnPool(objectKey);
        }
        protected  IEnumerator EnumCollect(float delay,CFAction action=null)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
        /// <summary>
        /// 空函数
        /// </summary>
        protected virtual void OnValidate() { }
        /// <summary>
        /// 默认判断是否有对齐对象,
        /// 执行的时候默认会将生成对象作为对齐目标的子物体。
        /// 执行结束自动作为ActiveObjectMount的子对象。
        /// 默认将active false的对象变成true
        /// </summary>
        /// <param name="go">生成的对象</param>
        /// <param name="trans">对齐对象</param>
        protected void AlignObject(ObjectSpawnAlignType alignType,MonoObjectBase go,Transform trans)
        {
            if (trans == null)
                return;
            go.transform.SetParent(trans);
            go.gameObject.SetActive(true);
            switch (alignType)
            {
                case ObjectSpawnAlignType.AlignTransform:
                    go.transform.ResetLocalTransform();
                    break;
                case ObjectSpawnAlignType.AlignPosition:
                    go.transform.position = trans.position;
                    break;
                case ObjectSpawnAlignType.AlignPositionRotation:
                    go.transform.position = trans.position;
                    go.transform.rotation = trans.rotation;
                    break;
                case ObjectSpawnAlignType.AlignPositionScale:
                    go.transform.position = trans.position;
                    go.transform.localScale = trans.localScale;
                    break;
                case ObjectSpawnAlignType.AlignRotationScale:
                    go.transform.rotation = trans.rotation;
                    go.transform.localScale = trans.localScale;
                    break;
            }
            go.transform.SetParent(ActiveObjectMount);
        }
        //TODO Spawn后随机旋转
        protected void AlignObject(ObjectPoolDataSet poolDataSet, MonoObjectBase go, Transform trans)
        {
            ObjectSpawnAlignType alignType = poolDataSet.AlignType;
            if (trans == null)
                return;
            go.transform.SetParent(trans);
            go.gameObject.SetActive(true);
            switch (alignType)
            {
                case ObjectSpawnAlignType.AlignTransform:
                    go.transform.ResetLocalTransform();
                    break;
                case ObjectSpawnAlignType.AlignPosition:
                    go.transform.position = trans.position;
                    break;
                case ObjectSpawnAlignType.AlignPositionRotation:
                    go.transform.position = trans.position;
                    go.transform.rotation = trans.rotation;
                    break;
                case ObjectSpawnAlignType.AlignPositionScale:
                    go.transform.position = trans.position;
                    go.transform.localScale = trans.localScale;
                    break;
                case ObjectSpawnAlignType.AlignRotationScale:
                    go.transform.rotation = trans.rotation;
                    go.transform.localScale = trans.localScale;
                    break;
            }
            go.transform.SetParent(ActiveObjectMount);
        }
    }
  
}

