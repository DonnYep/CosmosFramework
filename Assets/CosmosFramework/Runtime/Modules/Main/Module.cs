using UnityEngine;
using System.Collections;
using System;
namespace Cosmos
{

    /// <summary>
    /// 模块的抽象基类；
    /// 外部可扩展；
    /// </summary>
    public abstract class Module
    {
        public bool IsPause { get; protected set; }
        #region Methods
        /// <summary>
        /// 空虚函数;
        /// </summary>
        public virtual void OnInitialization(){}
        /// <summary>
        /// 空虚函数，停止模块
        /// </summary>
        public virtual void OnTermination(){}
        public virtual void OnActive() { }
        public virtual void OnPreparatory() { }
        public virtual void OnPause() { IsPause = true; }
        public virtual void OnUnPause() { IsPause = false; }
        public virtual void OnDeactive(){}
        #endregion
    }
}