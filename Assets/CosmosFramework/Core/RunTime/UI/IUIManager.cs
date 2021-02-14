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
        /// <summary>
        /// 设置UI根节点
        /// </summary>
        /// <param name="uiRoot">传入的UIRoot</param>
        /// <param name="destroyOldOne">销毁旧的uiRoot对象</param>
        void SetUIRoot(GameObject uiRoot, bool destroyOldOne = false);
        void SetHelper(IUIFormHelper helper);
        T OpenUIForm<T>() where T : UIFormBase;
        /// <summary>
        ///  通过特性UIAssetAttribute加载Panel（同步）
        /// </summary>
        /// <param name="type">目标组件的type类型</param>
        /// <returns>生成的UI对象Comp</returns>
        UIFormBase OpenUIForm(Type type);
        /// <summary>
        ///  通过AssetInfo加载UI对象（同步）；
        /// </summary>
        /// <typeparam name="T">目标组件的type类型</typeparam>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <returns>生成的UI对象Comp</returns>
        T OpenUIForm<T>(AssetInfo assetInfo) where T : UIFormBase;
        /// <summary>
        /// 通过AssetInfo加载UI对象（同步）；
        /// </summary>
        /// <param name="assetInfo">目标组件的type类型</param>
        /// <param name="type">传入的assetInfo对象</param>
        /// <returns>生成的UI对象Comp</returns>
        UIFormBase OpenUIForm(AssetInfo assetInfo, Type type);
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel
        /// </summary>
        /// <typeparam name="T">带有UIAssetAttribute特性的panel类</typeparam>
        /// <param name="callback">加载成功的回调。若失败，则不执行</param>
        Coroutine OpenUIFormAsync<T>(Action<T> callback = null) where T : UIFormBase;
        /// <summary>
        /// 通过AssetInfo加载UI对象
        /// </summary>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="type">目标组件的type类型</param>
        /// <param name="loadDoneCallback">加载完成后的回调</param>
        /// <returns></returns>
        Coroutine OpenUIFormAsync(AssetInfo assetInfo, Type type, Action<UIFormBase> loadDoneCallback = null);
        /// <summary>
        /// 通过AssetInfo加载UI对象
        /// </summary>
        /// <typeparam name="T">目标UI组件</typeparam>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="loadDoneCallback">加载完成后的回调</param>
        Coroutine OpenUIFormAsync<T>(AssetInfo assetInfo, Action<T> loadDoneCallback = null) where T : UIFormBase;
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel
        /// </summary>
        /// <param name="type">带有UIAssetAttribute特性的panel类</param>
        /// <param name="loadDoneCallback">加载成功的回调。若失败，则不执行</param>
        /// <returns></returns>
        Coroutine OpenUIFormAsync(Type type, Action<UIFormBase> loadDoneCallback = null);
        /// <summary>
        /// 隐藏UI，调用UI中的HidePanel方法；
        /// <see cref=" UIFormBase",>
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiName">UIFormBase.UIName</param>
        void HideUIForm(string uiName);
        void HideUIForm(UIFormBase uiForm);
        void ShowUIForm(string uiName);
        void ShowUIForm(UIFormBase uiForm);
        /// <summary>
        /// 移除UI，但是不销毁
        /// <see cref=" UIFormBase",>
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiName">UIFormBase.UIName</param>
        /// <param name="panel">移除后返回的panel</param>
        void RemoveUIForm(string uiName, out UIFormBase panel);
        /// <summary>
        /// 移除UI，但是不销毁
        /// <see cref=" UIFormBase",>
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiName">UIFormBase.UIName</param>
        void RemoveUIForm(string uiName);
        /// <summary>
        /// 销毁UI
        /// <see cref=" UIFormBase",>
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiName">UIFormBase.UIName</param>
        void DestroyUlForm(string uiName);
        void DestroyUlForm(UIFormBase uiForm);
        /// <summary>
        /// 是否存在UI;
        /// <see cref=" UIFormBase",>
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiName">UIFormBase.UIName</param>
        /// <returns>存在的结果</returns>
        bool HasUIForm(string uiName);
        /// <summary>
        /// 获得UI；
        /// </summary>
        /// <typeparam name="T">UIForm的类型</typeparam>
        /// <param name="uiName">UI的名称</param>
        /// <returns>UI对象组件</returns>
        T PeekUIForm<T>(string uiName) where T : UIFormBase;
        /// <summary>
        /// 获得UI；
        /// </summary>
        /// <param name="uiType">UIForm的类型</param>
        /// <param name="uiName">UI的名称</param>
        /// <returns>UI对象组件</returns>
        UIFormBase PeekUIForm(Type uiType, string uiName);
        /// <summary>
        /// 注册UI；
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiForm">UI对象</param>
        void RegisterUIForm(UIFormBase uiForm);
        /// <summary>
        /// 注销UI;
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiForm">UI对象</param>
        void DeregisterUIForm(UIFormBase uiForm);
    }
}
