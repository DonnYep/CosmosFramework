namespace Cosmos.Input
{
    /// <summary>
    /// 输入帮助体：把某个具体输入后端（旧 Input Manager / 新 Input System / 插件）的原始状态
    /// 翻译成本模块的虚拟按键与轴线。
    /// <para>约定：</para>
    /// <para>1、<see cref="OnInitialization"/> 里注册自己用到的虚拟按键/轴线，<see cref="OnTermination"/> 里注销；</para>
    /// <para>2、<see cref="OnRefresh"/> 每帧调用一次，<b>只推送电平状态</b>（按下=SetButtonPressed(true)），
    /// 按下/抬起的边沿由模块根据电平变化自行推导；</para>
    /// <para>3、不再从 CosmosEntry 取管理器（构造时机不确定会拿到 null），由框架在初始化时注入。</para>
    /// </summary>
    public interface IInputHelper
    {
        /// <summary>
        /// 设备启动；在此注册虚拟按键与轴线
        /// </summary>
        /// <param name="inputManager">输入模块（由框架注入，勿缓存全局静态引用）</param>
        void OnInitialization(IInputManager inputManager);
        /// <summary>
        /// 设备运行：每帧推送一次电平状态
        /// </summary>
        void OnRefresh();
        /// <summary>
        /// 设备关闭；在此注销虚拟按键与轴线
        /// </summary>
        void OnTermination();
    }
}
