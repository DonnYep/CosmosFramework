using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos {
    /// <summary>
    /// 框架中整体业务逻辑的事件参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LogicEventArgs<T> : GameEventArgs
    {
        /// <summary>
        /// 泛型构造
        /// </summary>
        /// <param name="data"></param>
        public LogicEventArgs(T data)
        {
            SetData(data);
        }
        public LogicEventArgs() { }
        /// <summary>
        /// 泛型数据类型
        /// </summary>
        public T Data { get; private set; }
        public void SetData(T data)
        {
            Data = data;
        }
        public override void Clear()
        {
            Data = default(T);
        }
    }
}