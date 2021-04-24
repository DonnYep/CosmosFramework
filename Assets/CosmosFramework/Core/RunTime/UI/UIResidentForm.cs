using UnityEngine;
using System.Collections;

namespace Cosmos.UI
{
    /// <summary>
    /// 常驻UI
    /// </summary>
    public abstract class UIResidentForm : UIFormBase
    {
        public sealed override void ShowUIForm()
        {
            UIManager.ShowUIForm(UIFormName);
        }
        public sealed override void HideUIForm()
        {
            UIManager.HideUIForm(UIFormName);
        }
    }
}