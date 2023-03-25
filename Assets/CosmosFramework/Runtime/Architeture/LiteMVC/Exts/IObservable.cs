namespace LiteMVC
{
    /// <summary>
    /// 实现此接口，可通过 this关键字直接使用MVC的方法；
    /// 注意：若实现了此接口的对象绑定(Bind)函数，则在被销毁&回收时务必解绑(Unbind)
    /// </summary>
    public interface IObservable { }
}
