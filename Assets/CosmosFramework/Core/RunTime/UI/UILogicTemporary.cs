using UnityEngine;
using System.Collections;
namespace Cosmos.UI
{
    /// <summary>
    /// 临时UI
    /// </summary>
    public abstract class UILogicTemporary :UILogicBase
    {
        public override void HidePanel()
        {
            uiManager.RemovePanel(UILogicName);
        }
    }
}