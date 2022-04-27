using System;
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
        IUIForm[] GetUIForms(Predicate<IUIForm> condition);
        bool HasUIForm(string uiFormName);
        bool AddUIForm(IUIForm uiForm);
        bool RemoveUIForm(IUIForm uiForm);
    }
}
