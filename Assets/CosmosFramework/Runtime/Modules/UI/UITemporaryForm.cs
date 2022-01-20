﻿namespace Cosmos.UI
{
    /// <summary>
    /// 临时UI
    /// </summary>
    public abstract class UITemporaryForm :UIFormBase
    {
        public sealed override void ShowUIForm(){}
        public sealed override void HideUIForm()
        {
            UIManager.RemoveUIForm(UIFormName);
        }
    }
}