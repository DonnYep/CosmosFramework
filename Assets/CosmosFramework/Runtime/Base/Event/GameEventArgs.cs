using UnityEngine;
using System.Collections;
using System;
namespace Cosmos
{
    /// <summary>
    /// 标准事件的参数传值
    /// </summary>
    public abstract class GameEventArgs : EventArgs, IReference
    {
        /// <summary>
        /// 释放EventArgs；
        /// </summary>
        public abstract void Release();
    }
}