﻿using Cosmos.UI;
namespace Cosmos
{
    /// <summary>
    /// UIForm帮助展示辅助接口；
    /// 如tween动画等过渡可在此接口实现；
    /// </summary>
    public interface IUIFormHelper
    {
        /// <summary>
        /// ui帮助接口展示ui对象；
        /// </summary>
        /// <param name="uiForm">传入的form</param>
        void ShowUIForm(UIFormBase uiForm);
        /// <summary>
        /// ui帮助接口隐藏ui对象；
        /// </summary>
        /// <param name="uiForm">传入的form</param>
        void HideUIForm(UIFormBase uiForm);
        /// <summary>
        /// ui帮助接口移除ui对象；
        /// </summary>
        /// <param name="uiForm">传入的form</param>
        void RemoveUIForm(UIFormBase uiForm);
        /// <summary>
        /// ui帮助接口销毁ui对象；
        /// </summary>
        /// <param name="uiForm">传入的form</param>
        void DestroyUIForm(UIFormBase uiForm);
    }
}
