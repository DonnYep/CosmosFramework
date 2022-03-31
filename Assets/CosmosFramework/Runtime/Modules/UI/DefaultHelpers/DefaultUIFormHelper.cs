using Cosmos.UI;
using UnityEngine;
namespace Cosmos
{
    /// <summary>
    /// 默认提供的ui动效Helper
    /// </summary>
    public class DefaultUIFormHelper : IUIFormMotionHelper
    {
        public void DeactiveUIForm(IUIForm uiForm)
        {
            uiForm.Handle.CastTo<GameObject>().SetActive(false);
        }
        public void ActiveUIForm(IUIForm uiForm)
        {
            uiForm.Handle.CastTo<GameObject>().SetActive(true);
        }
    }
}
