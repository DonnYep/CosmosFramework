﻿using System;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos.UI
{
    //================================================
    /*
     * 1、UI模块。UI模块管理的对象为IUIForm，因此支持多种UI方案。
     * 
     * 2、框架目前内置了UGUI支持。若需要支持如FGUI、NGUI等UI方案，
     * 参照UGUI的支持库写法即可。
     */
    //================================================
    public interface IUIManager : IModuleManager, IModuleInstance
    {
        /// <summary>
        /// UIForm激活回调；
        /// </summary>
        event Action<IUIForm> OnUIFormActiveCallback;
        /// <summary>
        /// UIForm失活回调；
        /// </summary>
        event Action<IUIForm> OnUIFormDeactiveCallback;
        /// <summary>
        /// UIForm释放回调；
        /// </summary>
        event Action<IUIForm> OnUIFormReleaseCallback;
        /// <summary>
        /// UIForm加载完成回调；
        /// </summary>
        event Action<IUIForm> OnUIFormLoadCallback;
        /// <summary>
        /// 设置ui资产加载帮助体；
        /// </summary>
        /// <param name="helper">帮助体对象</param>
        void SetUIFormAssetHelper(IUIFormAssetHelper helper);
        /// <summary>
        /// 通过UIAssetInfo加载UI对象（异步）；
        /// </summary>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="uiType">目标组件的type类型</param>
        /// <param name="callback">加载完成后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine OpenUIFormAsync(UIAssetInfo assetInfo, Type uiType, Action<IUIForm> callback = null);
        /// <summary>
        /// 通过UIAssetInfo加载UI对象
        /// </summary>
        /// <typeparam name="T">目标UI组件</typeparam>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="callback">加载完成后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine OpenUIFormAsync<T>(UIAssetInfo assetInfo, Action<T> callback = null) where T : class, IUIForm;
        /// <summary>
        /// 通过UIAssetInfo加载UI对象
        /// </summary>
        /// <typeparam name="T">目标UI组件</typeparam>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <returns>Task异步任务</returns>
        Task<T> OpenUIFormAsync<T>(UIAssetInfo assetInfo) where T : class, IUIForm;
        /// <summary>
        ///  通过UIAssetInfo加载UI对象
        /// </summary>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="uiType">目标组件的type类型</param>
        /// <returns>Task异步任务</returns>
        Task<IUIForm> OpenUIFormAsync(UIAssetInfo assetInfo, Type uiType);
        /// <summary>
        /// 释放UIForm；
        /// 此操作会释放UIForm对象；
        /// </summary>
        /// <param name="uiFormName">UI资源的名称</param>
        void ReleaseUIForm(string uiFormName);
        /// <summary>
        /// 关闭释放UIForm；
        /// 此操作会释放UIForm对象；
        /// </summary>
        /// <param name="uiForm">open的UIForm</param>
        void ReleaseUIForm(IUIForm uiForm);
        /// <summary>
        /// 释放关闭整个组；
        /// </summary>
        /// <param name="uiGroupName">UI组的名字</param>
        void ReleaseUIGroup(string uiGroupName);
        /// <summary>
        /// 激活已加载的UIForm；
        /// </summary>
        /// <param name="uiFormName">UI资源的名称</param>
        void ActiveUIForm(string uiFormName);
        /// <summary>
        /// 激活已加载的UIForm；
        /// </summary>
        /// <param name="uiForm">已加载的UIForm</param>
        void ActiveUIForm(IUIForm uiForm);
        /// <summary>
        /// 失活已加载的UIForm；
        /// </summary>
        /// <param name="uiFormName">UI资源的名称</param>
        void DeactiveUIForm(string uiFormName);
        /// <summary>
        /// 失活已加载的UIForm；
        /// </summary>
        /// <param name="uiForm">已加载的UIForm</param>
        void DeactiveUIForm(IUIForm uiForm);
        /// <summary>
        /// 失活整个ui组；
        /// </summary>
        /// <param name="uiGroupName">UI组的名字</param>
        void DeactiveUIGroup(string uiGroupName);
        /// <summary>
        /// 激活整个UI组；
        /// </summary>
        /// <param name="uiGroupName">ui组的名字</param>
        void ActiveUIGroup(string uiGroupName);
        /// <summary>
        /// 是否存在UI;
        ///<see cref="IUIForm"/>
        /// UIForm.UIName
        /// </summary>
        /// <param name="uiFormName">UI资源的名称</param>
        /// <returns>存在的结果</returns>
        bool HasUIForm(string uiFormName);
        /// <summary>
        /// 获取UIForm；
        /// </summary>
        bool PeekUIForm(string uiFormName, out IUIForm uiForm);
        /// <summary>
        /// 获取UIForm；
        /// </summary>
        bool PeekUIForm<T>(string uiFormName, out T uiForm) where T : class, IUIForm;
        /// <summary>
        /// 通过条件选择组中的UIForm；
        /// </summary>
        /// <param name="uiGroupName">UI组的名字</param>
        /// <param name="condition">条件委托</param>
        /// <returns>符合条件的UIForm</returns>
        IUIForm[] FindUIForms(string uiGroupName, Predicate<IUIForm> condition);
        /// <summary>
        /// 通过条件选择UIForm
        /// </summary>
        /// <param name="condition">条件委托</param>
        /// <returns>符合条件的UIForm</returns>
        IUIForm[] FindUIForms(Predicate<IUIForm> condition);
        /// <summary>
        /// 设置UIForm的组别；
        /// 若UIForm本身已存在组别，则被移除旧组，加入新组；
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
