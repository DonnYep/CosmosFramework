using System;

namespace Cosmos.DataTable
{
    public interface IDataTableManager : IModuleManager, IModuleInstance
    {
        /// <summary>
        /// 数据表数量；
        /// </summary>
        int DataTableCount { get; }
        /// <summary>
        /// 设置数据表数据表帮助体；
        /// </summary>
        /// <param name="provider">帮助体</param>
        void SetDataTableProvider(IDataTableHelper provider);
        /// <summary>
        /// 异步读取数据表资源
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        /// <param name="dataTable">数据表</param>
        void ReadDataTableAssetAsync(DataTableAssetInfo assetInfo, DataTableBase dataTable);
        /// <summary>
        /// 异步读取数据表资源
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        /// <param name="name">数据表名</param>
        void ReadDataTableAssetAsync(DataTableAssetInfo assetInfo, string name);
        /// <summary>
        /// 获取数据表；
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="name">数据表名</param>
        /// <returns>数据表</returns>
        IDataTable<T> GetDataTable<T>(string name) where T : class, IDataRow, new();
        /// <summary>
        /// 获取数据表；
        /// </summary>
        /// <param name="name">数据表名</param>
        /// <returns>数据表</returns>
        DataTableBase GetDataTable(string name);
        /// <summary>
        /// 是否存在数据表；
        /// </summary>
        /// <param name="name">数据表名</param>
        /// <returns>是否存在</returns>
        bool HasDataTable(string name);
        /// <summary>
        /// 释放数据表；
        /// </summary>
        /// <param name="name">数据表名</param>
        void ReleaseDataTable(string name);
        /// <summary>
        /// 释放数据表；
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="dataTable">数据表</param>
        void ReleaseDataTable<T>(IDataTable<T> dataTable) where T : class, IDataRow, new();
        /// <summary>
        /// 释放数据表；
        /// </summary>
        /// <param name="dataTable">数据表</param>
        void ReleaseDataTable(DataTableBase dataTable);
        /// <summary>
        /// 创建一个数据表；
        /// </summary>
        /// <param name="name">数据表名</param>
        /// <param name="rowType">数据类型</param>
        /// <returns>数据表</returns>
        DataTableBase CreateDataTable(string name, Type rowType);
        /// <summary>
        /// 创建一个数据表；
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="name">数据表名</param>
        /// <returns>数据表</returns>
        IDataTable<T> CreateDataTable<T>(string name) where T : class, IDataRow, new();
    }
}
