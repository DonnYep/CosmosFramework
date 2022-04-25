using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Cosmos.DataTable
{
    internal sealed partial class DataTableManager
    {
        sealed class DataTable<T> : DataTableBase, IDataTable<T>
             where T : class, IDataRow, new()
        {
            Dictionary<int, T> rowDataDict;
            Pool<T> rowPool;
            /// <inheritdoc/>
            public int RowCount { get { return rowDataDict.Count; } }
            /// <inheritdoc/>
            public Type Type { get { return typeof(T); } }
            /// <inheritdoc/>
            public T this[int id]
            {
                get
                {
                    if (rowDataDict.TryGetValue(id, out var data))
                        return data;
                    else
                        return null;
                }
            }

            public DataTable(string name) : base(name)
            {
                rowDataDict = new Dictionary<int, T>();
                rowPool = new Pool<T>(() => { return new T(); }, r => { r.Dispose(); });
            }
            /// <inheritdoc/>
            public bool AddRow(string rowString)
            {
                var data = rowPool.Spawn();
                if (data.ParseData(rowString))
                {
                    var rst = rowDataDict.TryAdd(data.Id, data);
                    if (!rst)
                        rowPool.Despawn(data);
                    return rst;
                }
                else
                {
                    rowPool.Despawn(data);
                    throw new Exception($"DataTable: {name} ParseData Failure");
                }
            }
            /// <inheritdoc/>
            public bool AddRow(byte[] rowBytes)
            {
                var data = rowPool.Spawn();
                if (data.ParseData(rowBytes))
                {
                    var rst = rowDataDict.TryAdd(data.Id, data);
                    if (!rst)
                        rowPool.Despawn(data);
                    return rst;
                }
                else
                {
                    rowPool.Despawn(data);
                    throw new Exception($"DataTable: {name} ParseData Failure");
                }
            }
            /// <inheritdoc/>
            public T[] GetAllRowDatas()
            {
                return rowDataDict.Values.ToArray();
            }
            /// <inheritdoc/>
            public T GetRowData(int id)
            {
                rowDataDict.TryGetValue(id, out var data);
                return data;
            }
            /// <inheritdoc/>
            public T[] GetRowDatas(Predicate<T> predicate)
            {
                return rowDataDict.Values.Where(t => predicate(t)).ToArray();
            }
            /// <inheritdoc/>
            public bool HasRow(int id)
            {
                return rowDataDict.ContainsKey(id);
            }
            /// <inheritdoc/>
            public bool HasRow(Predicate<T> predicate)
            {
                foreach (var data in rowDataDict.Values)
                {
                    if (predicate(data))
                        return true;
                }
                return false;
            }
            /// <inheritdoc/>
            public void RemoveAllRows()
            {
                rowDataDict.Clear();
            }
            /// <inheritdoc/>
            public bool RemoveRow(int id)
            {
                var rst = rowDataDict.TryRemove(id, out var data);
                if (rst)
                    rowPool.Despawn(data);
                return rst;
            }
            public IEnumerator<T> GetEnumerator()
            {
                return rowDataDict.Values.GetEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return rowDataDict.Values.GetEnumerator();
            }
            /// <inheritdoc/>
            internal override void ReadDataTable(byte[] dataTableBytes)
            {
                using (MemoryStream stream = new MemoryStream(dataTableBytes))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line = string.Empty;
                        while ((line = reader.ReadLine()) != null)
                        {
                            AddRow(line);
                        }
                    }
                }
            }
            /// <inheritdoc/>
            internal override void OnRelease()
            {
                rowDataDict.Clear();
                rowPool.Clear();
                DataTableAssetInfo = null;
            }
        }
    }
}
