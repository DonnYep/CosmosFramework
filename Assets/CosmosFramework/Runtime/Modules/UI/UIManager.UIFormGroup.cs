using System;
using System.Collections.Generic;
using System.Linq;
namespace Cosmos.UI
{
    internal sealed partial class UIManager
    {
        private class UIFormGroup : IUIFormGroup
        {
            public string UIGroupName { get; }
            Dictionary<string, IUIForm> uiFormDict;
            Type iuiFormType = typeof(IUIForm);
            public int UIFormCount { get { return uiFormDict.Count; } }
            public UIFormGroup(string uiGroupName)
            {
                UIGroupName = uiGroupName;
                uiFormDict = new Dictionary<string, IUIForm>();
            }
            public IUIForm[] GetAllUIForm()
            {
                return uiFormDict.Values.ToArray();
            }
            public IUIForm[] GetUIForms(Predicate<IUIForm> predicate)
            {
                var dst = new IUIForm[UIFormCount];
                int idx = 0;
                foreach (var uiForm in uiFormDict.Values)
                {
                    if (predicate.Invoke(uiForm))
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
            /// 添加UIForm到UI组重；
            /// 添加到UI组后，UI组会自动为UIForm的UIGroupName赋予当前组的名称，表示此UIForm属于这个UI组；
            /// </summary>
            /// <param name="uiForm">UIForm对象</param>
            /// <returns>是否添加成功</returns>
            public bool AddUIForm(IUIForm uiForm)
            {
                var result = uiFormDict.TryAdd(uiForm.UIFormName, uiForm);
                if (result)
                {
                    //这里是为了避免自定义UIForm并进行类型派生导致的UIGroupName属性无法写入的问题。因此通过反射自动获取第一个实现了IUIForm接口的类型；
                    var derivedType = Utility.Assembly.GetInterfaceHigestImplementedType(uiForm.GetType(), iuiFormType);
                    //通过传入合适的类型，对private set的属性进行赋值；
                    Utility.Assembly.SetPropertyValue(derivedType, uiForm, "UIGroupName", UIGroupName);
                }
                return result;
            }
            /// <summary>
            /// 从UI组重移除UIForm；
            /// 移除后，UI组会对UIForm进行清空UIGroupName的操作；
            /// </summary>
            /// <param name="uiForm">UIForm对象</param>
            /// <returns>是否移除成功</returns>
            public bool RemoveUIForm(IUIForm uiForm)
            {
                var result = uiFormDict.TryAdd(uiForm.UIFormName, uiForm);
                if (result)
                {
                    var derivedType = Utility.Assembly.GetInterfaceHigestImplementedType(uiForm.GetType(), iuiFormType);
                    Utility.Assembly.SetPropertyValue(derivedType, uiForm, "UIGroupName", string.Empty);
                }
                return result;
            }
        }
    }
}