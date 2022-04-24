using System;
using System.Collections.Generic;
namespace Cosmos.DataTable
{
    [Module]
    internal sealed partial class DataTableManager : Module, IDataTableManager
    {
        Dictionary<string, DataTableBase> dataTableDict;
        IDataTableHelper dataTableHelper;
        public int DataTableCount { get { return dataTableDict.Count; } }
        public void SetDataTableProvider(IDataTableHelper provider)
        {
            if (dataTableHelper != null)
            {
                dataTableHelper.OnReadDataTableFailure -= OnReadFailure;
                dataTableHelper.OnReadDataTableSuccess -= OnReadSuccess;
            }
            this.dataTableHelper = provider;
            dataTableHelper.OnReadDataTableFailure += OnReadFailure;
            dataTableHelper.OnReadDataTableSuccess += OnReadSuccess;
        }
        public void ReadDataTableAssetAsync(DataTableAssetInfo assetInfo, DataTableBase dataTable)
        {
            if (dataTableHelper == null)
                throw new ArgumentNullException("dataTableHelper is invalid ");
            if (assetInfo == null)
                throw new ArgumentNullException("assetInfo is invalid ");
            if (dataTable == null)
                throw new ArgumentNullException("dataTable is invalid ");
            dataTableHelper.LoadDataTableAsync(assetInfo, dataTable);
            dataTable.DataTableAssetInfo = assetInfo;
        }
        public void ReadDataTableAssetAsync(DataTableAssetInfo assetInfo, string name)
        {
            if (dataTableHelper == null)
                throw new ArgumentNullException("dataTableHelper is invalid ");
            if (assetInfo == null)
                throw new ArgumentNullException("assetInfo is invalid ");
            if (dataTableDict.TryGetValue(name, out var dataTable))
            {
                dataTableHelper.LoadDataTableAsync(assetInfo, dataTable);
                dataTable.DataTableAssetInfo = assetInfo;
            }
        }
        public IDataTable<T> GetDataTable<T>(string name) where T : class, IDataRow, new()
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException($"{name} is invalid ");
            dataTableDict.TryGetValue(name, out var dataTableBase);
            return dataTableBase as IDataTable<T>;
        }
        public DataTableBase GetDataTable(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException($"{name} is invalid ");
            dataTableDict.TryGetValue(name, out var dataTableBase);
            return dataTableBase;
        }
        public bool HasDataTable(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException($"{name} is invalid ");
            return dataTableDict.ContainsKey(name);
        }
        public void ReleaseDataTable(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException($"{name} is invalid ");
            if (dataTableDict.TryRemove(name, out var dataTable))
            {
                if (dataTable.DataTableAssetInfo != null)
                    dataTableHelper.UnLoadDataTable(dataTable.DataTableAssetInfo);
            }
        }
        public void ReleaseDataTable<T>(IDataTable<T> dataTable) where T : class, IDataRow, new()
        {
            if (dataTable == null)
                throw new ArgumentNullException("dataTable is invalid ");
            if (dataTableDict.Remove(dataTable.Name))
            {
                var dataTableBase = dataTable as DataTableBase;
                if (dataTableBase.DataTableAssetInfo != null)
                    dataTableHelper.UnLoadDataTable(((DataTable<T>)dataTable).DataTableAssetInfo);
            }
        }
        public void ReleaseDataTable(DataTableBase dataTable)
        {
            if (dataTable == null)
                throw new ArgumentNullException("dataTable is invalid ");
            if (dataTableDict.Remove(dataTable.Name))
            {
                if (dataTable.DataTableAssetInfo != null)
                    dataTableHelper.UnLoadDataTable(dataTable.DataTableAssetInfo);
            }
        }
        public DataTableBase CreateDataTable(string name, Type dataType)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException($"{name} is invalid ");
            if (dataTableDict.TryGetValue(name, out var dataTableBase))
                return dataTableBase;
            if (typeof(IDataRow).IsAssignableFrom(dataType))
                throw new Exception($"{dataType} is not inherit from IDataRow");
            Type dataTableType = typeof(DataTable<>).MakeGenericType(dataType);
            var dataTable = (DataTableBase)Activator.CreateInstance(dataTableType, name);
            dataTableDict.Add(name, dataTable);
            return dataTable;
        }
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
            this.dataTableHelper = new DefaultDataTableHelper();
        }
        void OnReadSuccess(DataTableBase dataTable)
        {
            dataTable.OnReadDataSuccess();
        }
        void OnReadFailure(DataTableBase dataTable)
        {
            dataTable.OnReadDataFailure();
        }
    }
}