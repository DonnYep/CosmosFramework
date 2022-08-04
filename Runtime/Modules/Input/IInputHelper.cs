namespace Cosmos.Input
{
    public interface IInputHelper
    {
        /// <summary>
        /// 设备启动
        /// </summary>
        void OnStart();
        /// <summary>
        /// 设备运行
        /// </summary>
        void OnRun();
        /// <summary>
        /// 设备关闭
        /// </summary>
        void OnShutdown();
    }
}