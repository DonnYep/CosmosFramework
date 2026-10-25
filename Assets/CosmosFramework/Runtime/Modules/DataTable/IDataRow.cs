namespace Cosmos.DataTable
{
    /// <summary>
    /// 一条数据行。
    /// <para>设计要求（生产视角）：</para>
    /// <para>1、行对象在加载完成后就是**只读共享**的：同一份表被所有消费者共享，行不再被回收或复用；
    /// 因此不要在这里做“重置/复用”逻辑，也不要暴露可写字段给业务改。</para>
    /// <para>2、行不再自己负责解析（旧版的 ParseData/Dispose 已移除）：解析由
    /// <see cref="IDataRowParser{T}"/> 决定；想零反射解析就实现 <see cref="IRawDataRow"/>。</para>
    /// </summary>
    public interface IDataRow
    {
        /// <summary>
        /// 行主键（表内唯一）
        /// </summary>
        int Id { get; }
    }
}
