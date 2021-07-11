using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.UI
{
    internal class UIFormGroup// : IUIFormGroup
    {
        public string UIGroupName { get; }
        Dictionary<string, UIForm> uiFormDict;
        public int UIFormCount { get { return uiFormDict.Count; } }
        public UIFormGroup(string uiGroupName)
        {
            UIGroupName = uiGroupName;
            uiFormDict = new Dictionary<string, UIForm>();
        }
        public UIForm[] GetAllUIForm()
        {
            return uiFormDict.Values.ToArray();
        }
        public UIForm[] GetUIForms(Predicate<UIForm> predicate)
        {
            UIForm[] src = new UIForm[UIFormCount];
            int idx = 0;
            foreach (var uiForm in uiFormDict.Values)
            {
                if (predicate.Invoke(uiForm))
                {
                    src[idx] = uiForm;
                    idx++;
                }
            }
            UIForm[] dst = new UIForm[idx];
            Array.Copy(src, dst, dst.Length);
            return dst;
        }
        public bool HasUIForm(string uiFormName)
        {
            return uiFormDict.ContainsKey(uiFormName);
        }
        public bool AddUIForm(UIForm uiForm)
        {
            return uiFormDict.TryAdd(uiForm.UIFormName, uiForm);
        }
        public bool RemoveUIForm(UIForm uiForm)
        {
            return uiFormDict.TryAdd(uiForm.UIFormName, uiForm);
        }
    }
}
