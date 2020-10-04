using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    internal static partial class GameManager
    {
        /// <summary>
        /// 自定义模块容器；
        /// </summary>
        static Dictionary<Type, IModule> customeModuleDict = new Dictionary<Type, IModule>();
        /// <summary>
        /// 线程安全；
        /// 获取自定义模块；
        /// 需要从Module类派生;
        /// 此类模块不由CF框架生成，由用户自定义
        /// </summary>
        /// <typeparam name="TModule">实现模块功能的类对象</typeparam>
        /// <returns>获取的模块</returns>
        public static TModule CustomeModule<TModule>()
            where TModule : Module<TModule>, new()
        {
            Type type = typeof(TModule);
            IModule module = default;
            var result = customeModuleDict.TryGetValue(type, out module);
            if (result)
            {
                return module as TModule;
            }
            else
                return default(TModule);
        }
        /// <summary>
        /// 初始化自定义模块
        /// </summary>
        /// <param name="assembly">模块所在程序集</param>
        public static void InitCustomeModule(Assembly assembly)
        {
            Type[] types = assembly.GetTypes();
            var moduleType = typeof(IModule);
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].GetCustomAttribute<CustomeModuleAttribute>() != null && moduleType.IsAssignableFrom(types[i]))
                {
                    try
                    {
                        var module = Utility.Assembly.GetTypeInstance(types[i]) as IModule;
                        var result = customeModuleDict.TryAdd(types[i], module);
                        if (result)
                        {
                            module.OnInitialization();
                            Utility.Debug.LogInfo($"Custome Module :{module} is OnInitialization");
                            GameManager.RefreshHandler+= module.OnRefresh;
                        }
                    }
                    catch
                    {
                        Utility.Debug.LogError($"Custome module create instance fail:{types[i]} is not derived from the{ typeof(Cosmos.Module<>)}");
                    }
                }
            }
            PrepareCustomeModule();
        }
        static void PrepareCustomeModule()
        {
            foreach (var module in customeModuleDict.Values)
            {
                module.OnPreparatory();
            }
        }
    }
}
