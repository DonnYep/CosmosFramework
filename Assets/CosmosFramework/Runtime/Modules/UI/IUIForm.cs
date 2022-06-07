namespace Cosmos.UI
{
    public interface IUIForm
    {
        UIAssetInfo UIAssetInfo { get; set; }
        /// <summary>
        /// UI实体对象；
        /// </summary>
        object Handle { get; }
        /// <summary>
        /// UI优先级；
        /// </summary>
        int Priority { get; }
        /// <summary>
        /// 当UI被激活；
        /// </summary>
        void OnActive();
        /// <summary>
        /// 当UI被失活；
        /// </summary>
        void OnDeactive();
        /// <summary>
        /// 当UI被关闭释放；
        /// </summary>
        void OnClose();
    }
}
