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
        T OpenUI<T>() where T : UILogicBase;
        /// <summary>
        ///  通过特性UIAssetAttribute加载Panel（同步）
        /// </summary>
        /// <param name="type">目标组件的type类型</param>
        /// <returns>生成的UI对象Comp</returns>
        UILogicBase OpenUI(Type type);
        /// <summary>
        ///  通过AssetInfo加载UI对象（同步）；
        /// </summary>
        /// <typeparam name="T">目标组件的type类型</typeparam>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <returns>生成的UI对象Comp</returns>
        T OpenUI<T>(AssetInfo assetInfo) where T : UILogicBase;
        /// <summary>
        /// 通过AssetInfo加载UI对象（同步）；
        /// </summary>
        /// <param name="assetInfo">目标组件的type类型</param>
        /// <param name="type">传入的assetInfo对象</param>
        /// <returns>生成的UI对象Comp</returns>
        UILogicBase OpenUI(AssetInfo assetInfo, Type type);
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel
        /// </summary>
        /// <typeparam name="T">带有UIAssetAttribute特性的panel类</typeparam>
        /// <param name="callback">加载成功的回调。若失败，则不执行</param>
        Coroutine OpenUIAsync<T>(Action<T> callback = null) where T : UILogicBase;
        /// <summary>
        /// 通过AssetInfo加载UI对象
        /// </summary>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="type">目标组件的type类型</param>
        /// <param name="loadDoneCallback">加载完成后的回调</param>
        /// <returns></returns>
        Coroutine OpenUIAsync(AssetInfo assetInfo, Type type, Action<UILogicBase> loadDoneCallback = null);
        /// <summary>
        /// 通过AssetInfo加载UI对象
        /// </summary>
        /// <typeparam name="T">目标UI组件</typeparam>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="loadDoneCallback">加载完成后的回调</param>
        Coroutine OpenUIAsync<T>(AssetInfo assetInfo, Action<T> loadDoneCallback = null) where T : UILogicBase;
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel
        /// </summary>
        /// <param name="type">带有UIAssetAttribute特性的panel类</param>
        /// <param name="loadDoneCallback">加载成功的回调。若失败，则不执行</param>
        /// <returns></returns>
        Coroutine OpenUIAsync(Type type, Action<UILogicBase> loadDoneCallback = null);
        /// <summary>
        /// 隐藏UI，调用UI中的HidePanel方法；
        /// <see cref=" UILogicBase",>
        /// UILogicBase.UIName
        /// </summary>
        /// <param name="uiName">UILogicBase.UIName</param>
        void HideUI(string uiName);
        /// <summary>
        /// 移除UI，但是不销毁
        /// <see cref=" UILogicBase",>
        /// UILogicBase.UIName
        /// </summary>
        /// <param name="uiName">UILogicBase.UIName</param>
        /// <param name="panel">移除后返回的panel</param>
        void RemoveUI(string uiName, out UILogicBase panel);
        /// <summary>
        /// 移除UI，但是不销毁
        /// <see cref=" UILogicBase",>
        /// UILogicBase.UIName
        /// </summary>
        /// <param name="uiName">UILogicBase.UIName</param>
        void RemoveUI(string uiName);
        /// <summary>
        /// 销毁UI
        /// <see cref=" UILogicBase",>
        /// UILogicBase.UIName
        /// </summary>
        /// <param name="uiName">UILogicBase.UIName</param>
        void DestroyUl(string uiName);
        /// <summary>
        /// 是否存在UI;
        /// <see cref=" UILogicBase",>
        /// UILogicBase.UIName
        /// </summary>
        /// <param name="uiName">UILogicBase.UIName</param>
        /// <returns>存在的结果</returns>
        bool HasUI(string uiName);
    }
}
