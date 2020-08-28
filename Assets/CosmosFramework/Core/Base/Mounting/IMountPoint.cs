using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos
{
    /// <summary>
    /// 基于mono的挂载对象接口
    /// </summary>
    public interface IMountPoint
    {
        GameObject MountPoint{ get; }
        Type MountType { get; }
    }
}