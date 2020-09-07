using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    internal sealed partial class GameManager
    {
        /// <summary>
        /// 外源模块；
        /// 凡是实现Module的非框架拥有的管理类。享有同等的轮询、初始化等级；
        /// </summary>
        public sealed class External
        {
            /// <summary>
            /// 外源模块容器；
            /// </summary>
          static   Dictionary<Type, IModule> moduleDict = new Dictionary<Type, IModule>();
            /// <summary>
            /// 获取外源模块；
            /// 此类模块不由CF框架生成，由用户自定义
            /// 需要从Module类派生;
            /// 线程安全；
            /// </summary>
            /// <typeparam name="TModule">实现模块功能的类对象</typeparam>
            /// <returns>获取的模块</returns>
            public static TModule GetModule<TModule>()
                where TModule : Module<TModule>, new()
            {
                Type type = typeof(TModule);
                IModule module = default;
                var result = moduleDict.TryGetValue(type, out module);
                if (!result)
                {
                    module = new TModule();
                    moduleDict.Add(type, module);
                    module.OnInitialization();
                    Utility.Debug.LogInfo($"生成新模块 , Module :{module.ToString()} ");
                    GameManager.Instance.refreshHandler += module.OnRefresh;
                }
                return module as TModule;
            }
            /// <summary>
            /// 清理外源模块；
            /// 此类模块不由CF框架生成，由用户自定义
            /// 需要从Module类派生;
            /// </summary>
            /// <typeparam name="TModule"></typeparam>
            public static void RemoveModule<TModule>()
        where TModule : Module<TModule>, new()
            {
                Type type = typeof(TModule);
                if (moduleDict.ContainsKey(type))
                {
                    var module = moduleDict[type];
                    try
                    {
                        GameManager.Instance.refreshHandler -= module.OnRefresh;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    module.OnTermination();
                    moduleDict.Remove(type);
                }
            }
            public static bool HasModule<TModule>()
        where TModule : Module<TModule>, new()
            {
                Type type = typeof(TModule);
                return moduleDict.ContainsKey(type);
            }
            public static void ClearAllModule()
            {
                moduleDict.Clear();
            }
        }
    }
}
