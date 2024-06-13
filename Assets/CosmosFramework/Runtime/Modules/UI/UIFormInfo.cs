using System;

namespace Cosmos.UI
{
    public struct UIFormInfo
    {
        UIAssetInfo uiAssetInfo;
        bool isOpened;
        int priority;
        Type uiType;
        public UIAssetInfo UIAssetInfo
        {
            get { return uiAssetInfo; }
        }
        public bool IsOpened
        {
            get { return isOpened; }
        }
        public int Priority
        {
            get { return priority; }
        }
        public Type UIType
        {
            get { return uiType;}
        }
        public readonly static UIFormInfo None = new UIFormInfo();
        public UIFormInfo(UIAssetInfo uiAssetInfo, bool isOpened, int priority, Type uiType)
        {
            this.uiAssetInfo = uiAssetInfo;
            this.isOpened = isOpened;
            this.priority = priority;
            this.uiType= uiType;
        }
    }
}
