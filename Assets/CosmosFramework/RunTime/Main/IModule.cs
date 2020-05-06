using UnityEngine;
using System.Collections;
using System;
namespace Cosmos
{
    public interface IModule: IWholeBehaviour
    {
        /// <summary>
        /// 容器挂载的对象
        /// </summary>
        GameObject ModuleMountObject { get; }
    }
}
