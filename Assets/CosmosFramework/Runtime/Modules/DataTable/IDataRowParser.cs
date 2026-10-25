using System;

namespace Cosmos.DataTable
{
    /// <summary>
    /// 数据表解析器：把表资源（bytes）解析成行数组。
    /// <para>解析失败请抛 <see cref="DataTableParseException"/>（带行号/列名），
    /// 模块会把它转成表的 Failed 状态与 Error 文案，而不是让异常穿透到资源回调。</para>
    /// </summary>
    /// <typeparam name="T">行类型</typeparam>
    public interface IDataRowParser<T> where T : class, IDataRow, new()
    {
        /// <summary>
        /// 解析整张表
        /// </summary>
        /// <param name="assetInfo">表资源信息</param>
        /// <param name="bytes">表资源字节</param>
        /// <returns>行数组（可为空数组，不可为 null）</returns>
        T[] Parse(DataTableAssetInfo assetInfo, byte[] bytes);
    }
    /// <summary>
    /// 数据表解析异常：带表名、行号与列名，便于定位坏数据。
    /// </summary>
    public class DataTableParseException : Exception
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string DataTableName { get; private set; }
        /// <summary>
        /// 行号（从 1 开始，含表头行）
        /// </summary>
        public int Line { get; private set; }
        /// <summary>
        /// 列名（可为空）
        /// </summary>
        public string Column { get; private set; }
        public DataTableParseException(string dataTableName, int line, string column, string message)
            : base(FormatMessage(dataTableName, line, column, message))
        {
            DataTableName = dataTableName;
            Line = line;
            Column = column;
        }
        public DataTableParseException(string dataTableName, int line, string column, string message, Exception innerException)
            : base(FormatMessage(dataTableName, line, column, message), innerException)
        {
            DataTableName = dataTableName;
            Line = line;
            Column = column;
        }
        static string FormatMessage(string dataTableName, int line, string column, string message)
        {
            var location = string.IsNullOrEmpty(column) ? $"第 {line} 行" : $"第 {line} 行第 {column} 列";
            return $"DataTable [{dataTableName}] {location} : {message}";
        }
    }
}
