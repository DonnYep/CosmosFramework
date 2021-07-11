using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos.UI
{
    /// <summary>
    /// UIForm资源帮助体，负责处理资源加载销毁；
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
        Coroutine InstanceUIFormAsync(UIAssetInfo assetInfo, Type uiType, Action<UIForm> doneCallback);
        /// <summary>
        /// 同步实例化UIForm；
        /// </summary>
        /// <param name="assetInfo"> UI资源信息</param>
        /// <param name="uiType">UI类型</param>
        /// <returns>实例化后的UIForm 对象</returns>
        UIForm InstanceUIForm(UIAssetInfo assetInfo, Type uiType);
        /// <summary>
        /// 释放UIForm；
        /// </summary>
        /// <param name="uiForm">UIForm对象</param>
        void ReleaseUIForm(UIForm uiForm);
    }
}
