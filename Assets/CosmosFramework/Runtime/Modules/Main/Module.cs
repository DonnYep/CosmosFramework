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
        protected virtual void OnInitialization(){}
        protected virtual void OnTermination(){}
        protected virtual void OnActive() { }
        protected virtual void OnPreparatory() { }
        protected virtual void OnPause() { IsPause = true; }
        protected virtual void OnUnPause() { IsPause = false; }
        protected virtual void OnDeactive(){}
        #endregion
    }
}