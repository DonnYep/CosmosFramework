using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 数据代理类
/// </summary>
namespace Cosmos.Mvvm
{
    public abstract class Proxy
    {
        public abstract void OnBind();
        public abstract void OnUnbind();

    }
}