using Cosmos.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cosmos
{
    public interface IUIManager:IModuleManager
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
        /// 通过特性UIAssetAttribute加载Panel
        /// </summary>
        /// <typeparam name="T">带有UIAssetAttribute特性的panel类</typeparam>
        /// <param name="callback">加载成功的回调。若失败，则不执行</param>
        void OpenUIAsync<T>(Action<T> callback = null) where T : UILogicBase;
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel
        /// </summary>
        /// <param name="type">带有UIAssetAttribute特性的panel类<</param>
        /// <param name="callback">加载成功的回调。若失败，则不执行</param>
        void OpenUIAsync(Type type, Action<UILogicBase> callback = null);
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
        /// 是否存在UI
        /// <see cref=" UILogicBase",>
        /// UILogicBase.UIName
        /// </summary>
        /// <param name="panelName">UILogicBase.UIName</param>
        /// <returns>是否存在的结果</returns>
        bool HasUI(string panelName);
    }
}
