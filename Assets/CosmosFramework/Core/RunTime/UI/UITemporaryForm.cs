using UnityEngine;
using System.Collections;
namespace Cosmos.UI
{
    /// <summary>
    /// 临时UI
    /// </summary>
    public abstract class UITemporaryForm :UIFormBase
    {
        public sealed override void ShowUIForm(){}
        public sealed override void HideUIForm()
        {
            uiManager.RemoveUI(UIFormName);
        }
    }
}