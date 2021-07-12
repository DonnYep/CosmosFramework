using Cosmos.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 默认提供的ui动效Helper
    /// </summary>
    public class DefaultUIFormHelper : IUIFormMotionHelper
    {
        public void DeactiveUIForm(UIForm uiForm)
        {
            uiForm.gameObject.SetActive(false);
        }
        public void ActiveUIForm(UIForm uiForm)
        {
            uiForm.gameObject.SetActive(true);
        }
    }
}
