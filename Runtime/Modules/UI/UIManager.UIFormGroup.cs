using System;
using System.Collections.Generic;
using System.Linq;
namespace Cosmos.UI
{
    internal sealed partial class UIManager
    {
        private class UIFormGroup : IUIFormGroup
        {
            static Queue<UIFormGroup> groupQueue = new Queue<UIFormGroup>();
            public static UIFormGroup Acquire(string uiGroupName)
            {
                UIFormGroup group = null;
                if (groupQueue.Count > 0)
                {
                    groupQueue.Dequeue();
                }
                else
                {
                    group = new UIFormGroup();
                }
                group.UIGroupName = uiGroupName;
                return group;
            }
            public static void Release(UIFormGroup uiFormGroup)
            {
                if (uiFormGroup != null)
                {
                    uiFormGroup.Clear();
                    groupQueue.Enqueue(uiFormGroup);
                }
            }
            public string UIGroupName { get; private set; }
            Dictionary<string, IUIForm> uiFormDict;
            public int UIFormCount { get { return uiFormDict.Count; } }
            public UIFormGroup()
            {
                uiFormDict = new Dictionary<string, IUIForm>();
            }
            public IUIForm[] GetAllUIForm()
            {
                return uiFormDict.Values.ToArray();
            }
            public IUIForm[] GetUIForms(Predicate<IUIForm> condition)
            {
                var dst = new IUIForm[UIFormCount];
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
            public bool HasUIForm(string uiFormName)
            {
                return uiFormDict.ContainsKey(uiFormName);
            }
            /// <summary>
            /// 添加UIForm到UI组；
            /// </summary>
            /// <param name="uiForm">UIForm对象</param>
            /// <returns>是否添加成功</returns>
            public bool AddUIForm(IUIForm uiForm)
            {
                var result = uiFormDict.TryAdd(uiForm.UIAssetInfo.UIFormName, uiForm);
                return result;
            }
            /// <summary>
            /// 从UI组重移除UIForm；
            /// </summary>
            /// <param name="uiForm">UIForm对象</param>
            /// <returns>是否移除成功</returns>
            public bool RemoveUIForm(IUIForm uiForm)
            {
                var result = uiFormDict.TryAdd(uiForm.UIAssetInfo.UIFormName, uiForm);
                return result;
            }
            public void Clear()
            {
                UIGroupName = string.Empty;
                uiFormDict.Clear();
            }
        }
    }
}