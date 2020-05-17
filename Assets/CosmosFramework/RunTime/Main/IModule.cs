using UnityEngine;
using System.Collections;
using System;
namespace Cosmos
{
    public interface IModule: IControllableBehaviour
    {
        /// <summary>
        /// 容器挂载的对象
        /// </summary>
        GameObject ModuleMountObject { get; }
        string ModuleFullyQualifiedName { get; }
    }
}
