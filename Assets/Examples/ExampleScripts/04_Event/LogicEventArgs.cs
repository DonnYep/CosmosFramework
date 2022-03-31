using Cosmos;
/// <summary>
/// 泛型数据传输容器
/// 封闭继承
/// </summary>
public sealed class LogicEventArgs<T> : GameEventArgs
{
    T data;
    public LogicEventArgs() { }
    public LogicEventArgs(T data)
    {
        this.data = data;
    }
    public override void Release()
    {
        data = default(T);
    }
    public LogicEventArgs<T> SetData(T data)
    {
        this.data = data;
        return this;
    }
    public T GetData()
    {
        return data;
    }
}