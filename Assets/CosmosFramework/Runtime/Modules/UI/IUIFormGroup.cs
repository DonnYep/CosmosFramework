using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos.UI
{
    /// <summary>
    /// UI群组；
    /// </summary>
    public interface IUIFormGroup
    {
        string UIGroupName { get; }
        int UIFormCount { get; }
        IUIForm[] GetAllUIForm();
        IUIForm[] GetUIForms(Predicate<IUIForm> predicate);
        bool HasUIForm(string uiFormName);
        bool AddUIForm(IUIForm uiForm);
        bool RemoveUIForm(IUIForm uiForm);
    }
}
