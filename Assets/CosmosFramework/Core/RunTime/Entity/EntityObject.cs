using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cosmos;
using System;

namespace Cosmos.Entity
{
    /// <summary>
    /// 实体对象
    /// </summary>
    public abstract class EntityObject : IEntityObject
    {
        public GameObject Entity { get; protected set; }
        public bool IsPause { get; protected set; }
        /// <summary>
        /// 空虚函数
        /// </summary>
        public virtual void Clear() { }
        public abstract void OnActive();
        public abstract void OnDeactive();
        /// <summary>
        /// 非空虚函数
        /// 覆写时候建议保留父类方法
        /// </summary>
        public virtual void OnRefresh() { if (IsPause) return; }
        /// <summary>
        /// 挂载到一个父类对象
        /// </summary>
        /// <param name="parent">父类对象</param>
        public abstract void OnAttach(Transform parent);
        /// <summary>
        /// 空虚函数
        /// </summary>
        public virtual void OnAttachChild(Transform child) { }
        /// <summary>
        /// 从父类对象移除
        /// </summary>
        public abstract void OnDetach();
        /// <summary>
        /// 空虚函数
        /// 移除子对象
        /// </summary>
        public virtual void OnDetachChild(Transform child) { }
        public void OnPause(){IsPause = true;}
        public void OnUnPause(){IsPause = false;}
        public void SetEntity(GameObject entity){Entity = entity;}
        public GameObject GetEntity(){return Entity;}
    }
}