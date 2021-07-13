using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.UI
{
    internal class UIFormGroup : IUIFormGroup
    {
        public string UIGroupName { get; }
        Dictionary<string, IUIForm> uiFormDict;
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
        public bool AddUIForm(IUIForm uiForm)
        {
            //TODO需要设置最高Type级别，即第一继承IUIForm的Type；
            var result = uiFormDict.TryAdd(uiForm.UIFormName, uiForm);
            if (result)
                Utility.Assembly.SetPropertyValue(uiForm.GetType().BaseType,uiForm, "UIGroupName", UIGroupName);
            return result;
        }
        public bool RemoveUIForm(IUIForm uiForm)
        {
            var result = uiFormDict.TryAdd(uiForm.UIFormName, uiForm);
            if (result)
                Utility.Assembly.SetPropertyValue(uiForm.GetType().BaseType,uiForm, "UIGroupName", string.Empty);
            return result;
        }
    }
}
