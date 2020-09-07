using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public interface IActor<T>
        where T:class
    {
        /// <summary>
        /// Actor的拥有者
        /// </summary>
        T Owner { get; }
        /// <summary>
        /// Actor枚举约束的类型;
        /// </summary>
        byte ActorType { get; }
        /// <summary>
        ///系统分配的ID
        /// </summary>
        int ActorID { get; }
        /// <summary>
        /// 设置Acotor数据
        /// </summary>
        /// <typeparam name="TData">数据类型，无约束</typeparam>
        /// <param name="dataKey">数据类型</param>
        /// <param name="data">具体数据</param>
        void SetData<TData>(ushort dataKey, TData data)where TData: Variable;
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="TData">数据类型，无约束</typeparam>
        /// <param name="dataKey">数据类型</param>
        /// <returns>具体数据</returns>
        TData GetData<TData>(ushort dataKey) where TData : Variable;
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="dataKey">数据类型</param>
        /// <returns>是否移除成功</returns>
        bool RemoveData(ushort dataKey);
        /// <summary>
        /// 是否存在数据
        /// </summary>
        /// <param name="dataKey">数据类型</param>
        /// <returns>是否存在</returns>
        bool HasData(ushort dataKey);
    }
}
