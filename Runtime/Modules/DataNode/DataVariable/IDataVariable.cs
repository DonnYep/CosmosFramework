using System;

namespace Cosmos.DataNode
{
    public interface IDataVariable : IReference
    {
        /// <summary>
        /// 变量类型
        /// </summary>
        Type Type { get; }
        /// <summary>
        /// 获取变量值
        /// </summary>
        /// <returns></returns>
        object GetValue();
        /// <summary>
        /// 设置变量值
        /// </summary>
        /// <param name="value">变量值</param>
        void SetValue(object value);
    }
}
