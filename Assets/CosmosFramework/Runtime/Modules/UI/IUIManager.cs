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
        /// <summary>
        /// 设置ui动效帮助体；
        /// </summary>
        /// <param name="helper">帮助体对象</param>
        void SetMotionHelper(IUIFormMotionHelper helper);
        /// <summary>
        /// 设置ui资产加载帮助体；
        /// </summary>
        /// <param name="helper">帮助体对象</param>
        void SetUIFormAssetHelper(IUIFormAssetHelper helper);
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel（同步）；
        /// </summary>
        /// <typeparam name="T">目标组件的type类型</typeparam>
        /// <returns>生成的UI对象Comp</returns>
        T OpenUIForm<T>() where T : UIForm;
        /// <summary>
        ///  通过特性UIAssetAttribute加载Panel（同步）
        /// </summary>
        /// <param name="uiType">目标组件的type类型</param>
        /// <returns>生成的UI对象Comp</returns>
        UIForm OpenUIForm(Type uiType);
        /// <summary>
        ///  通过UIAssetInfo加载UI对象（同步）；
        /// </summary>
        /// <typeparam name="T">目标组件的type类型</typeparam>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <returns>生成的UI对象Comp</returns>
        T OpenUIForm<T>(UIAssetInfo assetInfo) where T : UIForm;
        /// <summary>
        /// 通过UIAssetInfo加载UI对象（同步）；
        /// </summary>
        /// <param name="assetInfo">目标组件的type类型</param>
        /// <param name="uiType">传入的assetInfo对象</param>
        /// <returns>生成的UI对象Comp</returns>
        UIForm OpenUIForm(UIAssetInfo assetInfo, Type uiType);
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel（异步）；
        /// </summary>
        /// <typeparam name="T">带有UIAssetAttribute特性的panel类</typeparam>
        /// <param name="callback">加载成功的回调。若失败，则不执行</param>
        /// <returns>协程对象</returns>
        Coroutine OpenUIFormAsync<T>(Action<T> callback = null) where T : UIForm;
        /// <summary>
        /// 通过UIAssetInfo加载UI对象（异步）；
        /// </summary>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="uiType">目标组件的type类型</param>
        /// <param name="callback">加载完成后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine OpenUIFormAsync(UIAssetInfo assetInfo, Type uiType, Action<UIForm> callback = null);
        /// <summary>
        /// 通过UIAssetInfo加载UI对象
        /// </summary>
        /// <typeparam name="T">目标UI组件</typeparam>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="callback">加载完成后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine OpenUIFormAsync<T>(UIAssetInfo assetInfo, Action<T> callback = null) where T : UIForm;
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel（异步）；
        /// </summary>
        /// <param name="uiType">带有UIAssetAttribute特性的panel类</param>
        /// <param name="callback">加载成功的回调。若失败，则不执行</param>
        /// <returns>协程对象</returns>
        Coroutine OpenUIFormAsync(Type uiType, Action<UIForm> callback = null);
        /// <summary>
        /// 释放UIForm；
        /// 此操作会释放UIForm对象；
        /// </summary>
        /// <param name="uiFormName">UI资源的名称</param>
        void ReleaseUIForm(string uiFormName);
        /// <summary>
        /// 失活UIForm，并触发UIForm中的OnDeactive回调；
        /// </summary>
        /// <param name="uiFormName">UI资源的名称</param>
        void DeactiveUIForm(string uiFormName);
        /// <summary>
        /// 激活UIForm,并触发UIForm中的OnActive回调；
        /// </summary>
        /// <param name="uiFormName">UI资源的名称</param>
        void ActiveUIForm(string uiFormName);
        /// <summary>
        /// 释放整个组；
        /// </summary>
        /// <param name="uiGroupName">ui组的名字</param>
        void ReleaseUIGroup(string uiGroupName);
        /// <summary>
        /// 失活整个ui组；
        /// </summary>
        /// <param name="uiGroupName">ui组的名字</param>
        void DeactiveUIGroup(string uiGroupName);
        /// <summary>
        /// 激活整个UI组；
        /// </summary>
        /// <param name="uiGroupName">ui组的名字</param>
        void ActiveUIGroup(string uiGroupName);
        /// <summary>
        /// 是否存在UI;
        /// <see cref=" UIForm",>
        /// UIForm.UIName
        /// </summary>
        /// <param name="uiFormName">UI资源的名称</param>
        /// <returns>存在的结果</returns>
        bool HasUIForm(string uiFormName);
        /// <summary>
        /// 获取UIForm；
        /// </summary>
        T PeekUIForm<T>(string uiFormName) where T : UIForm;
        /// <summary>
        /// 获取UIForm；
        /// </summary>
        UIForm PeekUIForm(string uiFormName);
        /// <summary>
        /// 通过条件选择组中的UIForm；
        /// </summary>
        /// <param name="uiGroupName">UI组的名字</param>
        /// <param name="handler">条件委托</param>
        /// <returns>符合条件的UIForm</returns>
        UIForm[] FindUIForms(string uiGroupName, Predicate<UIForm> handler);
        /// <summary>
        /// 通过条件选择UIForm
        /// </summary>
        /// <param name="handler">条件委托</param>
        /// <returns>符合条件的UIForm</returns>
        UIForm[] FindtUIForms(Predicate<UIForm> handler);
        /// <summary>
        /// 设置UIForm的组别；
        /// </summary>
        /// <param name="uiFormName">UI资源的名称</param>
        /// <param name="uiGroupName">UI组的名字</param>
        void GroupUIForm(string uiFormName, string uiGroupName);
        /// <summary>
        /// 解除UIForm的组别；
        /// </summary>
        /// <param name="uiFormName">UI资源的名称</param>
        void UngroupUIForm(string uiFormName);
    }
}
