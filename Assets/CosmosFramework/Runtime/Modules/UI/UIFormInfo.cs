using System;

namespace Cosmos.UI
{
    public struct UIFormInfo
    {
        UIAssetInfo uiAssetInfo;
        bool isOpened;
        int order;
        Type uiType;
        public UIAssetInfo UIAssetInfo
        {
            get { return uiAssetInfo; }
        }
        public bool IsOpened
        {
            get { return isOpened; }
        }
        public int Order
        {
            get { return order; }
        }
        public Type UIType
        {
            get { return uiType;}
        }
        public readonly static UIFormInfo None = new UIFormInfo();
        public UIFormInfo(UIAssetInfo uiAssetInfo, bool isOpened, int order,Type uiType)
        {
            this.uiAssetInfo = uiAssetInfo;
            this.isOpened = isOpened;
            this.order = order;
            this.uiType= uiType;
        }
    }
}
