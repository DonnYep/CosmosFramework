using System;
using System.Collections.Generic;
namespace Cosmos.DataTable
{
    [Module]
    internal sealed partial class DataTableManager : Module, IDataTableManager
    {
        Dictionary<string, DataTableBase> dataTableDict;
        IDataTableHelper dataTableHelper;
        ///<inheritdoc/>
        public int DataTableCount { get { return dataTableDict.Count; } }
        ///<inheritdoc/>
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
        ///<inheritdoc/>
        public void ReadDataTableAssetAsync(DataTableAssetInfo assetInfo, DataTableBase dataTable)
        {
            if (dataTableHelper == null)
                throw new ArgumentNullException("dataTableHelper is invalid ");
            if (dataTable == null)
                throw new ArgumentNullException("dataTable is invalid ");
            dataTableHelper.LoadDataTableAsync(assetInfo, dataTable);
            dataTable.DataTableAssetInfo = assetInfo;
        }
        ///<inheritdoc/>
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
        ///<inheritdoc/>
        public IDataTable<T> GetDataTable<T>(string name) where T : class, IDataRow, new()
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException($"{name} is invalid ");
            dataTableDict.TryGetValue(name, out var dataTableBase);
            return dataTableBase as IDataTable<T>;
        }
        ///<inheritdoc/>
        public DataTableBase GetDataTable(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException($"{name} is invalid ");
            dataTableDict.TryGetValue(name, out var dataTableBase);
            return dataTableBase;
        }
        ///<inheritdoc/>
        public bool HasDataTable(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException($"{name} is invalid ");
            return dataTableDict.ContainsKey(name);
        }
        ///<inheritdoc/>
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
        ///<inheritdoc/>
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
        ///<inheritdoc/>
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
        ///<inheritdoc/>
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
        ///<inheritdoc/>
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