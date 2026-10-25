using System.Collections.Generic;

namespace Cosmos.DataTable
{
    /// <summary>
    /// 可选接口：由行自己完成解析，从而完全避免反射。
    /// <para>适合代码生成的表类（生产环境推荐）：生成器按列名直接赋值，性能与手写一致。</para>
    /// <para>实现此接口后，默认解析器不再反射写字段。</para>
    /// </summary>
    public interface IRawDataRow
    {
        /// <summary>
        /// 用一行字段填充数据。
        /// </summary>
        /// <param name="fields">按分隔符切好的字段</param>
        /// <param name="headerIndex">列名 → 列序号（无表头时为 null）</param>
        /// <returns>是否解析成功</returns>
        bool ParseFields(string[] fields, IReadOnlyDictionary<string, int> headerIndex);
    }
}
