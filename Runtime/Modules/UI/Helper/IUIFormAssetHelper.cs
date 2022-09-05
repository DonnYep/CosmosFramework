using System;
using UnityEngine;
namespace Cosmos.UI
{
    /// <summary>
    /// UIForm资源帮助体，处理资源加载与释放；
    /// </summary>
    public interface IUIFormAssetHelper
    {
        /// <summary>
        /// 异步实例化UIForm；
        /// </summary>
        /// <param name="assetInfo"> UI资源信息</param>
        /// <param name="uiType">UI类型</param>
        /// <param name="doneCallback">实例化完成回调</param>
        /// <returns>协程对象</returns>
        Coroutine InstanceUIFormAsync(UIAssetInfo assetInfo, Type uiType, Action<IUIForm> doneCallback);
        /// <summary>
        /// 关闭并释放UIForm；
        /// </summary>
        /// <param name="uiForm">UIForm对象</param>
        void ReleaseUIForm(IUIForm uiForm);
    }
}
