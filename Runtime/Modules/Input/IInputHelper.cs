namespace Cosmos.Input
{
    public interface IInputHelper
    {
        /// <summary>
        /// 设备启动
        /// </summary>
        void OnInitialization();
        /// <summary>
        /// 设备运行
        /// </summary>
        void OnRefresh();
        /// <summary>
        /// 设备关闭
        /// </summary>
        void OnTermination();
    }
}