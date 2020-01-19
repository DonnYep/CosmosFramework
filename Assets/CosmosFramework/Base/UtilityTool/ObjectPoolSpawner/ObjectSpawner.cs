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
        protected GameObject deactiveObjectMount;
        public Transform DeactiveObjectMount
        {
            get
            {
                if (deactiveObjectMount == null)
                {
                    deactiveObjectMount = new GameObject(this.gameObject.name + "->>DeactiveObjectMount");
                    deactiveObjectMount.transform.SetParent(Facade.Instance.GetModule(CFModule.ObjectPool).ModuleMountObject.transform);
                    deactiveObjectMount.transform.RestLocalTransform();
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
        public virtual void SpawnWithoutPool() { }
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
            go.transform.RestLocalTransform();
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
    }
}