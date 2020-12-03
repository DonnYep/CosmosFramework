using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cosmos
{
    public static partial class GameManager
    {
        /// <summary>
        /// 自定义模块容器；
        /// </summary>
        static Dictionary<Type, Module> externalModuleDict = new Dictionary<Type, Module>();
        static Dictionary<Type, Type> externalInterfaceModuleDict = new Dictionary<Type, Type>();
        static Dictionary<Type, GameObject> externalModuleMountDict = new Dictionary<Type, GameObject>();
        /// <summary>
        /// 线程安全；
        /// 获取自定义模块；
        /// 需要从Module类派生;
        /// 此类模块不由CF框架生成，由用户自定义
        /// </summary>
        /// <typeparam name="TModule">实现模块功能的类对象</typeparam>
        /// <returns>获取的模块</returns>
        public static T ExternalModule<T>()
            where T : class, IModuleManager
        {
            Type interfaceType = typeof(T);
            var hasType = externalInterfaceModuleDict.TryGetValue(interfaceType, out var derivedType);
            if (!hasType)
            {
                foreach (var m in moduleDict)
                {
                    if (interfaceType.IsAssignableFrom(m.Key))
                    {
                        derivedType = m.Key;
                        externalInterfaceModuleDict.TryAdd(interfaceType, derivedType);
                        break;
                    }
                }
            }
            externalModuleDict.TryGetValue(derivedType, out var module);
            return module as T;
        }
        /// <summary>
        /// 扩展模块挂载对象；
        /// </summary>
        /// <typeparam name="T">模块interface</typeparam>
        /// <returns>挂载对象</returns>
        public static GameObject GetExternalModuleMount<T>()
    where T : class, IModuleManager
        {
            Type interfaceType = typeof(T);
            Type derivedType = null;
            var hasType = externalInterfaceModuleDict.TryGetValue(interfaceType, out derivedType);
            if (!hasType)
            {
                foreach (var m in moduleDict)
                {
                    if (interfaceType.IsAssignableFrom(m.Key))
                    {
                        derivedType = m.Key;
                        break;
                    }
                }
            }
            GameObject moduleMount;
            var hasMount = externalModuleMountDict.TryGetValue(derivedType, out moduleMount);
            if (!hasMount)
            {
                moduleMount = new GameObject(derivedType.Name + "Module-->>Container");
                moduleMount.transform.SetParent(InstanceObject.transform);
                if (!externalModuleMountDict.TryAdd(derivedType, moduleMount))
                {
                    GameObject.Destroy(moduleMount);
                }
            }
            return moduleMount;
        }
        /// <summary>
        /// 初始化自定义模块
        /// </summary>
        /// <param name="assembly">模块所在程序集</param>
        public static void InitExternalModule(Assembly assembly)
        {
            Type[] types = assembly.GetTypes();
            var moduleType = typeof(Module);
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].GetCustomAttribute<ExternalModuleAttribute>() != null && moduleType.IsAssignableFrom(types[i]))
                {
                    try
                    {
                        var module = Utility.Assembly.GetTypeInstance(types[i]) as Module;
                        var result = externalModuleDict.TryAdd(types[i], module);
                        if (result)
                        {
                            module.OnInitialization();
                            Utility.Debug.LogInfo($"Custome Module :{module} is OnInitialization");
                        }
                    }
                    catch
                    {
                        Utility.Debug.LogError($"Custome module create instance fail:{types[i]}");
                    }
                }
            }
            ActiveExternalModule();
        }
        static void ActiveExternalModule()
        {
            foreach (var module in externalModuleDict.Values)
            {
                module.OnActive();
            }
            PrepareExternalModule();
        }
        static void PrepareExternalModule()
        {
            foreach (var module in externalModuleDict.Values)
            {
                module.OnPreparatory();
            }
            ExternalListenRefresh();
        }
        static void ExternalListenRefresh()
        {
            foreach (var module in externalModuleDict.Values)
            {
                GameManager.RefreshHandler += module.OnRefresh;
                GameManager.LateRefreshHandler += module.OnLateRefresh;
                GameManager.FixedRefreshHandler += module.OnFixRefresh;
            }
        }
    }
}
