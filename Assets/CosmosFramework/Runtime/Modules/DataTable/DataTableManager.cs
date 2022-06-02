using System;
using System.Collections.Generic;
namespace Cosmos.DataTable
{
    [Module]
    internal sealed partial class DataTableManager : Module, IDataTableManager
    {
        Dictionary<string, DataTableBase> dataTableDict;
        IDataTableHelper dataTableHelper;
        /// <summary>
        /// 数据表数量；
        /// </summary>
        public int DataTableCount { get { return dataTableDict.Count; } }
        /// <summary>
        /// 设置数据表数据表帮助体；
        /// </summary>
        /// <param name="provider">帮助体</param>
        public void SetDataTableProvider(IDataTableHelper provider)
        {
            if (dataTableHelper != null)
            {
                dataTableHelper.OnReadDataTableFailure -= OnReadFailure;
                dataTableHelper.OnReadDataTableSuccess -= OnReadSuccess;
            }
            this.dataTableHelper = provider;
            this.dataTableHelper.OnReadDataTableFailure += OnReadFailure;
            this.dataTableHelper.OnReadDataTableSuccess += OnReadSuccess;
        }
        /// <summary>
        /// 异步读取数据表资源
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        /// <param name="dataTable">数据表</param>
        public void ReadDataTableAssetAsync(DataTableAssetInfo assetInfo, DataTableBase dataTable)
        {
            if (dataTableHelper == null)
                throw new ArgumentNullException("dataTableHelper is invalid ");
            if (dataTable == null)
                throw new ArgumentNullException("dataTable is invalid ");
            dataTableHelper.LoadDataTableAsync(assetInfo, dataTable);
            dataTable.DataTableAssetInfo = assetInfo;
        }
        /// <summary>
        /// 异步读取数据表资源
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        /// <param name="name">数据表名</param>
        public void ReadDataTableAssetAsync(DataTableAssetInfo assetInfo, string name)
        {
            if (dataTableHelper == null)
                throw new ArgumentNullException("dataTableHelper is invalid ");
            if (dataTableDict.TryGetValue(name, out var dataTable))
            {
                dataTableHelper.LoadDataTableAsync(assetInfo, dataTable);
                dataTable.DataTableAssetInfo = assetInfo;
            }
        }
        /// <summary>
        /// 获取数据表；
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="name">数据表名</param>
        /// <returns>数据表</returns>
        public IDataTable<T> GetDataTable<T>(string name) where T : class, IDataRow, new()
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException($"{name} is invalid ");
            dataTableDict.TryGetValue(name, out var dataTableBase);
            return dataTableBase as IDataTable<T>;
        }
        /// <summary>
        /// 获取数据表；
        /// </summary>
        /// <param name="name">数据表名</param>
        /// <returns>数据表</returns>
        public DataTableBase GetDataTable(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException($"{name} is invalid ");
            dataTableDict.TryGetValue(name, out var dataTableBase);
            return dataTableBase;
        }
        /// <summary>
        /// 是否存在数据表；
        /// </summary>
        /// <param name="name">数据表名</param>
        /// <returns>是否存在</returns>
        public bool HasDataTable(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException($"{name} is invalid ");
            return dataTableDict.ContainsKey(name);
        }
        /// <summary>
        /// 释放数据表；
        /// </summary>
        /// <param name="name">数据表名</param>
        public void ReleaseDataTable(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException($"{name} is invalid ");
            if (dataTableDict.TryRemove(name, out var dataTable))
            {
                dataTableHelper.UnLoadDataTable(dataTable.DataTableAssetInfo);
                dataTable.OnRelease();
            }
        }
        /// <summary>
        /// 释放数据表；
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="dataTable">数据表</param>
        public void ReleaseDataTable<T>(IDataTable<T> dataTable) where T : class, IDataRow, new()
        {
            if (dataTable == null)
                throw new ArgumentNullException("dataTable is invalid ");
            if (dataTableDict.Remove(dataTable.Name))
            {
                var dataTableBase = dataTable as DataTableBase;
                dataTableHelper.UnLoadDataTable(((DataTable<T>)dataTable).DataTableAssetInfo);
                dataTableBase.OnRelease();
            }
        }
        /// <summary>
        /// 释放数据表；
        /// </summary>
        /// <param name="dataTable">数据表</param>
        public void ReleaseDataTable(DataTableBase dataTable)
        {
            if (dataTable == null)
                throw new ArgumentNullException("dataTable is invalid ");
            if (dataTableDict.Remove(dataTable.Name))
            {
                dataTableHelper.UnLoadDataTable(dataTable.DataTableAssetInfo);
                dataTable.OnRelease();
            }
        }
        /// <summary>
        /// 创建一个数据表，dataType指需要传入IDataRow的派生类型；
        /// </summary>
        /// <param name="name">数据表名</param>
        /// <param name="dataType">数据类型,IDataRow的派生类</param>
        /// <returns>数据表</returns>
        public DataTableBase CreateDataTable(string name, Type dataType)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException($"{name} is invalid ");
            if (dataTableDict.TryGetValue(name, out var dataTableBase))
                return dataTableBase;
            if (dataType == null)
                throw new ArgumentNullException($"dataType is invalid");
            if (typeof(IDataRow).IsAssignableFrom(dataType))
                throw new Exception($"{dataType} is not inherit from IDataRow");
            Type dataTableType = typeof(DataTable<>).MakeGenericType(dataType);
            var dataTable = (DataTableBase)Activator.CreateInstance(dataTableType, name);
            dataTableDict.Add(name, dataTable);
            return dataTable;
        }
        /// <summary>
        /// 创建一个数据表；
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="name">数据表名</param>
        /// <returns>数据表</returns>
        public IDataTable<T> CreateDataTable<T>(string name) where T : class, IDataRow, new()
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException($"{name} is invalid ");
            if (dataTableDict.TryGetValue(name, out var dataTableBase))
                return (IDataTable<T>)dataTableBase;
            var dataTable = new DataTable<T>(name);
            dataTableDict.Add(name, dataTable);
            return dataTable;
        }
        protected override void OnInitialization()
        {
            dataTableDict = new Dictionary<string, DataTableBase>();
            SetDataTableProvider(new DefaultDataTableHelper());
        }
        void OnReadSuccess(DataTableBase dataTable, byte[] dataTableBytes)
        {
            dataTable.ReadDataTable(dataTableBytes);
            dataTable.OnReadDataSuccess();
        }
        void OnReadFailure(DataTableBase dataTable, string errorMessage)
        {
            dataTable.OnReadDataFailure();
        }
    }
}