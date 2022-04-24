using System;
using System.Collections.Generic;

namespace Cosmos.DataTable
{
    public interface IDataTable<T>:IEnumerable<T>where T:IDataRow
    {
        /// <summary>
        /// 数据表名
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 数据的行数
        /// </summary>
        int RowCount { get; }
        /// <summary>
        /// 数据的类型
        /// </summary>
        Type Type { get; }
        /// <summary>
        /// 添加一行数据；
        /// </summary>
        /// <param name="rowString">行数据</param>
        /// <returns>是否添加成功</returns>
        bool AddRow(string rowString);
        /// <summary>
        /// 添加一行数据；
        /// </summary>
        /// <param name="rowBytes">行数据</param>
        /// <returns>是否添加成功</returns>
        bool AddRow(byte[] rowBytes);
        /// <summary>
        /// 获取指定行的数据对象；
        /// </summary>
        /// <param name="id">索引</param>
        /// <returns>获取到的数据对象</returns>
        T GetRowData(int id);
        /// <summary>
        /// 条件查找数据对象集合
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns>满足条件的集合</returns>
        T[] GetRowDatas(Predicate<T> predicate);
        /// <summary>
        /// 获取所有行数据；
        /// </summary>
        /// <returns>行数据集合</returns>
        T[] GetAllRowDatas();
        /// <summary>
        /// 是否存在行数据；
        /// </summary>
        /// <param name="id">索引</param>
        /// <returns>是否存在</returns>
        bool HasRow(int id);
        /// <summary>
        /// 条件判断是否存在行数据；
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns>是否存在</returns>
        bool HasRow(Predicate<T> predicate);
        /// <summary>
        /// 移除一个行数据；
        /// </summary>
        /// <param name="id">索引</param>
        /// <returns>执行结果</returns>
        bool RemoveRow(int id);
        /// <summary>
        /// 移除所有行数据；
        /// </summary>
        void RemoveAllRows();
    }
}
