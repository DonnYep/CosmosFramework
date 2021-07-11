using Cosmos.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 默认提供的Helper
    /// </summary>
    public class DefaultUIFormHelper : IUIFormHelper
    {
        public void CloseUIForm(UIForm uiForm)
        {
            MonoGameManager.KillObject(uiForm.gameObject);
        }
        public void HideUIForm(UIForm uiForm)
        {
            uiForm.gameObject.SetActive(false);
        }
        public void ShowUIForm(UIForm uiForm)
        {
            uiForm.gameObject.SetActive(true);
        }
    }
}
