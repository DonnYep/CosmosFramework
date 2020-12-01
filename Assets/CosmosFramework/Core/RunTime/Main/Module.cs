using UnityEngine;
using System.Collections;
using System;
namespace Cosmos
{

    /// <summary>
    /// 模块的抽象基类；
    /// 外部可扩展；
    /// </summary>
    public abstract class Module: IControllableBehaviour, IMountPoint, IOperable
    {
        #region IMountPoint
        GameObject mountPoint;
        public GameObject MountPoint
        {
            get
            {
                if (mountPoint == null)
                {
                    mountPoint = new GameObject("Module-->>Container");
                    mountPoint.transform.SetParent(GameManager.InstanceObject.transform);
                }
                return mountPoint;
            }
        }
        #endregion
        public bool IsPause { get; protected set; }
        #region Methods
        #region interface IModule
        /// <summary>
        /// 空虚函数;
        /// </summary>
        public virtual void OnInitialization(){}
        /// <summary>
        /// 非空虚函数，停止模块
        /// 在子类调用时，建议保留执行父类函数
        /// </summary>
        public virtual void OnTermination()
        {
            //TODO 生命周期销毁问题 ，module
            mountPoint = null;
        }
        /// <summary>
        /// 非空虚函数
        /// 覆写时请尽量保留父类方法
        /// </summary>
        public virtual void OnRefresh() { if (IsPause) return; }
        public virtual void OnFixRefresh() { if (IsPause) return; }
        public virtual void OnLateRefresh() { if (IsPause) return; }
        public virtual void OnActive() { }
        public virtual void OnPreparatory() { }
        public virtual void OnPause() { IsPause = true; }
        public virtual void OnUnPause() { IsPause = false; }
        public virtual void OnDeactive(){}
        #endregion
        #endregion
    }
}