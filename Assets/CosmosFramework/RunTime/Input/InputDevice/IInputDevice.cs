using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.Input{

    public interface IInputDevice
    {
        /// <summary>
        /// 设备启动
        /// </summary>
        void OnStart();
        /// <summary>
        /// 设备运行
        /// </summary>
        void OnRun();
        /// <summary>
        /// 设备关闭
        /// </summary>
        void OnShutdown();
    }
}