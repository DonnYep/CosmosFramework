using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    /// <summary>
    /// 框架所有输入可控抽象基类
    /// 无参轮询特性支持；
    /// <see cref="TickRefreshAttribute"/>
    /// <see cref="LateRefreshAttribute"/>
    /// <see cref="FixedRefreshAttribute"/>
    /// </summary>
    public interface  IController:IReference
    {
        int Id { get; }
        string GroupName { get; }
        object Handle { get; }
        Type HandleType { get; }
        string ControllerName { get; }
        bool Pause { get; set; }
    }
}