using UnityEngine;
using System.Collections;
using System;
namespace Cosmos
{
    public interface IModule: IControllable
    {
        /// <summary>
        /// 容器挂载的对象
        /// </summary>
        GameObject ModuleMountObject { get; }
        [Obsolete("临时的注销方法")]
        void Deregister();
    }
}
