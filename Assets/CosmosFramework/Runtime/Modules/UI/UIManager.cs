using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    [Module]
    internal sealed partial class UIManager : Module, IUIManager
    {
        #region Properties
        /// <inheritdoc/>
        Type uiFromBaseType = typeof(IUIForm);
        /// <summary>
        /// UI资产帮助体；
        /// </summary>
        IUIFormAssetHelper uiFormAssetHelper;
        /// <summary>
        /// UIGroupName===UIGroup；
        /// </summary>
        Dictionary<string, IUIFormGroup> uiGroupDict;
        /// <summary>
        /// UIFormName===UIForm；
        /// </summary>
        Dictionary<string, IUIForm> uiFormDict;
        #endregion
        #region Methods
        /// <inheritdoc/>
        public void SetUIFormAssetHelper(IUIFormAssetHelper helper)
        {
            this.uiFormAssetHelper = helper;
        }
        /// <inheritdoc/>
        public Coroutine OpenUIFormAsync(UIAssetInfo assetInfo, Type uiType, Action<IUIForm> callback = null)
        {
            CheckUIAssetInfoValid(assetInfo, uiType);
            if (uiFormDict.TryGetValue(assetInfo.UIFormName, out var ui))
            {
                ui.Active = true;
                return null;
            }
            else
                return uiFormAssetHelper.InstanceUIFormAsync(assetInfo, uiType, uiForm =>
                {
                    uiForm.UIAssetInfo = assetInfo;
                    uiFormDict.Add(assetInfo.UIFormName, uiForm);
                    uiForm.OnInit();
                    if (!string.IsNullOrEmpty(assetInfo.UIGroupName))
                    {
                        if (!uiGroupDict.TryGetValue(assetInfo.UIGroupName, out var group))
                        {
                            group = UIFormGroup.Acquire(assetInfo.UIGroupName);
                            uiGroupDict.Add(assetInfo.UIGroupName, group);
                        }
                        group.AddUIForm(uiForm);
                    }
                    uiForm.Active = true;
                    callback?.Invoke(uiForm);
                });
        }
        /// <inheritdoc/>
        public Coroutine OpenUIFormAsync<T>(UIAssetInfo assetInfo, Action<T> callback = null)
            where T : class, IUIForm
        {
            var type = typeof(T);
            CheckUIAssetInfoValid(assetInfo, type);
            if (uiFormDict.TryGetValue(assetInfo.UIFormName, out var ui))
            {
                ui.Active = true;
                return null;
            }
            else
                return OpenUIFormAsync(assetInfo, typeof(T), uiForm => { callback?.Invoke(uiForm as T); });
        }
        /// <inheritdoc/>
        public async Task<T> OpenUIFormAsync<T>(UIAssetInfo assetInfo)
            where T : class, IUIForm
        {
            T uiForm = null;
            await OpenUIFormAsync<T>(assetInfo, pnl => uiForm = pnl);
            return uiForm;
        }
        /// <inheritdoc/>
        public async Task<IUIForm> OpenUIFormAsync(UIAssetInfo assetInfo, Type uiType)
        {
            IUIForm uiForm = null;
            await OpenUIFormAsync(assetInfo, uiType, pnl => uiForm = pnl);
            return uiForm;
        }
        /// <inheritdoc/>
        public void ReleaseUIForm(string uiFormName)
        {
            CheckUIFormNameValid(uiFormName);
            uiFormDict.Remove(uiFormName, out var uiForm);
            ReleaseUIForm(uiForm);
        }
        /// <inheritdoc/>
        public void ReleaseUIForm(IUIForm uiForm)
        {
            if (uiForm == null)
                throw new ArgumentNullException("UIForm is invalid.");
            var uiFormName = uiForm.UIAssetInfo.UIFormName;
            Utility.Text.IsStringValid(uiFormName, "UIFormName is invalid !");
            if (uiFormDict.Remove(uiFormName, out var srcUIForm))
            {
                if (srcUIForm != uiForm)
                    throw new ArgumentException($"{uiFormName}'s ptr is not equal !");//指针不一致
                if (!string.IsNullOrEmpty(uiForm.UIAssetInfo.UIGroupName))
                {
                    uiGroupDict.TryGetValue(uiForm.UIAssetInfo.UIGroupName, out var group);
                    group?.RemoveUIForm(uiForm);
                    if (group.UIFormCount <= 0)
                        uiGroupDict.Remove(uiForm.UIAssetInfo.UIGroupName);
                }
                if (uiForm.Active)
                    uiForm.Active = false;
                uiForm.OnRelease();
                uiFormAssetHelper.ReleaseUIForm(uiForm);
            }
            else
                throw new ArgumentNullException($"UI  {uiFormName} is not existed or registered !");
        }
        /// <inheritdoc/>
        public void ReleaseUIGroup(string uiGroupName)
        {
            if (uiGroupDict.TryGetValue(uiGroupName, out var group))
            {
                var uiForms = group.GetAllUIForm();
                var length = uiForms.Length;
                for (int i = 0; i < length; i++)
                {
                    var uiForm = uiForms[i];
                    if (uiForm.Active)
                        uiForm.Active = false;
                    uiForm.OnRelease();
                    uiFormAssetHelper.ReleaseUIForm(uiForms[i]);
                }
                uiGroupDict.Remove(uiGroupName, out var uiFormGroup);
                UIFormGroup.Release(uiFormGroup as UIFormGroup);
            }
        }
        /// <inheritdoc/>
        public void DeactiveUIGroup(string uiGroupName)
        {
            if (uiGroupDict.TryGetValue(uiGroupName, out var group))
            {
                var uiForms = group.GetAllUIForm();
                var length = uiForms.Length;
                for (int i = 0; i < length; i++)
                    uiForms[i].Active = false;
            }
        }
        /// <inheritdoc/>
        public void ActiveUIGroup(string uiGroupName)
        {
            if (uiGroupDict.TryGetValue(uiGroupName, out var group))
            {
                var uiForms = group.GetAllUIForm();
                var length = uiForms.Length;
                for (int i = 0; i < length; i++)
                    uiForms[i].Active = true;
            }
        }
        /// <inheritdoc/>
        public bool HasUIForm(string uiFormName)
        {
            Utility.Text.IsStringValid(uiFormName, "UIFormName is invalid !");
            return uiFormDict.ContainsKey(uiFormName);
        }
        /// <inheritdoc/>
        public bool PeekUIForm<T>(string uiFormName, out T uiForm)
            where T : class, IUIForm
        {
            Utility.Text.IsStringValid(uiFormName, "UIFormName is invalid !");
            var rst = uiFormDict.TryGetValue(uiFormName, out var form);
            uiForm = form as T;
            return rst;
        }
        /// <inheritdoc/>
        public bool PeekUIForm(string uiFormName, out IUIForm uiForm)
        {
            Utility.Text.IsStringValid(uiFormName, "UIFormName is invalid !");
            return uiFormDict.TryGetValue(uiFormName, out uiForm);
        }
        /// <inheritdoc/>
        public IUIForm[] FindUIForms(string uiGroupName, Predicate<IUIForm> condition)
        {
            Utility.Text.IsStringValid(uiGroupName, "UIGroupName is invalid !");
            if (condition == null)
                throw new ArgumentNullException("Handler is invalid !");
            if (uiGroupDict.TryGetValue(uiGroupName, out var group))
                return group.GetUIForms(condition);
            else
                throw new ArgumentException($"UIGroup  {uiGroupName} is not existed !");
        }
        /// <inheritdoc/>
        public IUIForm[] FindUIForms(Predicate<IUIForm> condition)
        {
            if (condition == null)
                throw new ArgumentNullException("Handler is invalid !");
            var dst = new IUIForm[uiFormDict.Count];
            int idx = 0;
            foreach (var uiForm in uiFormDict.Values)
            {
                if (condition.Invoke(uiForm))
                {
                    dst[idx] = uiForm;
                    idx++;
                }
            }
            Array.Resize(ref dst, idx);
            return dst;
        }
        /// <inheritdoc/>
        public void GroupUIForm(string uiFormName, string uiGroupName)
        {
            CheckUIFormNameValid(uiFormName);
            if (string.IsNullOrEmpty(uiGroupName))
                throw new ArgumentNullException("UIGroupName is invalid !");
            if (!uiGroupDict.TryGetValue(uiGroupName, out var group))
            {
                group = UIFormGroup.Acquire(uiGroupName);
                uiGroupDict.Add(uiGroupName, group);
            }
            var uiForm = uiFormDict[uiFormName];
            if (!string.IsNullOrEmpty(uiForm.UIAssetInfo.UIGroupName))
            {
                if (uiGroupDict.TryGetValue(uiGroupName, out var latestGroup))
                {
                    latestGroup.RemoveUIForm(uiForm);
                    if (latestGroup.UIFormCount <= 0)
                    {
                        uiGroupDict.Remove(uiGroupName, out var uiFormGroup);
                        UIFormGroup.Release(uiFormGroup as UIFormGroup);
                    }
                }
            }
            var latesetInfo = uiForm.UIAssetInfo;
            uiForm.UIAssetInfo = new UIAssetInfo(latesetInfo.AssetName, latesetInfo.UIFormName, latesetInfo.UIGroupName);
            group.AddUIForm(uiForm);
        }
        /// <inheritdoc/>
        public void UngroupUIForm(string uiFormName)
        {
            CheckUIFormNameValid(uiFormName);
            var uiForm = uiFormDict[uiFormName];
            if (!string.IsNullOrEmpty(uiForm.UIAssetInfo.UIGroupName))
            {
                if (uiGroupDict.TryGetValue(uiForm.UIAssetInfo.UIGroupName, out var group))
                {
                    var latesetInfo = uiForm.UIAssetInfo;
                    uiForm.UIAssetInfo = new UIAssetInfo(latesetInfo.AssetName, latesetInfo.UIFormName, string.Empty);
                    group.RemoveUIForm(uiForm);
                    if (group.UIFormCount <= 0)
                    {
                        uiGroupDict.Remove(group.UIGroupName);
                        UIFormGroup.Release(group as UIFormGroup);
                    }
                }
            }
        }
        protected override void OnInitialization()
        {
            uiGroupDict = new Dictionary<string, IUIFormGroup>();
            uiFormDict = new Dictionary<string, IUIForm>();
        }
        /// <summary>
        /// 检测UIForm的名字是否有效，且是否存在；
        /// </summary>
        /// <param name="uiFormName">UI资源的名称</param>
        void CheckUIFormNameValid(string uiFormName)
        {
            Utility.Text.IsStringValid(uiFormName, "UIFormName is invalid !");
            if (!uiFormDict.ContainsKey(uiFormName))
                throw new ArgumentNullException($"UI  {uiFormName} is not existed or registered !");
        }
        void CheckUIAssetInfoValid(UIAssetInfo assetInfo, Type uiType)
        {
            if (string.IsNullOrEmpty(assetInfo.UIFormName))
                throw new ArgumentException("UIFormName is invalid !");
            if (!uiFromBaseType.IsAssignableFrom(uiType))
                throw new NotImplementedException($"Type:{uiType} is not inherit form UIForm");
        }
        #endregion
    }
}
