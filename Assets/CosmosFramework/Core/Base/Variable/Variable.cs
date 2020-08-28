using System;
namespace Cosmos
{
    public abstract class Variable : IBehaviour, IRenewable
    {
        protected Variable() { }
        /// <summary>
        /// 变量类型
        /// </summary>
        public abstract Type Type { get; }
        /// <summary>
        /// 空虚函数；
        /// 获取变量值
        /// </summary>
        /// <returns></returns>
        public virtual object GetValue() { return null; }
        /// <summary>
        /// 空虚函数；
        /// 设置变量值
        /// </summary>
        /// <param name="value">变量值</param>
        public virtual void SetValue(object value) { }
        /// <summary>
        /// 空虚函数；
        /// 重置变量;
        /// </summary>
        public virtual void OnRenewal() { }
        /// <summary>
        /// 空虚函数;
        /// 初始化
        /// </summary>
        public virtual void OnInitialization() { }
        /// <summary>
        /// 空虚函数;
        /// 终结释放变量;
        /// </summary>
        public virtual void OnTermination() { }
    }
}