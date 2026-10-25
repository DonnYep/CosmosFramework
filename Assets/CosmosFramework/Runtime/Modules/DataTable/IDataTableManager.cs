using System;

namespace Cosmos.DataTable
{
    /// <summary>
    /// 数据表模块。
    /// <para>设计取舍（相对旧版“享元 + 行对象池”）：</para>
    /// <para>1、保留“一张表一个实例、多个消费者共享”的思想——这是配置表真正需要的东西；</para>
    /// <para>2、去掉行对象池与逐行增删：行加载后只读共享、不再回收复用
    /// （池化共享对象会让业务持有的行引用突然变成另一行，这是享元模式最常见的误用）；</para>
    /// <para>3、补齐生产需要的：加载状态/失败原因、幂等加载、重载、晚订阅回调、
    /// 批量释放、可插拔解析器（表头驱动 + 反射只做一次）、类型不匹配的明确报错。</para>
    /// </summary>
    public interface IDataTableManager : IModuleManager
    {
        /// <summary>
        /// 已创建的数据表数量
        /// </summary>
        int DataTableCount { get; }
        /// <summary>
        /// 设置数据表资源帮助体（默认使用 Resource 模块加载 TextAsset）
        /// </summary>
        /// <param name="provider">帮助体</param>
        void SetDataTableProvider(IDataTableHelper provider);
        /// <summary>
        /// 为某个行类型指定解析器（未指定时使用 <see cref="CsvDataRowParser{T}"/>）
        /// </summary>
        /// <typeparam name="T">行类型</typeparam>
        /// <param name="parser">解析器</param>
        void SetDataTableParser<T>(IDataRowParser<T> parser) where T : class, IDataRow, new();
        /// <summary>
        /// 获取某个行类型当前使用的解析器
        /// </summary>
        /// <typeparam name="T">行类型</typeparam>
        /// <returns>解析器</returns>
        IDataRowParser<T> GetDataTableParser<T>() where T : class, IDataRow, new();
        /// <summary>
        /// 创建（或取回）数据表；同名表已存在但行类型不同会报错并返回 null
        /// </summary>
        /// <typeparam name="T">行类型</typeparam>
        /// <param name="name">表名</param>
        /// <returns>数据表</returns>
        IDataTable<T> CreateDataTable<T>(string name) where T : class, IDataRow, new();
        /// <summary>
        /// 获取数据表（不存在或行类型不匹配返回 null）
        /// </summary>
        /// <typeparam name="T">行类型</typeparam>
        /// <param name="name">表名</param>
        /// <returns>数据表</returns>
        IDataTable<T> GetDataTable<T>(string name) where T : class, IDataRow, new();
        /// <summary>
        /// 获取数据表（非泛型视图）
        /// </summary>
        /// <param name="name">表名</param>
        /// <returns>数据表</returns>
        DataTableBase GetDataTable(string name);
        /// <summary>
        /// 是否存在数据表
        /// </summary>
        /// <param name="name">表名</param>
        /// <returns>是否存在</returns>
        bool HasDataTable(string name);
        /// <summary>
        /// 数据表是否已加载完成
        /// </summary>
        /// <param name="name">表名</param>
        /// <returns>是否已加载</returns>
        bool IsDataTableLoaded(string name);
        /// <summary>
        /// 异步加载表数据（幂等：加载中重复调用会被忽略，已加载会立即回调）
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        /// <param name="name">表名</param>
        void ReadDataTableAssetAsync(DataTableAssetInfo assetInfo, string name);
        /// <summary>
        /// 异步加载表数据（幂等）
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        /// <param name="dataTable">数据表</param>
        void ReadDataTableAssetAsync(DataTableAssetInfo assetInfo, DataTableBase dataTable);
        /// <summary>
        /// 异步加载表数据，并注册一次性完成回调（成功/失败都会回调；即便已加载也会立即回调）
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        /// <param name="dataTable">数据表</param>
        /// <param name="onCompleted">完成回调</param>
        void ReadDataTableAssetAsync(DataTableAssetInfo assetInfo, DataTableBase dataTable, Action<DataTableBase> onCompleted);
        /// <summary>
        /// 异步加载表数据（泛型重载，免去调用方强转 DataTableBase）
        /// </summary>
        /// <typeparam name="T">行类型</typeparam>
        /// <param name="assetInfo">资源信息</param>
        /// <param name="dataTable">数据表</param>
        /// <param name="onCompleted">完成回调</param>
        void ReadDataTableAssetAsync<T>(DataTableAssetInfo assetInfo, IDataTable<T> dataTable, Action<DataTableBase> onCompleted = null) where T : class, IDataRow, new();
        /// <summary>
        /// 重新加载表数据（用于热更/重进场景）
        /// </summary>
        /// <param name="name">表名</param>
        void ReloadDataTableAsync(string name);
        /// <summary>
        /// 释放数据表（卸载资源并清空数据）
        /// </summary>
        /// <param name="name">表名</param>
        /// <returns>是否释放成功</returns>
        bool ReleaseDataTable(string name);
        /// <summary>
        /// 释放全部数据表
        /// </summary>
        void ReleaseAllDataTables();
    }
}
