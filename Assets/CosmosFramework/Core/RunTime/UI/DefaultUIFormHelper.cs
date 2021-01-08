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
        public void DestroyUIForm(UIFormBase uiForm)
        {
            MonoGameManager.KillObject(uiForm.gameObject);
        }
        public void HideUIForm(UIFormBase uiForm)
        {
            uiForm.gameObject.SetActive(false);
        }
        public void RemoveUIForm(UIFormBase uiForm)
        {
            MonoGameManager.KillObject(uiForm.gameObject);
        }
        public void ShowUIForm(UIFormBase uiForm)
        {
            uiForm.gameObject.SetActive(true);
        }
    }
}
