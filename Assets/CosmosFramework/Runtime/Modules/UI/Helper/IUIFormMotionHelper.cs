using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.UI;
namespace Cosmos
{
    /// <summary>
    /// UIForm动效接口；
    /// 如tween动画等过渡可在此接口实现；
    /// </summary>
    public interface IUIFormMotionHelper
    {
        /// <summary>
        /// 激活；
        /// </summary>
        void ActiveUIForm(IUIForm uiForm);
        /// <summary>
        /// 失活；
        /// </summary>
        void DeactiveUIForm(IUIForm uiForm);
    }
}
