using System;
using System.Collections.Generic;

namespace Cosmos.DataTable
{
    /// <summary>
    /// 数据表（只读视图）。
    /// <para>没有 AddRow/RemoveRow：配置表在运行时**只读**。旧版允许逐行增删，
    /// 又用对象池复用行对象，等于把共享的享元对象回收后再给别人用——
    /// 这是“共享可变状态”，属于享元模式最危险的用法（业务侧持有的行引用会突然变成另一行）。
    /// 确需运行时数据请用普通集合，不要拿它当共享配置表。</para>
    /// </summary>
    /// <typeparam name="T">行类型</typeparam>
    public interface IDataTable<T> : IEnumerable<T> where T : IDataRow
    {
        /// <summary>
        /// 数据表名
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 数据行数
        /// </summary>
        int RowCount { get; }
        /// <summary>
        /// 加载状态
        /// </summary>
        DataTableState State { get; }
        /// <summary>
        /// 是否已加载
        /// </summary>
        bool IsLoaded { get; }
        /// <summary>
        /// 最近一次失败原因（State == Failed 时有效）
        /// </summary>
        string Error { get; }
        /// <summary>
        /// 加载成功事件（已加载时订阅会立即回调）
        /// </summary>
        event Action<DataTableBase> OnReadSuccess;
        /// <summary>
        /// 加载失败事件（已失败时订阅会立即回调）
        /// </summary>
        event Action<DataTableBase> OnReadFailure;
        /// <summary>
        /// 行类型
        /// </summary>
        Type Type { get; }
        /// <summary>
        /// 只读行视图（不分配；与 <see cref="GetAllRowDatas"/> 是同一份数组）
        /// </summary>
        IReadOnlyList<T> Rows { get; }
        /// <summary>
        /// 按主键取行；不存在返回 null
        /// </summary>
        /// <param name="id">行主键</param>
        /// <returns>行数据</returns>
        T this[int id] { get; }
        /// <summary>
        /// 按主键尝试取行（推荐：不产生 null 判断歧义）
        /// </summary>
        /// <param name="id">行主键</param>
        /// <param name="row">行数据</param>
        /// <returns>是否存在</returns>
        bool TryGetRow(int id, out T row);
        /// <summary>
        /// 按主键取行；不存在返回 null（等价索引器）
        /// </summary>
        /// <param name="id">行主键</param>
        /// <returns>行数据</returns>
        T GetRowData(int id);
        /// <summary>
        /// 全部行（返回内部数组，**不要修改其内容**；需要副本请自行 Clone）
        /// </summary>
        /// <returns>行数组</returns>
        T[] GetAllRowDatas();
        /// <summary>
        /// 条件筛选（结果为新数组；全表扫描，建议只用于低频查询）
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns>满足条件的行</returns>
        T[] GetRowDatas(Predicate<T> condition);
        /// <summary>
        /// 是否存在该主键的行
        /// </summary>
        /// <param name="id">行主键</param>
        /// <returns>是否存在</returns>
        bool HasRow(int id);
        /// <summary>
        /// 是否存在满足条件的行
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns>是否存在</returns>
        bool HasRow(Predicate<T> condition);
    }
}
