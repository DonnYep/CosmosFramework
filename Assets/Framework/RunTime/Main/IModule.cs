using UnityEngine;
using System.Collections;
using System;
namespace Cosmos
{
    public interface IModule
    {
        /// <summary>
        /// 容器挂载的对象
        /// </summary>
        GameObject ModuleMountObject { get; }
        void DeregisterModule();
        /// <summary>
        /// 初始化
        /// </summary>
        void OnInitialization();
        /// <summary>
        /// 暂停
        /// </summary>
        void OnPause();
        /// <summary>
        /// 恢复暂停
        /// </summary>
        void OnUnPause();
        /// <summary>
        /// 停止
        /// </summary>
        void OnTermination();
    }
}
