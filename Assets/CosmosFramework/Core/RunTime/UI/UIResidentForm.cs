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
            uiManager.ShowUI(this);
        }
        public sealed override void HideUIForm()
        {
            uiManager.HideUI(this);
        }
    }
}