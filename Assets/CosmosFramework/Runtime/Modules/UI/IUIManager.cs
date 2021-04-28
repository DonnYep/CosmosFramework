using Cosmos.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cosmos
{
    public interface IUIManager : IModuleManager
    {
        /// <summary>
        /// UI根节点对象；
        /// </summary>
        GameObject UIRoot { get; }
        void SetUIRoot(GameObject uiRoot, bool destroyOldOne = false);
        void SetHelper(IUIFormHelper helper);
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel（同步）；
        /// </summary>
        /// <typeparam name="T">目标组件的type类型</typeparam>
        /// <returns>生成的UI对象Comp</returns>
        T OpenUIForm<T>() where T : UIFormBase;
        /// <summary>
        ///  通过特性UIAssetAttribute加载Panel（同步）
        /// </summary>
        /// <param name="uiType">目标组件的type类型</param>
        /// <returns>生成的UI对象Comp</returns>
        UIFormBase OpenUIForm(Type uiType);
        /// <summary>
        ///  通过UIAssetInfo加载UI对象（同步）；
        /// </summary>
        /// <typeparam name="T">目标组件的type类型</typeparam>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <returns>生成的UI对象Comp</returns>
         T OpenUIForm<T>(UIAssetInfo assetInfo) where T : UIFormBase;
        /// <summary>
        /// 通过UIAssetInfo加载UI对象（同步）；
        /// </summary>
        /// <param name="assetInfo">目标组件的type类型</param>
        /// <param name="uiType">传入的assetInfo对象</param>
        /// <returns>生成的UI对象Comp</returns>
        UIFormBase OpenUIForm(UIAssetInfo assetInfo, Type uiType);
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel（异步）；
        /// </summary>
        /// <typeparam name="T">带有UIAssetAttribute特性的panel类</typeparam>
        /// <param name="loadDoneCallback">加载成功的回调。若失败，则不执行</param>
        /// <returns>协程对象</returns>
        Coroutine OpenUIFormAsync<T>(Action<T> loadDoneCallback = null) where T : UIFormBase;
        /// <summary>
        /// 通过UIAssetInfo加载UI对象（异步）；
        /// </summary>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="uiType">目标组件的type类型</param>
        /// <param name="loadDoneCallback">加载完成后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine OpenUIFormAsync(UIAssetInfo assetInfo, Type uiType, Action<UIFormBase> loadDoneCallback = null);
        /// <summary>
        /// 通过UIAssetInfo加载UI对象
        /// </summary>
        /// <typeparam name="T">目标UI组件</typeparam>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="loadDoneCallback">加载完成后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine OpenUIFormAsync<T>(UIAssetInfo assetInfo, Action<T> loadDoneCallback = null) where T : UIFormBase;
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel（异步）；
        /// </summary>
        /// <param name="type">带有UIAssetAttribute特性的panel类</param>
        /// <param name="loadDoneCallback">加载成功的回调。若失败，则不执行</param>
        /// <returns>协程对象</returns>
        Coroutine OpenUIFormAsync(Type type, Action<UIFormBase> loadDoneCallback = null);
        void CloseUIForm(string uiAssetName);
        /// <summary>
        /// 隐藏UI，调用UI中的HidePanel方法；
        /// <see cref=" UIFormBase",>
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiAssetName">ui资源的名称</param>
        void HideUIForm(string uiAssetName);
        void ShowUIForm(string uiAssetName);
        /// <summary>
        /// 是否存在UI;
        /// <see cref=" UIFormBase",>
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiAssetName">ui资源的名称</param>
        /// <returns>存在的结果</returns>
        bool HasUIForm(string uiAssetName);
        T PeekUIForm<T>(string uiAssetName) where T : UIFormBase;
        UIFormBase PeekUIForm(string uiAssetName);
    }
}
