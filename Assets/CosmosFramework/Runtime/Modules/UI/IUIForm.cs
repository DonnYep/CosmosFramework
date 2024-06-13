namespace Cosmos.UI
{
    public interface IUIForm
    {
        /// <summary>
        /// UI资产信息；
        /// </summary>
        UIAssetInfo UIAssetInfo { get; set; }
        /// <summary>
        /// 活跃状态；
        /// </summary>
        bool Active { get; set; }
        /// <summary>
        /// UI实体对象；
        /// </summary>
        object Handle { get; }
        /// <summary>
        /// UI渲染优先级；
        /// </summary>
        int Priority { get; set; }
        /// <summary>
        /// 当UI被初始化；
        /// </summary>
        void OnInit();
        /// <summary>
        /// 当UI被打开；
        /// </summary>
        void OnOpen();
        /// <summary>
        /// 顺序更改，当且仅当激活状态下会被触发；
        /// </summary>
        /// <param name="index">当前顺序id</param>
        void OnPriorityChange(int index);
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
