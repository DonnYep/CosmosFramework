using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cosmos.DataTable
{
    /// <summary>
    /// 默认 CSV 解析器（逗号分隔、首行表头）。
    /// <para>设计目标（生产可用）：</para>
    /// <para>1、**表头驱动**：按列名映射到行类型成员，列顺序变化不会串列（旧版按字段顺序逐列塞，改顺序即错位）；</para>
    /// <para>2、**解析只反射一次**：绑定计划在读到表头时构建（见 <see cref="DataRowBinder{T}"/>）；
    /// 行实现 <see cref="IRawDataRow"/> 时完全零反射；</para>
    /// <para>3、**容错可控**：空行与 # 注释行跳过；坏行可跳过（<see cref="SkipBadRows"/>）或整表失败；</para>
    /// <para>4、**错误可定位**：失败抛 <see cref="DataTableParseException"/>，带表名/行号/列名；</para>
    /// <para>5、**支持引号**：`"a,b"` 不会被当成分隔符，`""` 表示一个引号（Excel 导出的 CSV 常带引号）。</para>
    /// </summary>
    /// <typeparam name="T">行类型</typeparam>
    public sealed class CsvDataRowParser<T> : IDataRowParser<T> where T : class, IDataRow, new()
    {
        static readonly string[] emptyFields = new string[0];
        /// <summary>
        /// 分隔符，默认逗号
        /// </summary>
        public char Separator { get; set; } = ',';
        /// <summary>
        /// 首行是否为表头（默认 true；为 false 时只能配合 <see cref="IRawDataRow"/> 使用）
        /// </summary>
        public bool HasHeader { get; set; } = true;
        /// <summary>
        /// 是否跳过坏行（默认 false：整表失败，避免“少了几行却没人发现”）
        /// </summary>
        public bool SkipBadRows { get; set; }
        /// <summary>
        /// 是否裁剪字段两端空白（默认 true）
        /// </summary>
        public bool TrimFields { get; set; } = true;
        /// <summary>
        /// 注释行前缀，'\0' 表示不识别注释
        /// </summary>
        public char CommentPrefix { get; set; } = '#';
        /// <summary>
        /// 主键列名，默认 "Id"
        /// </summary>
        public string IdColumnName { get; set; } = "Id";
        /// <summary>
        /// 解析整张表
        /// </summary>
        /// <param name="assetInfo">表资源信息</param>
        /// <param name="bytes">表资源字节</param>
        /// <returns>行数组</returns>
        public T[] Parse(DataTableAssetInfo assetInfo, byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return new T[0];
            string text;
            using (var stream = new MemoryStream(bytes))
            using (var reader = new StreamReader(stream, Encoding.UTF8, true))
                text = reader.ReadToEnd();
            var tableName = assetInfo.DataTableName;
            var rows = new List<T>();
            var idLines = new Dictionary<int, int>();
            Dictionary<string, int> header = null;
            DataRowBinder<T> binder = null;
            var rawRow = typeof(IRawDataRow).IsAssignableFrom(typeof(T));
            var lineNumber = 0;
            var lineStart = 0;
            var length = text.Length;
            while (lineStart <= length)
            {
                var lineEnd = text.IndexOf('\n', lineStart);
                if (lineEnd < 0)
                    lineEnd = length;
                var line = text.Substring(lineStart, lineEnd - lineStart).TrimEnd('\r');
                lineStart = lineEnd + 1;
                lineNumber++;
                var trimmedLine = line.Trim();
                if (trimmedLine.Length == 0)
                    continue;
                if (CommentPrefix != '\0' && trimmedLine[0] == CommentPrefix)
                    continue;
                if (header == null)
                {
                    if (HasHeader)
                    {
                        header = BuildHeader(tableName, lineNumber, trimmedLine);
                        binder = rawRow ? null : DataRowBinder<T>.Create(header, IdColumnName);
                        continue;
                    }
                    if (!rawRow)
                        throw new DataTableParseException(tableName, lineNumber, null,
                            "没有表头时只支持实现 IRawDataRow 的行类型（否则无法按列名绑定字段）");
                    header = new Dictionary<string, int>(StringComparer.Ordinal);
                }
                var fields = SplitLine(line);
                T row = null;
                string error = null;
                try
                {
                    row = CreateRow(fields, header, binder, rawRow);
                }
                catch (Exception e)
                {
                    error = e.Message;
                }
                if (row == null)
                    error = error ?? "解析结果为空";
                if (error == null)
                {
                    int firstLine;
                    if (idLines.TryGetValue(row.Id, out firstLine))
                        error = $"主键重复：Id={row.Id} 已在第 {firstLine} 行出现";
                    else
                        idLines.Add(row.Id, lineNumber);
                }
                if (error != null)
                {
                    if (SkipBadRows)
                        continue;
                    throw new DataTableParseException(tableName, lineNumber, null, error);
                }
                rows.Add(row);
            }
            if (HasHeader && header == null)
                throw new DataTableParseException(tableName, 1, null, "表头缺失（文件为空或只有注释）");
            return rows.ToArray();
        }
        T CreateRow(string[] fields, Dictionary<string, int> header, DataRowBinder<T> binder, bool rawRow)
        {
            var row = new T();
            if (rawRow)
            {
                var raw = (IRawDataRow)row;
                return raw.ParseFields(fields, header) ? row : null;
            }
            binder.Bind(row, fields);
            return row;
        }
        Dictionary<string, int> BuildHeader(string tableName, int lineNumber, string line)
        {
            var fields = SplitLine(line);
            var header = new Dictionary<string, int>(StringComparer.Ordinal);
            for (int i = 0; i < fields.Length; i++)
            {
                var name = fields[i] == null ? string.Empty : fields[i].Trim();
                if (name.Length == 0)
                    continue;
                if (header.ContainsKey(name))
                    throw new DataTableParseException(tableName, lineNumber, name, "表头列名重复");
                header.Add(name, i);
            }
            if (header.Count == 0)
                throw new DataTableParseException(tableName, lineNumber, null, "表头为空");
            int idIndex;
            if (!header.TryGetValue(IdColumnName, out idIndex))
                throw new DataTableParseException(tableName, lineNumber, IdColumnName, $"表头缺少主键列 \"{IdColumnName}\"");
            return header;
        }
        /// <summary>
        /// 切分一行（支持双引号包裹与 "" 转义）
        /// </summary>
        string[] SplitLine(string line)
        {
            if (string.IsNullOrEmpty(line))
                return emptyFields;
            var fields = new List<string>(8);
            var builder = new StringBuilder();
            var inQuotes = false;
            for (int i = 0; i < line.Length; i++)
            {
                var ch = line[i];
                if (inQuotes)
                {
                    if (ch == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            builder.Append('"');
                            i++;
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        builder.Append(ch);
                    }
                    continue;
                }
                if (ch == '"')
                {
                    inQuotes = true;
                    continue;
                }
                if (ch == Separator)
                {
                    fields.Add(TrimFields ? builder.ToString().Trim() : builder.ToString());
                    builder.Length = 0;
                    continue;
                }
                builder.Append(ch);
            }
            fields.Add(TrimFields ? builder.ToString().Trim() : builder.ToString());
            return fields.ToArray();
        }
    }
}
