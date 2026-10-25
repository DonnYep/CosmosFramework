using System;
using System.Collections.Generic;

namespace Cosmos.DataTable
{
    /// <summary>
    /// 数据表实现：行数组 + 主键索引。
    /// <para>为什么不是 Dictionary&lt;int,T&gt;：</para>
    /// <para>1、数组连续存储，遍历/筛选没有哈希开销，`Rows` 可直接以 IReadOnlyList 暴露（零分配）；</para>
    /// <para>2、主键查询用 Dictionary&lt;int,int&gt;（id → 下标），仍是 O(1) 且比存放引用更省内存；</para>
    /// <para>3、行对象只在加载时创建一次，之后**不再回收/复用**，因此不存在引用失效问题。</para>
    /// </summary>
    /// <typeparam name="T">行类型</typeparam>
    internal sealed class DataTable<T> : DataTableBase, IDataTable<T> where T : class, IDataRow, new()
    {
        static readonly T[] emptyRows = new T[0];
        readonly IDataRowParser<T> parser;
        T[] rows = emptyRows;
        readonly Dictionary<int, int> idIndex = new Dictionary<int, int>();
        public DataTable(string name, IDataRowParser<T> parser)
            : base(name)
        {
            this.parser = parser ?? new CsvDataRowParser<T>();
        }
        public override int RowCount { get { return rows.Length; } }
        public override Type RowType { get { return typeof(T); } }
        public Type Type { get { return typeof(T); } }
        public IReadOnlyList<T> Rows { get { return rows; } }
        public T this[int id]
        {
            get
            {
                T row;
                return TryGetRow(id, out row) ? row : null;
            }
        }
        public bool TryGetRow(int id, out T row)
        {
            int index;
            if (idIndex.TryGetValue(id, out index))
            {
                row = rows[index];
                return true;
            }
            row = null;
            return false;
        }
        public T GetRowData(int id)
        {
            T row;
            return TryGetRow(id, out row) ? row : null;
        }
        public T[] GetAllRowDatas()
        {
            return rows;
        }
        public T[] GetRowDatas(Predicate<T> condition)
        {
            if (condition == null)
                return emptyRows;
            var result = new List<T>();
            for (int i = 0; i < rows.Length; i++)
            {
                var row = rows[i];
                if (row != null && condition(row))
                    result.Add(row);
            }
            return result.Count == 0 ? emptyRows : result.ToArray();
        }
        public bool HasRow(int id)
        {
            return idIndex.ContainsKey(id);
        }
        public bool HasRow(Predicate<T> condition)
        {
            if (condition == null)
                return false;
            for (int i = 0; i < rows.Length; i++)
            {
                var row = rows[i];
                if (row != null && condition(row))
                    return true;
            }
            return false;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)rows).GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)rows).GetEnumerator();
        }
        internal override void ParseAndSet(byte[] dataTableBytes)
        {
            var parsedRows = parser.Parse(AssetInfo, dataTableBytes) ?? emptyRows;
            ClearRows();
            if (parsedRows.Length == 0)
            {
                rows = emptyRows;
                return;
            }
            var index = new Dictionary<int, int>(parsedRows.Length);
            for (int i = 0; i < parsedRows.Length; i++)
            {
                var row = parsedRows[i];
                if (row == null)
                    continue;
                //主键重复由解析器负责报错；这里做最后兜底（保留第一条）
                if (!index.ContainsKey(row.Id))
                    index.Add(row.Id, i);
            }
            rows = parsedRows;
            foreach (var pair in index)
                idIndex.Add(pair.Key, pair.Value);
        }
        internal override void ClearRows()
        {
            rows = emptyRows;
            idIndex.Clear();
        }
    }
}
