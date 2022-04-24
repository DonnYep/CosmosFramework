namespace Cosmos.DataTable
{
    public interface IDataRow
    {
        /// <summary>
        /// 行索引
        /// </summary>
        int Id { get; }
        /// <summary>
        /// 解析一条数据
        /// </summary>
        /// <param name="dataBytes">行数据</param>
        /// <returns>解析结果</returns>
        bool ParseData(byte[] dataBytes);
        /// <summary>
        /// 解析一条数据
        /// </summary>
        /// <param name="dataString">行数据</param>
        /// <returns>解析结果</returns>
        bool ParseData(string dataString);
    }
}
