using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos{
    /// <summary>
    /// 游戏框架中包含事件数据的类的基类。
    /// </summary>
    public abstract class EventArgsBase :EventArgs,IEventArgs
    {
        /// <summary>
        /// 清空引用
        /// </summary>
        public abstract void Clear();
    }
}
