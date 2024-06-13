using System;

namespace Cosmos.UI
{
    internal sealed partial class UIManager
    {
        /// <summary>
        /// 记录UIForm状态；
        /// </summary>
        internal class UIFormState : IEquatable<UIFormState>
        {
            public IUIForm UIForm;
            public string UIFormName;
            public bool IsOpened;
            public int Priority;
            public UIFormState(IUIForm uiForm, string uiFormName, bool isOpened)
            {
                UIForm = uiForm;
                UIFormName = uiFormName;
                IsOpened = isOpened;
            }
            public bool Equals(UIFormState other)
            {
                return other.UIFormName == UIFormName;
            }
            public override bool Equals(object obj)
            {
                return Equals(obj as UIFormState);
            }
        }
    }
}
