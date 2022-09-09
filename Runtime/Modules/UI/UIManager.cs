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
        /// 加载完成后所存储的字典；
        /// </summary>
        Dictionary<string, UIFormInfo> uiFormInfoLoadedDict;
        /// <summary>
        /// 加载中名单；
        /// </summary>
        HashSet<string> loadingUIForms;
        /// <summary>
        /// 将被释放的名单；
        /// </summary>
        HashSet<string> uiFormsToRelease;
        /// <summary>
        /// 将被关闭的名单；
        /// </summary>
        HashSet<string> uiFormsToClose;
        /// <summary>
        /// UIFormInfo缓存；
        /// </summary>
        List<UIFormInfo> uiFormInfoCache;
        /// <summary>
        /// 激活的UI链表；
        /// </summary>
        LinkedList<UIFormInfo> activeUILnk;
        /// <summary>
        /// 失活的UI链表；
        /// </summary>
        LinkedList<UIFormInfo> deactiveUILnk;
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
            var uiFormName = assetInfo.UIFormName;
            if (uiFormInfoLoadedDict.TryGetValue(uiFormName, out var uiInfo))
            {
                uiInfo.UIForm.Active = true;
                return null;
            }
            else
            {
                var canLoad = loadingUIForms.Add(uiFormName);
                if (!canLoad)//处于加载名单中
                {
                    if (uiFormsToRelease.Contains(uiFormName))
                    {
                        //若在释放名单中，则从释放名单中移除
                        uiFormsToRelease.Remove(uiFormName);
                    }
                    else if (uiFormsToClose.Contains(uiFormName))
                    {
                        //若在关闭名单中，则从关闭名单中移除
                        uiFormsToClose.Remove(uiFormName);
                    }
                    return null;
                }
                else
                {
                    return uiFormAssetHelper.InstanceUIFormAsync(assetInfo, uiType, uiForm =>
                    {
                        OnUIFormLoad(assetInfo, uiForm, callback);
                    });
                }
            }
        }
        /// <inheritdoc/>
        public Coroutine OpenUIFormAsync<T>(UIAssetInfo assetInfo, Action<T> callback = null)
            where T : class, IUIForm
        {
            var type = typeof(T);
            CheckUIAssetInfoValid(assetInfo, type);
            if (uiFormInfoLoadedDict.TryGetValue(assetInfo.UIFormName, out var uiInfo))
            {
                uiInfo.UIForm.Active = true;
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
            Utility.Text.IsStringValid(uiFormName, "UIFormName is invalid !");
            uiFormInfoLoadedDict.TryGetValue(uiFormName, out var uiForm);
            ReleaseUIForm(uiForm.UIForm);
        }
        /// <inheritdoc/>
        public void ReleaseUIForm(IUIForm uiForm)
        {
            if (uiForm == null)
                throw new ArgumentNullException("UIForm is invalid.");
            var uiFormName = uiForm.UIAssetInfo.UIFormName;
            Utility.Text.IsStringValid(uiFormName, "UIFormName is invalid !");
            if (uiFormInfoLoadedDict.Remove(uiFormName, out var uiFormInfo))
            {
                //处理已经加载完成的UIForm
                if (uiFormInfo.UIForm != uiForm)
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
                if (uiFormInfo.IsOpened)
                    uiForm.OnClose();
                uiForm.OnRelease();
                uiFormAssetHelper.ReleaseUIForm(uiForm);

                uiFormInfoCache.Remove(uiFormInfo);
            }
            else
            {
                //处理未加载完成的UIForm
                if (loadingUIForms.Contains(uiFormName))//处于加载名单中时，表示当前资源正在加载。
                {
                    //将名称添加到释放名单中，当加载结束后在释放名单中查询到相同名称，则直接释放。
                    uiFormsToRelease.Add(uiFormName);
                    uiFormsToClose.Remove(uiFormName);
                }
            }
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
                    uiFormInfoLoadedDict.Remove(uiForm.UIAssetInfo.UIFormName, out var uiFormInfo);
                    if (uiFormInfo.IsOpened)
                        uiForm.OnClose();
                    uiForm.OnRelease();
                    uiFormAssetHelper.ReleaseUIForm(uiForms[i]);
                    uiFormInfoCache.Remove(uiFormInfo);
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
            return uiFormInfoLoadedDict.ContainsKey(uiFormName);
        }
        /// <inheritdoc/>
        public bool PeekUIForm<T>(string uiFormName, out T uiForm)
            where T : class, IUIForm
        {
            Utility.Text.IsStringValid(uiFormName, "UIFormName is invalid !");
            var rst = uiFormInfoLoadedDict.TryGetValue(uiFormName, out var form);
            uiForm = form as T;
            return rst;
        }
        /// <inheritdoc/>
        public bool PeekUIForm(string uiFormName, out IUIForm uiForm)
        {
            Utility.Text.IsStringValid(uiFormName, "UIFormName is invalid !");
            uiForm = default;
            if (uiFormInfoLoadedDict.TryGetValue(uiFormName, out var uiFormInfo))
            {
                uiForm = uiFormInfo.UIForm;
                return true;
            }
            return false;
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
            var dst = new IUIForm[uiFormInfoLoadedDict.Count];
            int idx = 0;
            foreach (var uiFormInfo in uiFormInfoLoadedDict.Values)
            {
                var uiForm = uiFormInfo.UIForm;
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
            Utility.Text.IsStringValid(uiFormName, "UIFormName is invalid !");
            Utility.Text.IsStringValid(uiGroupName, "UIGroupName is invalid !");
            if (!uiGroupDict.TryGetValue(uiGroupName, out var group))
            {
                group = UIFormGroup.Acquire(uiGroupName);
                uiGroupDict.Add(uiGroupName, group);
            }
            var uiFormInfo = uiFormInfoLoadedDict[uiFormName];
            var uiForm = uiFormInfo.UIForm;
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
            Utility.Text.IsStringValid(uiFormName, "UIFormName is invalid !");
            var uiFormInfo = uiFormInfoLoadedDict[uiFormName];
            var uiForm = uiFormInfo.UIForm;
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
            uiFormInfoLoadedDict = new Dictionary<string, UIFormInfo>();
            uiFormInfoCache = new List<UIFormInfo>();
            loadingUIForms = new HashSet<string>();
            uiFormsToRelease = new HashSet<string>();
            uiFormsToClose = new HashSet<string>();
            activeUILnk = new LinkedList<UIFormInfo>();
            deactiveUILnk = new LinkedList<UIFormInfo>();
        }
        [TickRefresh]
        void TickRefresh()
        {
            var length = uiFormInfoCache.Count;
            for (int i = 0; i < length; i++)
            {
                var uiFormInfo = uiFormInfoCache[i];
                var uiForm = uiFormInfo.UIForm;
                if (uiForm.Active != uiFormInfo.IsOpened)
                {
                    if (uiForm.Active)
                    {
                        uiFormInfo.IsOpened = true;
                        uiForm.OnOpen();
                        OnUIFormActive(uiFormInfo);
                    }
                    else
                    {
                        uiFormInfo.IsOpened = false;
                        uiForm.OnClose();
                        OnUIFormDeactive(uiFormInfo);
                    }
                }
                if (uiForm.Order != uiFormInfo.Order)
                {
                    uiFormInfo.Order = uiForm.Order;
                    OnUIFormOrderChange(uiFormInfo);
                }
            }
        }
        void CheckUIAssetInfoValid(UIAssetInfo assetInfo, Type uiType)
        {
            if (string.IsNullOrEmpty(assetInfo.UIFormName))
                throw new ArgumentException("UIFormName is invalid !");
            if (!uiFromBaseType.IsAssignableFrom(uiType))
                throw new NotImplementedException($"Type:{uiType} is not inherit form UIForm");
        }
        void OnUIFormLoad(UIAssetInfo assetInfo, IUIForm uiForm, Action<IUIForm> callback)
        {
            var uiFormName = assetInfo.UIFormName;
            loadingUIForms.Remove(uiFormName);//由于加载完成，因此从加载名单中移除。
            if (uiFormsToRelease.Contains(uiFormName))
            {
                //若在释放名单中，不做初始化，不调用任何生命周期，直接释放。
                uiFormAssetHelper.ReleaseUIForm(uiForm);
                return;
            }
            uiForm.UIAssetInfo = assetInfo;
            if (uiFormsToClose.Contains(uiFormName))
            {
                uiForm.Active = false;//若在关闭名单中，设置失活
                uiFormsToClose.Remove(uiFormName);//移除关闭名单中的名字
            }
            else
                uiForm.Active = true;//若不在关闭名单中，设置激活
            var uiFormInfo = new UIFormInfo(uiForm, assetInfo.UIFormName, false);
            uiFormInfoLoadedDict.Add(assetInfo.UIFormName, uiFormInfo);
            uiForm.OnInit();//调用初始化生命周期
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
            //加载完毕并执行完生命周期后，添加到缓存中。
            if (!uiFormInfoCache.Contains(uiFormInfo))
                uiFormInfoCache.Add(uiFormInfo);
        }
        void OnUIFormActive(UIFormInfo uiFormInfo)
        {
            uiFormInfo.Order = uiFormInfo.UIForm.Order;
            var current = activeUILnk.First;
            var added = false;
            while (current != null && current.Value != null)
            {
                if (uiFormInfo.Order < current.Value.Order)
                {
                    //若激活的uiFormInfo.Order<当前节点uiFormInfo.Order，则插入当前节点之前
                    //Order示例：0<1(current.Previous)=(1 插入到current.Previous之前)<2(current)
                    activeUILnk.AddBefore(current, uiFormInfo);
                    added = true;
                    break;
                }
                current = current.Next;
            }
            if (!added)
            {
                activeUILnk.AddLast(uiFormInfo);
            }
            deactiveUILnk.Remove(uiFormInfo);
            var index = 0;
            foreach (var info in activeUILnk)
            {
                info.UIForm.OnOrderChange(index);
                index++;
            }
        }
        void OnUIFormDeactive(UIFormInfo uiFormInfo)
        {
            activeUILnk.Remove(uiFormInfo);
            deactiveUILnk.AddLast(uiFormInfo);
        }
        void OnUIFormOrderChange(UIFormInfo uiFormInfo)
        {
            if (!uiFormInfo.IsOpened)//若为非激活状态，则返回
                return;
            //若为激活状态，则移除队列，并重新添加到链表中；
            activeUILnk.Remove(uiFormInfo);
            var current = activeUILnk.First;
            var added = false;
            while (current != null && current.Value != null)
            {
                if (uiFormInfo.Order < current.Value.Order)
                {
                    //若激活的uiFormInfo.Order<当前节点uiFormInfo.Order，则插入当前节点之前
                    //Order示例：0<1(current.Previous)=(1 插入到current.Previous之前)<2(current)
                    activeUILnk.AddBefore(current, uiFormInfo);
                    added = true;
                    break;
                }
                current = current.Next;
            }
            if (!added)
            {
                activeUILnk.AddLast(uiFormInfo);
            }
        }
        #endregion
    }
}
