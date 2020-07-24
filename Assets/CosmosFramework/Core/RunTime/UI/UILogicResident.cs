using UnityEngine;
using System.Collections;

namespace Cosmos.UI
{
    /// <summary>
    /// 常驻UI
    /// </summary>
    public abstract class UILogicResident : UILogicBase
    {
        public override void ShowPanel()
        {
            SetPanelActive(true);
        }
        public override void HidePanel()
        {
            SetPanelActive(false);
        }
    }
}