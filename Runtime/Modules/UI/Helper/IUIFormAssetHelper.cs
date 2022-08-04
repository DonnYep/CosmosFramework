using System;
using UnityEngine;
namespace Cosmos.UI
{
    /// <summary>
    /// UIForm资源帮助体，负责处理资源加载销毁；
    /// </summary>
    public interface IUIFormAssetHelper
    {
        /// <summary>
        /// 附加UIForm到一个UIForm上；
        /// </summary>
        /// <param name="src">挂载到其他对象</param>
        /// <param name="dst">被其他对象挂载</param>
        void AttachTo(IUIForm src, IUIForm dst);
        /// <summary>
        /// 移除挂载；
        /// </summary>
        /// <param name="src">原本挂在其他form的对象</param>
        /// <param name="dst">原本被其他对象挂着的form</param>
        void DetachFrom(IUIForm src, IUIForm dst);
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
