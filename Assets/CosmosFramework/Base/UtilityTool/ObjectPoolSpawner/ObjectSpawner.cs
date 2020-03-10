using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    /// <summary>
    /// 对象生成脚本，每个脚本自身就是一个Key
    /// </summary>
    public abstract class ObjectSpawner : MonoBehaviour
    {
        public  virtual float CollectDelay { get; }
        public virtual HashSet<GameObject> UncollectableHashSet { get;protected set; }
        protected GameObject deactiveObjectMount;
        public Transform DeactiveObjectMount
        {
            get
            {
                if (deactiveObjectMount == null)
                {
                    deactiveObjectMount = new GameObject(this.gameObject.name + "->>DeactiveObjectMount");
                    deactiveObjectMount.transform.SetParent(Facade.Instance.GetModule(CFModules.OBJECTPOOL).ModuleMountObject.transform);
                    deactiveObjectMount.transform.ResetLocalTransform();
                }
                return deactiveObjectMount.transform;
            }
        }
        public Transform ActiveObjectMount
        {
            get
            {
                return Facade.Instance.GetObjectSpawnPoolActiveMount().transform;
            }
        }
        protected virtual void Awake()
        {
            RegisterSpawner();
        }
        private void OnDestroy()
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
        public virtual void SpawnUncollectable() { }
        /// <summary>
        /// 回收Uncollectable的对象，直接被销毁
        /// </summary>
        public virtual void DespawnUncollectable()
        {
            Cosmos.GameManager.KillObjects(UncollectableHashSet);
        }
        protected abstract void RegisterSpawner();
        /// <summary>
        /// 注销并销毁
        /// </summary>
        protected virtual void DeregisterSpawner()
        {
            Facade.Instance.DeregisterObjectSapwnPool(this);
            GameManager.KillObject(deactiveObjectMount);
        }
        protected virtual void SpawnHandler(GameObject go)
        {
            if (go == null)
                return;
            Facade.Instance.StartCoroutine(EnumCollect(CollectDelay,()=> Facade.Instance.DespawnObject(this, go)));
        }
        protected virtual  void DespawnHandler(GameObject go)
        {
            if (go == null)
                return;
            go.transform.SetParent(DeactiveObjectMount);
            go.transform.ResetLocalTransform();
        }
        public virtual void ClearAll()
        {
            Facade.Instance.ClearObjectSpawnPool(this);
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
        /// 执行结束自动作为ActiveObjectMount的子对象
        /// </summary>
        /// <param name="go">生成的对象</param>
        /// <param name="trans">对齐对象</param>
        protected void AlignObject(ObjectSpawnAlignType alignType,GameObject go,Transform trans)
        {
            if (trans == null)
                return;
            go.transform.SetParent(trans);
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