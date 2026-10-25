using Cosmos.Audio;
using Cosmos.Controller;
using Cosmos.DataTable;
using Cosmos.Download;
using Cosmos.Entity;
using Cosmos.Event;
using Cosmos.FSM;
using Cosmos.Input;
using Cosmos.Network;
using Cosmos.ObjectPool;
using Cosmos.Procedure;
using Cosmos.Resource;
using Cosmos.UI;
using Cosmos.WebRequest;

namespace Cosmos
{
    /// <summary>
    /// 内置模块注册表。
    /// <para>框架自带的模块在这里显式 new 出来：不再依赖"扫描 AppDomain + 反射实例化"，
    /// 因此内置模块的注册顺序是确定的、可读的，也不会有类型扫描的开销。</para>
    /// <para>顺序即初始化顺序：被其它模块在 OnInitialization 阶段依赖的模块排在前面
    /// （事件 → 资源 → 网络请求 → 其它）。</para>
    /// </summary>
    internal static class BuiltinModuleRegistry
    {
        /// <summary>
        /// 创建全部内置模块（每次调用都返回新实例）。
        /// </summary>
        /// <returns>内置模块数组</returns>
        internal static Module[] CreateModules()
        {
            return new Module[]
            {
                new EventManager(),
                new ResourceManager(),
                new WebRequestManager(),
                new DataTableManager(),
                new ObjectPoolManager(),
                new EntityManager(),
                new UIManager(),
                new AudioManager(),
                new InputManager(),
                new ControllerManager(),
                new FSMManager(),
                new ProcedureManager(),
                new NetworkManager(),
                new DownloadManager(),
            };
        }
    }
}
