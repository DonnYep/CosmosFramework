using Cosmos.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Cosmos.Hotfix
{
    public enum HotfixMethodType
    {
        Invalid,
        Action,
        Action_1Arg,
        Action_2Arg,
        Action_3Arg,
        Func,
        Func_1Arg,
        Func_2Arg,
        Func_3Arg
    }
    /// <summary>
    /// 热修复管理器
    /// </summary>
    [Module]
    internal class HotfixManager : Module, IHotfixManager
    {
        #region Properties
        /// <summary>
        /// 是否启用热更新；
        /// </summary>
        public bool HotfixEnable { get; set; } = false;
        /// <summary>
        /// 热更新DLL
        /// </summary>
        public TextAsset HotfixDll { get; private set; }
        /// <summary>
        /// 热更新程序集
        /// </summary>
        public Assembly HotfixAssembly { get; private set; }
        /// <summary>
        /// 热更新环境
        /// </summary>
        public object HotfixEnvironment { get; private set; }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        public Dictionary<HotfixMethodType, Dictionary<string, MethodInfo>> FixedMethods { get; private set; }
        /// <summary>
        /// 热修复后的方法
        /// </summary>
        public Dictionary<HotfixMethodType, Dictionary<string, Delegate>> FixedDelegates { get; private set; }
        /// <summary>
        /// 热更新库文件AB包名称
        /// "hotfix"
        /// </summary>
        public string HotfixDllAssetBundleName { get; private set; }
        /// <summary>
        /// 热更新库文件路径
        ///  "Assets/Hotfix/Hotfix.dll.bytes";
        /// </summary>
        public string HotfixDllAssetsPath { get; private set; }
        /// <summary>
        /// 执行热更新逻辑事件
        /// </summary>
        public event Action UpdateHotfixLogicEvent
        {
            add { updateHotfixLogicEvent += value; }
            remove { updateHotfixLogicEvent -= value; }
        }
        Action updateHotfixLogicEvent;
        /// <summary>
        /// 热更新准备完成事件
        /// </summary>
        public event Action HotfixReady
        {
            add { hotfixReady += value; }
            remove { hotfixReady -= value; }
        }
        Action hotfixReady;
        #endregion
        #region Methods
        public override void OnInitialization()
        {
            FixedMethods = new Dictionary<HotfixMethodType, Dictionary<string, MethodInfo>>();
            FixedDelegates = new Dictionary<HotfixMethodType, Dictionary<string, Delegate>>();
        }
        public override void OnPreparatory()
        {
            if (HotfixEnable)
            {
                IResourceManager resourceManager = GameManager.GetModule<IResourceManager>();
                //if (resourceManager.LoadMode == ResourceLoadMode.Resource)
                //{
                //    throw new ArgumentException("HotfixManager-->>热更新初始化失败：热更新库不支持使用Resource加载模式！");
                //}
                //AssetInfo info = new AssetInfo(HotfixDllAssetBundleName, HotfixDllAssetsPath, "");
                //resourceManager.LoadAssetAsync<TextAsset>(0,info, HotfixDllLoadDone, null);
            }
        }
        /// <summary>
        /// 设置热更新dll的ab包名称
        /// </summary>
        /// <param name="hotfixDllAssetBundleName">热更新dll的ab包名称</param>
        public void SetHotfixDllAssetBundleName(string hotfixDllAssetBundleName)
        {
            this.HotfixDllAssetBundleName = hotfixDllAssetBundleName;
        }
        /// <summary>
        /// 设置热更新dll资源路径
        /// </summary>
        /// <param name="hotfixDllAssetsPath">热更新dll资源路径</param>
        public void SetHotfixDllAssetsPath(string hotfixDllAssetsPath)
        {
            this.HotfixDllAssetsPath = hotfixDllAssetsPath;
        }
        public Action FixMethod(Action action)
        {
            Delegate del = FixMethod(HotfixMethodType.Action, action.Target.GetType().FullName + "." + action.Method.Name, typeof(Action));
            if (del != null) return del as Action;
            else return action;
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public Action<T> FixMethod<T>(Action<T> action)
        {
            Delegate del = FixMethod(HotfixMethodType.Action_1Arg, action.Target.GetType().FullName + "." + action.Method.Name, typeof(Action<T>));
            if (del != null) return del as Action<T>;
            else return action;
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public Action<T1, T2> FixMethod<T1, T2>(Action<T1, T2> action)
        {
            Delegate del = FixMethod(HotfixMethodType.Action_2Arg, action.Target.GetType().FullName + "." + action.Method.Name, typeof(Action<T1, T2>));
            if (del != null) return del as Action<T1, T2>;
            else return action;
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public Action<T1, T2, T3> FixMethod<T1, T2, T3>(Action<T1, T2, T3> action)
        {
            Delegate del = FixMethod(HotfixMethodType.Action_3Arg, action.Target.GetType().FullName + "." + action.Method.Name, typeof(Action<T1, T2, T3>));
            if (del != null) return del as Action<T1, T2, T3>;
            else return action;
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public Func<TResult> FixMethod<TResult>(Func<TResult> action)
        {
            Delegate del = FixMethod(HotfixMethodType.Func, action.Target.GetType().FullName + "." + action.Method.Name, typeof(Func<TResult>));
            if (del != null) return del as Func<TResult>;
            else return action;
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public Func<T, TResult> FixMethod<T, TResult>(Func<T, TResult> action)
        {
            Delegate del = FixMethod(HotfixMethodType.Func_1Arg, action.Target.GetType().FullName + "." + action.Method.Name, typeof(Func<T, TResult>));
            if (del != null) return del as Func<T, TResult>;
            else return action;
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public Func<T1, T2, TResult> FixMethod<T1, T2, TResult>(Func<T1, T2, TResult> action)
        {
            Delegate del = FixMethod(HotfixMethodType.Func_2Arg, action.Target.GetType().FullName + "." + action.Method.Name, typeof(Func<T1, T2, TResult>));
            if (del != null) return del as Func<T1, T2, TResult>;
            else return action;
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public Func<T1, T2, T3, TResult> FixMethod<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> action)
        {
            Delegate del = FixMethod(HotfixMethodType.Func_3Arg, action.Target.GetType().FullName + "." + action.Method.Name, typeof(Func<T1, T2, T3, TResult>));
            if (del != null) return del as Func<T1, T2, T3, TResult>;
            else return action;
        }
        private Delegate FixMethod(HotfixMethodType methodType, string targetName, Type type)
        {
            if (!HotfixEnable)
            {
                return null;
            }

            if (FixedDelegates[methodType].ContainsKey(targetName))
            {
                return FixedDelegates[methodType][targetName];
            }
            else
            {
                if (FixedMethods[methodType].ContainsKey(targetName))
                {
                    Delegate del = Delegate.CreateDelegate(type, FixedMethods[methodType][targetName]);
                    FixedDelegates[methodType].Add(targetName, del);
                    return del;
                }
                else
                {
                    return null;
                }
            }
        }
        private void HotfixDllLoadDone(TextAsset asset)
        {
            HotfixDll = asset;
            HotfixAssembly = Assembly.Load(HotfixDll.bytes, null);
            HotfixEnvironment = HotfixAssembly.CreateInstance("HotfixEnvironment");
            if (HotfixEnvironment == null)
            {
                throw new ArgumentNullException($"HotfixManager-->>热更新初始化失败：热更新库中不存在热更新环境 HotfixEnvironment！");
            }
            SearchHotfixMethod();
            hotfixReady?.Invoke();
        }
        private void SearchHotfixMethod()
        {
            Type[] types = HotfixAssembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                MethodInfo[] methods = types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                for (int j = 0; j < methods.Length; j++)
                {
                    HotfixMethodAttribute att = methods[j].GetCustomAttribute<HotfixMethodAttribute>();
                    if (att != null)
                    {
                        HotfixMethodType methodType = GetHotfixMethodType(methods[j]);
                        if (!FixedMethods[methodType].ContainsKey(att.TargetName))
                        {
                            FixedMethods[methodType].Add(att.TargetName, methods[j]);
                        }
                    }
                }
            }
            if (FixedMethods[HotfixMethodType.Invalid].Count > 0)
            {
                foreach (var item in FixedMethods[HotfixMethodType.Invalid])
                {
                    Utility.Debug.LogError("发现无效的热修复方法：" + item.Value.Name);
                }
                FixedMethods[HotfixMethodType.Invalid].Clear();
            }
        }
        private HotfixMethodType GetHotfixMethodType(MethodInfo method)
        {
            bool isVoid = method.ReturnType.Name == "Void";
            ParameterInfo[] pis = method.GetParameters();
            if (isVoid)
            {
                switch (pis.Length)
                {
                    case 0:
                        return HotfixMethodType.Action;
                    case 1:
                        return HotfixMethodType.Action_1Arg;
                    case 2:
                        return HotfixMethodType.Action_2Arg;
                    case 3:
                        return HotfixMethodType.Action_3Arg;
                    default:
                        return HotfixMethodType.Invalid;
                }
            }
            else
            {
                switch (pis.Length)
                {
                    case 0:
                        return HotfixMethodType.Func;
                    case 1:
                        return HotfixMethodType.Func_1Arg;
                    case 2:
                        return HotfixMethodType.Func_2Arg;
                    case 3:
                        return HotfixMethodType.Func_3Arg;
                    default:
                        return HotfixMethodType.Invalid;
                }
            }
        }
        #endregion

    }
}