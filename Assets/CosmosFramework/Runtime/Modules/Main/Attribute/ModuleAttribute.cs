using System;
namespace Cosmos
{
    /// <summary>
    /// 模块标记特性；
    /// <para>内置模块与用户自定义模块共用同一个优先级序列：<see cref="Priority"/> 越小越先初始化
    /// （也越先挂载生命周期与轮询回调），因此用户模块可以按需插到任意内置模块之前 / 之间 / 之后。</para>
    /// <para>内置模块占用 [100, 1300] 区间（见 <see cref="ModuleConstant"/>），彼此不重复；</para>
    /// <para>未显式声明优先级的模块取 <see cref="DefaultPriority"/>，即排在全部内置模块之后（与旧行为一致）。</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false,Inherited =false)]
    public class ModuleAttribute : Attribute
    {
        /// <summary>
        /// 默认优先级：排在全部内置模块之后；
        /// <para>用户模块若要插到内置模块之前，显式声明一个更小的值（例如 50、150）。</para>
        /// </summary>
        public const int DefaultPriority = 10000;
        /// <summary>
        /// 模块优先级；越小越先。
        /// </summary>
        public int Priority { get; set; }
        public ModuleAttribute()
        {
            Priority = DefaultPriority;
        }
    }
}