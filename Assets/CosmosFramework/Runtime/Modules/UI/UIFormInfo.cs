namespace Cosmos.UI
{
    public struct UIFormInfo
    {
        UIAssetInfo uiAssetInfo;
        bool isOpened;
        int order;
        public UIAssetInfo UIAssetInfo
        {
            get { return uiAssetInfo; }
        }
        public bool IsOpened { get { return isOpened; } }
        public int Order
        {
            get { return order; }
        }
        public UIFormInfo(UIAssetInfo uiAssetInfo, bool isOpened, int order)
        {
            this.uiAssetInfo = uiAssetInfo;
            this.isOpened = isOpened;
            this.order = order;
        }
    }
}
