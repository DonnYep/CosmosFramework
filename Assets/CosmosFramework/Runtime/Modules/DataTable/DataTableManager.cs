using System;
using System.Collections.Generic;
namespace Cosmos.DataTable
{
    /// <summary>
    /// 数据表模块：表注册表 + 加载状态机 + 解析器注册表。
    /// <para>加载流程：ReadDataTableAssetAsync → 状态置 Loading → 帮助体加载资源
    /// → OnReadSuccess/OnReadFailure → 解析（失败原因写到表的 Error）→ 状态置 Loaded/Failed。</para>
    /// </summary>
    [Module(Priority = ModuleConstant.PRIORITY_DATA_TABLE)]
    internal sealed partial class DataTableManager : Module, IDataTableManager
    {
        Dictionary<string, DataTableBase> dataTableDict;
        IDataTableHelper dataTableHelper;
        /// <summary>
        /// 行类型 → 解析器（IDataRowParser&lt;T&gt;）
        /// </summary>
        readonly Dictionary<Type, object> parserDict = new Dictionary<Type, object>();
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
            dataTableHelper = provider;
            if (dataTableHelper != null)
            {
                dataTableHelper.OnReadDataTableFailure += OnReadFailure;
                dataTableHelper.OnReadDataTableSuccess += OnReadSuccess;
            }
        }
        ///<inheritdoc/>
        public void SetDataTableParser<T>(IDataRowParser<T> parser) where T : class, IDataRow, new()
        {
            parserDict[typeof(T)] = parser;
        }
        ///<inheritdoc/>
        public IDataRowParser<T> GetDataTableParser<T>() where T : class, IDataRow, new()
        {
            object parser;
            if (parserDict.TryGetValue(typeof(T), out parser))
            {
                var typed = parser as IDataRowParser<T>;
                if (typed != null)
                    return typed;
            }
            return new CsvDataRowParser<T>();
        }
        ///<inheritdoc/>
        public IDataTable<T> CreateDataTable<T>(string name) where T : class, IDataRow, new()
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name is invalid !");
            DataTableBase existingTable;
            if (dataTableDict.TryGetValue(name, out existingTable))
            {
                var typedTable = existingTable as IDataTable<T>;
                if (typedTable == null)
                {
                    Utility.Debug.LogError($"[DataTable] 表名 {name} 已被行类型 {existingTable.RowType} 占用，无法作为 {typeof(T)} 使用");
                    return null;
                }
                return typedTable;
            }
            var dataTable = new DataTable<T>(name, GetDataTableParser<T>());
            dataTableDict.Add(name, dataTable);
            return dataTable;
        }
        ///<inheritdoc/>
        public IDataTable<T> GetDataTable<T>(string name) where T : class, IDataRow, new()
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name is invalid !");
            DataTableBase dataTable;
            if (!dataTableDict.TryGetValue(name, out dataTable))
                return null;
            var typedTable = dataTable as IDataTable<T>;
            if (typedTable == null)
                Utility.Debug.LogWarning($"[DataTable] 表名 {name} 的行类型是 {dataTable.RowType}，不是 {typeof(T)}");
            return typedTable;
        }
        ///<inheritdoc/>
        public DataTableBase GetDataTable(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name is invalid !");
            DataTableBase dataTable;
            dataTableDict.TryGetValue(name, out dataTable);
            return dataTable;
        }
        ///<inheritdoc/>
        public bool HasDataTable(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name is invalid !");
            return dataTableDict.ContainsKey(name);
        }
        ///<inheritdoc/>
        public bool IsDataTableLoaded(string name)
        {
            DataTableBase dataTable;
            return !string.IsNullOrEmpty(name) && dataTableDict.TryGetValue(name, out dataTable) && dataTable.IsLoaded;
        }
        ///<inheritdoc/>
        public void ReadDataTableAssetAsync(DataTableAssetInfo assetInfo, string name)
        {
            ReadDataTableAssetAsync(assetInfo, GetDataTable(name), null);
        }
        ///<inheritdoc/>
        public void ReadDataTableAssetAsync(DataTableAssetInfo assetInfo, DataTableBase dataTable)
        {
            ReadDataTableAssetAsync(assetInfo, dataTable, null);
        }
        ///<inheritdoc/>
        public void ReadDataTableAssetAsync(DataTableAssetInfo assetInfo, DataTableBase dataTable, Action<DataTableBase> onCompleted)
        {
            if (dataTable == null)
                throw new ArgumentNullException("dataTable is invalid !");
            if (onCompleted != null)
                dataTable.AddCompletion(onCompleted);
            if (dataTable.State == DataTableState.Loading)
                return;     //幂等：正在加载，忽略重复请求（完成回调已挂在表上）
            if (dataTable.State == DataTableState.Loaded || dataTable.State == DataTableState.Failed)
            {
                //已处于终态：直接回调，不重复加载
                dataTable.FlushCompletionsIfSettled();
                return;
            }
            if (dataTableHelper == null)
                throw new InvalidOperationException("dataTableHelper is invalid !");
            dataTable.AssetInfo = assetInfo;
            dataTable.State = DataTableState.Loading;
            dataTable.Error = null;
            dataTableHelper.LoadDataTableAsync(assetInfo, dataTable);
        }
        ///<inheritdoc/>
        public void ReadDataTableAssetAsync<T>(DataTableAssetInfo assetInfo, IDataTable<T> dataTable, Action<DataTableBase> onCompleted = null) where T : class, IDataRow, new()
        {
            ReadDataTableAssetAsync(assetInfo, dataTable as DataTableBase, onCompleted);
        }
        ///<inheritdoc/>
        public void ReloadDataTableAsync(string name)
        {
            var dataTable = GetDataTable(name);
            if (dataTable == null)
            {
                Utility.Debug.LogError($"[DataTable] 表 {name} 不存在，无法重载");
                return;
            }
            dataTable.ClearRows();
            dataTable.State = DataTableState.None;
            dataTable.Error = null;
            ReadDataTableAssetAsync(dataTable.AssetInfo, dataTable, null);
        }
        ///<inheritdoc/>
        public bool ReleaseDataTable(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name is invalid !");
            DataTableBase dataTable;
            if (!dataTableDict.TryRemove(name, out dataTable))
                return false;
            ReleaseTableResource(dataTable);
            dataTable.Dispose();
            return true;
        }
        ///<inheritdoc/>
        public void ReleaseAllDataTables()
        {
            if (dataTableDict.Count == 0)
                return;
            var tables = new List<DataTableBase>(dataTableDict.Values);
            dataTableDict.Clear();
            for (int i = 0; i < tables.Count; i++)
            {
                ReleaseTableResource(tables[i]);
                tables[i].Dispose();
            }
        }
        protected override void OnInitialization()
        {
            dataTableDict = new Dictionary<string, DataTableBase>();
            SetDataTableProvider(new DefaultDataTableHelper());
        }
        protected override void OnTermination()
        {
            ReleaseAllDataTables();
            SetDataTableProvider(null);
        }
        void ReleaseTableResource(DataTableBase dataTable)
        {
            if (dataTable == null || dataTableHelper == null)
                return;
            if (dataTable.AssetInfo.IsValid)
                dataTableHelper.UnLoadDataTable(dataTable.AssetInfo);
        }
        void OnReadSuccess(DataTableBase dataTable, byte[] dataTableBytes)
        {
            if (dataTable == null)
                return;
            try
            {
                dataTable.ParseAndSet(dataTableBytes);
                dataTable.NotifyLoaded();
            }
            catch (Exception e)
            {
                //解析失败不再让异常穿透到资源回调：转成表的 Failed 状态并保留原因
                dataTable.NotifyFailed(e.Message);
                Utility.Debug.LogError($"[DataTable] 解析表 {dataTable.Name} 失败：{e.Message}");
            }
        }
        void OnReadFailure(DataTableBase dataTable, string errorMessage)
        {
            if (dataTable == null)
                return;
            dataTable.NotifyFailed(string.IsNullOrEmpty(errorMessage) ? "load data table failed" : errorMessage);
            Utility.Debug.LogError($"[DataTable] 加载表 {dataTable.Name} 失败：{errorMessage}");
        }
    }
}
