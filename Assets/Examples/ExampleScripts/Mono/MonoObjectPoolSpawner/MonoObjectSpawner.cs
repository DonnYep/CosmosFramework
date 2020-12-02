using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.ObjectPool;
using Cosmos.Mono;
namespace Cosmos
{
    /// <summary>
    /// 对象生成脚本，每个脚本自身就是一个Key
    /// </summary>
    public abstract class MonoObjectSpawner : MonoBehaviour
    {
        public  virtual float CollectDelay { get; }
        public virtual HashSet<GameObject> UncollectibleHashSet { get;protected set; }
        protected GameObject deactiveObjectMount;
        public Transform DeactiveObjectMount
        {
            get
            {
                if (deactiveObjectMount == null)
                {
                    deactiveObjectMount = new GameObject(this.gameObject.name + "->>DeactiveObjectMount");
                    deactiveObjectMount.transform.SetParent(GameManagerAgent.Instance.transform);
                    deactiveObjectMount.transform.ResetLocalTransform();
                }
                return deactiveObjectMount.transform;
            }
        }
        public Transform ActiveObjectMount
        {
            get
            {
                return GameManager.GetModuleMount<IObjectPoolManager>().transform;
            }
        }
        protected virtual void Start()
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
        public virtual void SpawnUncollectible() { }
        /// <summary>
        /// 回收Uncollectable的对象，直接被销毁
        /// </summary>
        public virtual void DespawnUncollectible()
        {
            GameManagerAgent.KillObjects(UncollectibleHashSet);
        }
        protected abstract void RegisterSpawner();
        /// <summary>
        /// 注销并销毁
        /// </summary>
        protected virtual void DeregisterSpawner()
        {
            GameManager.GetModule<IObjectPoolManager>().DeregisterSpawnPool(this);
            GameManagerAgent.KillObject(deactiveObjectMount);
        }
        protected virtual void SpawnHandler(GameObject go)
        {
            if (go == null)
                return;
            GameManager.GetModule<IMonoManager>().StartCoroutine(EnumCollect(CollectDelay,
                ()=> GameManager.GetModule<IObjectPoolManager>().Despawn(this, go)));
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
            GameManager.GetModule<IObjectPoolManager>().Clear(this);
        }
        protected  IEnumerator EnumCollect(float delay,Action action=null)
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
        protected void AlignObject(ObjectSpawnAlignType alignType,GameObject go,Transform trans)
        {
            if (trans == null||go==null)
                return;
            go.transform.SetParent(trans);
            go.SetActive(true);
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
        protected void AlignObject(ObjectPoolDataSet poolDataSet, GameObject go, Transform trans)
        {
            ObjectSpawnAlignType alignType = poolDataSet.AlignType;
            if (trans == null)
                return;
            go.transform.SetParent(trans);
            go.SetActive(true);
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

