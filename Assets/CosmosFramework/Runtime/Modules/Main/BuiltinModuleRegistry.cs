using Cosmos.Audio;
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
    /// 因此内置模块的集合是确定的、可读的，也不会有类型扫描的开销。</para>
    /// <para>数组只决定"有哪些内置模块"；初始化顺序不再由数组顺序决定，而是与用户模块一起
    /// 按 <see cref="ModuleAttribute.Priority"/> 统一排序（内置模块占 100~1300，见 <see cref="ModuleConstant"/>；
    /// 用户模块默认 <see cref="ModuleAttribute.DefaultPriority"/> = 10000，即排在全部内置模块之后）。</para>
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
                new FSMManager(),
                new ProcedureManager(),
                new NetworkManager(),
                new DownloadManager(),
            };
        }
    }
}
