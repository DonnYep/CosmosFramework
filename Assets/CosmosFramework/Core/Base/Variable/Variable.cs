using System;
namespace Cosmos
{
    /// <summary>
    /// Variable类型为模块之间的数据节点数据；
    /// </summary>
    public abstract class Variable : IReference
    {
        protected Variable() { }
        /// <summary>
        /// 变量类型
        /// </summary>
        public abstract Type Type { get; }
        /// <summary>
        /// 获取变量值
        /// </summary>
        /// <returns></returns>
        public abstract object GetValue();
        /// <summary>
        /// 设置变量值
        /// </summary>
        /// <param name="value">变量值</param>
        public abstract void SetValue(object value);
        /// <summary>
        /// 重置变量;
        /// 空虚函数；
        /// </summary>
        public abstract void Clear();
    }
}