using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;

/// <summary>
/// UI模块具体实现的参数类，仅做示例
/// </summary>
public class UIImplementArgs<T> : UIEventArgs
{
    /// <summary>
    /// 泛型构造
    /// </summary>
    /// <param name="data"></param>
    public UIImplementArgs(T data)
    {
        SetData(data);
    }
    public UIImplementArgs() { }
    /// <summary>
    /// 泛型数据类型
    /// </summary>
    public T Data { get;private set; }
    public void SetData(T data)
    {
        Data = data;
    }
    public override void Clear()
    {
        Data = default(T);
    }
}
