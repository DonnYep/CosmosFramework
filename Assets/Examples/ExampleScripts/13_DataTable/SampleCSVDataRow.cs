using Cosmos.DataTable;
using System.Collections.Generic;

/// <summary>
/// 数据表行示例。
/// <para>实现 <see cref="IRawDataRow"/>：按**列名**取值并赋值，全程零反射
/// （生产环境推荐用代码生成器产出这类代码；只想省事也可以不实现此接口，
/// 由默认解析器按字段名反射绑定，反射只发生在读表头时）。</para>
/// </summary>
public class SampleCSVDataRow : IDataRow, IRawDataRow
{
    readonly SampleCSV sample = new SampleCSV();
    /// <summary>
    /// 行主键
    /// </summary>
    public int Id { get { return sample.Id; } }
    /// <summary>
    /// 业务数据
    /// </summary>
    public SampleCSV SampleCSV { get { return sample; } }
    /// <summary>
    /// 按列名解析一行
    /// </summary>
    public bool ParseFields(string[] fields, IReadOnlyDictionary<string, int> headerIndex)
    {
        if (fields == null || headerIndex == null)
            return false;
        int id;
        int jobNumber;
        float amountOfSales;
        if (!TryGetInt(fields, headerIndex, "Id", out id))
            return false;
        if (!TryGetInt(fields, headerIndex, "JobNumber", out jobNumber))
            return false;
        if (!TryGetFloat(fields, headerIndex, "AmountOfSales", out amountOfSales))
            return false;
        sample.Id = id;
        sample.JobNumber = jobNumber;
        sample.AmountOfSales = amountOfSales;
        sample.Desc = GetString(fields, headerIndex, "Desc");
        sample.Name = GetString(fields, headerIndex, "Name");
        sample.CompanyName = GetString(fields, headerIndex, "CompanyName");
        return true;
    }
    static string GetString(string[] fields, IReadOnlyDictionary<string, int> headerIndex, string column)
    {
        int index;
        if (!headerIndex.TryGetValue(column, out index) || index < 0 || index >= fields.Length)
            return string.Empty;
        return fields[index] ?? string.Empty;
    }
    static bool TryGetInt(string[] fields, IReadOnlyDictionary<string, int> headerIndex, string column, out int value)
    {
        value = 0;
        var text = GetString(fields, headerIndex, column);
        if (string.IsNullOrEmpty(text))
            return true;
        return int.TryParse(text, out value);
    }
    static bool TryGetFloat(string[] fields, IReadOnlyDictionary<string, int> headerIndex, string column, out float value)
    {
        value = 0f;
        var text = GetString(fields, headerIndex, column);
        if (string.IsNullOrEmpty(text))
            return true;
        return float.TryParse(text, out value);
    }
}
