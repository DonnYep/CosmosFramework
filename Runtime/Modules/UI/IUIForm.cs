namespace Cosmos.UI
{
    public interface IUIForm
    {
        /// <summary>
        /// UI资产信息；
        /// </summary>
        UIAssetInfo UIAssetInfo { get; set; }
        /// <summary>
        /// 激活失火状态；
        /// </summary>
        bool Active{ get; set; }
        /// <summary>
        /// UI实体对象；
        /// </summary>
        object Handle { get; }
        /// <summary>
        /// UI优先级；
        /// 默认优先级为100，取值区间为[0,10000]；
        /// </summary>
        int Priority { get; }
        /// <summary>
        /// 当UI被初始化；
        /// </summary>
        void OnInit();
        /// <summary>
        /// 当UI被打开；
        /// </summary>
        void OnOpen();
        /// <summary>
        /// 当UI被关闭；
        /// </summary>
        void OnClose();
        /// <summary>
        /// 当UI被释放；
        /// </summary>
        void OnRelease();
    }
}
