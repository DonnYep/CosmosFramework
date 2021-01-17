using UnityEngine;
using System.Collections;
namespace Cosmos.UI
{
    /// <summary>
    /// 非常驻UI
    /// </summary>
    public abstract class UILogicTemporary :UILogicBase
    {
        public override void HidePanel()
        {
            GameManagerAgent.KillObject(gameObject);
        }
    }
}