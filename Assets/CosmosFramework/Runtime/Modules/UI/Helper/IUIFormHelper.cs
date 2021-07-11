using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.UI;
namespace Cosmos
{
    /// <summary>
    /// UIForm帮助展示辅助接口；
    /// 如tween动画等过渡可在此接口实现；
    /// </summary>
    public interface IUIFormHelper
    {
        void HideUIForm(UIForm uiForm);
        void ShowUIForm(UIForm uiForm);
    }
}
