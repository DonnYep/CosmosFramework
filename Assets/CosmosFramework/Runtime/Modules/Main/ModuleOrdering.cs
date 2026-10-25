using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace Cosmos
{
    /// <summary>
    /// 模块排序与优先级校验；
    /// <para>内置模块与用户自定义模块共用同一个优先级序列：<see cref="ModuleAttribute.Priority"/> 越小越先初始化。</para>
    /// <para>同优先级时保持传入顺序（调用方先把内置模块放进去，就能保证"内置优先"的旧语义）。</para>
    /// </summary>
    internal static class ModuleOrdering
    {
        /// <summary>
        /// 读取模块优先级；未标注 <see cref="ModuleAttribute"/> 的按
        /// <see cref="ModuleAttribute.DefaultPriority"/> 处理。
        /// </summary>
        public static int GetPriority(Module module)
        {
            if (module == null)
                throw new ArgumentNullException("Module is invalid !");
            var attribute = module.GetType().GetCustomAttribute<ModuleAttribute>(false);
            return attribute != null ? attribute.Priority : ModuleAttribute.DefaultPriority;
        }
        /// <summary>
        /// 按优先级稳定排序（升序）；同优先级保持传入顺序。
        /// </summary>
        /// <param name="modules">待排序模块；内置模块应排在前面</param>
        public static List<Module> Sort(IEnumerable<Module> modules)
        {
            var list = new List<Module>();
            if (modules == null)
                return list;
            foreach (var module in modules)
            {
                if (module != null)
                    list.Add(module);
            }
            //OrderBy 是稳定排序：同优先级按原顺序，因此"先内置后用户"的传入顺序会被保留
            return list.OrderBy(GetPriority).ToList();
        }
        /// <summary>
        /// 校验内置模块的优先级声明：
        /// 必须标注 <see cref="ModuleAttribute"/>、必须小于用户默认优先级、且互不重复。
        /// <para>这两类错误都会让"用户模块默认排在最后"的约定失效，且编译期查不出来，因此启动时直接报错。</para>
        /// </summary>
        /// <param name="builtinModules">内置模块集合</param>
        public static void ValidateBuiltinPriorities(IEnumerable<Module> builtinModules)
        {
            if (builtinModules == null)
                return;
            var usedPriorities = new Dictionary<int, Type>();
            foreach (var module in builtinModules)
            {
                if (module == null)
                    continue;
                var type = module.GetType();
                var attribute = type.GetCustomAttribute<ModuleAttribute>(false);
                if (attribute == null)
                    throw new ArgumentException($"Built-in module : {type} must be marked with [Module].");
                if (attribute.Priority >= ModuleAttribute.DefaultPriority)
                    throw new ArgumentException($"Built-in module : {type} must declare a priority lower than {ModuleAttribute.DefaultPriority} (ModuleAttribute.DefaultPriority), otherwise user modules can not be ordered after it by default.");
                if (usedPriorities.TryGetValue(attribute.Priority, out var existingType))
                    throw new ArgumentException($"Built-in modules : {type} and {existingType} share the same priority {attribute.Priority}; built-in priorities must be unique.");
                usedPriorities.Add(attribute.Priority, type);
            }
        }
    }
}
