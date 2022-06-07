using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cosmos.UI
{
    //================================================
    /*
     * 1、UI模块；由于UI模块没有强约束类型，因此是可以添加实现了IUIForm
     *  接口的自定义对象。
     */
    //================================================
    [Module]
    internal sealed partial class UIManager : Module, IUIManager
    {
        #region Properties
        /// <inheritdoc/>
        public Transform UIRoot { get; private set; }
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
        public void SetUIRoot(Transform uiRoot, bool destroyOldOne = false)
        {
            if (destroyOldOne)
                GameObject.Destroy(UIRoot.gameObject);
            var inct = this.Instance();
            uiRoot.SetParent(inct.transform);
            UIRoot = uiRoot;
        }
        /// <inheritdoc/>
        public void SetUIFormAssetHelper(IUIFormAssetHelper helper)
        {
            this.uiFormAssetHelper = helper;
        }
        /// <inheritdoc/>
        public Coroutine OpenUIFormAsync(UIAssetInfo assetInfo, Type uiType, Action<IUIForm> callback = null)
        {
            CheckUIAssetInfoValid(assetInfo, uiType);
            if (HasUIForm(assetInfo.UIFormName))
            {
                ActiveUIForm(assetInfo.UIFormName);
                return null;
            }
            else
                return uiFormAssetHelper.InstanceUIFormAsync(assetInfo, uiType, uiForm =>
                {
                    uiFormAssetHelper.AttachTo(uiForm, UIRoot);
                    uiForm.UIAssetInfo = assetInfo;
                    uiFormDict.Add(assetInfo.UIFormName, uiForm);
                    if (!string.IsNullOrEmpty(assetInfo.UIGroupName))
                    {
                        if (!uiGroupDict.TryGetValue(assetInfo.UIGroupName, out var group))
                        {
                            group = UIFormGroup.Acquire(assetInfo.UIGroupName);
                            uiGroupDict.Add(assetInfo.UIGroupName, group);
                        }
                        group.AddUIForm(uiForm);
                    }
                    callback?.Invoke(uiForm);
                });
        }
        /// <inheritdoc/>
        public Coroutine OpenUIFormAsync<T>(UIAssetInfo assetInfo, Action<T> callback = null)
            where T : class, IUIForm
        {
            var type = typeof(T);
            CheckUIAssetInfoValid(assetInfo, type);
            if (HasUIForm(assetInfo.UIFormName))
            {
                ActiveUIForm(assetInfo.UIFormName);
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
        public void CloseUIForm(string uiFormName)
        {
            CheckUIFormNameValid(uiFormName);
            uiFormDict.Remove(uiFormName, out var uiForm);
            if (!string.IsNullOrEmpty(uiForm.UIAssetInfo.UIGroupName))
            {
                uiGroupDict.TryGetValue(uiForm.UIAssetInfo.UIGroupName, out var group);
                group?.RemoveUIForm(uiForm);
                if (group.UIFormCount <= 0)
                {
                    uiGroupDict.Remove(uiForm.UIAssetInfo.UIGroupName, out var uiFormGroup);
                    UIFormGroup.Release(uiFormGroup as UIFormGroup);
                }
            }
            uiForm.OnClose();
            uiFormAssetHelper.CloseUIForm(uiForm);
        }
        /// <inheritdoc/>
        public void CloseUIForm(IUIForm uiForm)
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
                uiForm.OnClose();
                uiFormAssetHelper.CloseUIForm(uiForm);
            }
            else
                throw new ArgumentNullException($"UI  { uiFormName} is not existed or registered !");
        }
        /// <inheritdoc/>
        public void DeactiveUIForm(string uiFormName)
        {
            CheckUIFormNameValid(uiFormName);
            if (HasUIForm(uiFormName))
            {
                uiFormDict.TryGetValue(uiFormName, out var uiForm);
                uiForm.OnDeactive();
            }
        }
        /// <inheritdoc/>
        public void DeactiveUIForm(IUIForm uiForm)
        {
            if (uiForm == null)
                throw new ArgumentNullException("UIForm is invalid.");
            var uiFormName = uiForm.UIAssetInfo.UIFormName;
            Utility.Text.IsStringValid(uiFormName, "UIFormName is invalid !");
            if (HasUIForm(uiFormName))
            {
                uiFormDict.TryGetValue(uiFormName, out var srcUIForm);
                if (srcUIForm == uiForm)
                    uiForm.OnDeactive();
                else
                    throw new ArgumentException($"{uiFormName}'s ptr is not equal !");//指针不一致
            }
        }
        /// <inheritdoc/>
        public void ActiveUIForm(IUIForm uiForm)
        {
            if (uiForm == null)
                throw new ArgumentNullException("UIForm is invalid.");
            var uiFormName = uiForm.UIAssetInfo.UIFormName;
            Utility.Text.IsStringValid(uiFormName, "UIFormName is invalid !");
            if (HasUIForm(uiFormName))
            {
                uiFormDict.TryGetValue(uiFormName, out var srcUIForm);
                if (srcUIForm == uiForm)
                    uiForm.OnActive();
                else
                    throw new ArgumentException($"{uiFormName}'s ptr is not equal !");//指针不一致
            }
        }
        /// <inheritdoc/>
        public void ActiveUIForm(string uiFormName)
        {
            CheckUIFormNameValid(uiFormName);
            if (HasUIForm(uiFormName))
            {
                uiFormDict.TryGetValue(uiFormName, out var uiForm);
                uiForm.OnActive();
            }
        }
        /// <inheritdoc/>
        public void CloseUIGroup(string uiGroupName)
        {
            if (uiGroupDict.TryGetValue(uiGroupName, out var group))
            {
                var uiForms = group.GetAllUIForm();
                var length = uiForms.Length;
                for (int i = 0; i < length; i++)
                {
                    uiForms[i].OnClose();
                    uiFormAssetHelper.CloseUIForm(uiForms[i]);
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
                    uiForms[i].OnDeactive();
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
                    uiForms[i].OnActive();
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
            uiFormAssetHelper = new DefaultUIFormAssetHelper();
        }
        /// <summary>
        /// 检测UIForm的名字是否有效，且是否存在；
        /// </summary>
        /// <param name="uiFormName">UI资源的名称</param>
        void CheckUIFormNameValid(string uiFormName)
        {
            Utility.Text.IsStringValid(uiFormName, "UIFormName is invalid !");
            if (!uiFormDict.ContainsKey(uiFormName))
                throw new ArgumentNullException($"UI  { uiFormName} is not existed or registered !");
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
