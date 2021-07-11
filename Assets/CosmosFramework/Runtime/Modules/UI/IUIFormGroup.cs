using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos.UI
{
    /// <summary>
    /// UI群组
    /// </summary>
    public interface IUIFormGroup
    {
        string UIGroupName { get; }
        int UIFormCount { get; }
        void GetAllUIForm(ref IList<UIForm> result);
        void GetUIForms(Predicate<UIForm> predicate, ref IList<UIForm> result);
        bool HasUIForm(string uiName);
        bool AddUIForm(UIForm uiForm);
        bool RemoveUIForm(UIForm uiForm);
    }
}
