using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos{
    /// <summary>
    /// 游戏框架中包含事件数据的类的基类。
    /// </summary>
    public abstract class CFEventArgs :EventArgs,IReference
    {
        /// <summary>
        /// 清空引用
        /// </summary>
        public abstract void Reset();
    }
}
