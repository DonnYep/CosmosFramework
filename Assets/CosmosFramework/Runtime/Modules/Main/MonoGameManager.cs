using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos
{
    /// <summary>
    /// GameManager是静态的，因此由此类代理完成生命周期
    /// <para>执行顺序提前到所有默认脚本之前：模块的 OnUpdate（输入采样、音频过渡、下载推进）
    /// 先于业务脚本执行，因此业务脚本在 Update 里就能读到"本帧刚按下"的按键，不会晚一帧。</para>
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-20000)]
    public sealed class MonoGameManager : MonoSingleton<MonoGameManager>
    {
        DateTime previousTimeSinceStartup;
        /// <summary>
        /// moduleType===gameObject
        /// </summary>
        readonly Dictionary<Type, GameObject> moduleInstanceObjectDict = new Dictionary<Type, GameObject>();
        bool pause;
        Action onApplicationQuitHandler;
        public bool Pause
        {
            get { return pause; }
            set
            {
                if (pause == value)
                    return;
                pause = value;
                //同步全局暂停标记，取消暂停时模块刷新才能恢复
                GameManager.Pause = value;
            }
        }

        public event Action OnApplicationQuitHandler
        {
            add { onApplicationQuitHandler += value; }
            remove { onApplicationQuitHandler -= value; }
        }
        public GameObject GetModuleInstanceObject(IModuleManager moduleManager)
        {
            if (moduleManager == null)
                throw new ArgumentNullException("Module manager is invalid !");
            var type = moduleManager.GetType();
            if (!GameManager.HasModule(type))
                throw new InvalidOperationException($"Module : {type} is not registered, instance object is unavailable.");
            //假 null（节点被外部销毁）也走重建路径
            if (moduleInstanceObjectDict.TryGetValue(type, out var instanceObject) && instanceObject != null)
                return instanceObject;

            instanceObject = new GameObject($"{type.Name} (ModuleInstance)");
            //不继承父节点世界坐标，避免 CosmosRoot 偏移（浮动原点等）时挂载点带上额外位移
            instanceObject.transform.SetParent(transform, false);
            instanceObject.transform.localPosition = Vector3.zero;
            instanceObject.transform.localRotation = Quaternion.identity;
            instanceObject.transform.localScale = Vector3.one;
            moduleInstanceObjectDict[type] = instanceObject;
            return instanceObject;
        }
        /// <summary>
        /// 清理所有模块挂载点；
        /// <para>由 <see cref="GameManager.TerminateModules"/> 调用，保证挂载点生命周期与模块注册表一致。</para>
        /// </summary>
        internal static void ClearModuleInstanceObjects()
        {
            //只查不建：终止流程里不能把管理器又实例化出来
            if (instance == null)
                return;
            instance.ClearModuleInstanceObjectsInternal();
        }
        void ClearModuleInstanceObjectsInternal()
        {
            foreach (var instanceObject in moduleInstanceObjectDict.Values)
            {
                if (instanceObject == null)
                    continue;
                if (Application.isPlaying)
                    Destroy(instanceObject);
                else
                    DestroyImmediate(instanceObject);
            }
            moduleInstanceObjectDict.Clear();
        }
        private void Awake()
        {
            gameObject.name = "CosmosRoot";
            previousTimeSinceStartup = DateTime.Now;
            //记录主线程 ID：后台线程投递的网络事件等回调据此判断是否需要切换线程
            UnityMainThread.Initialize();
        }
        private void OnDestroy()
        {
            moduleInstanceObjectDict.Clear();
            GameManager.TerminateModules();
        }
        private void FixedUpdate()
        {
            if (pause)
                return;
            GameManager.OnFixedRefresh();
        }
        private void Update()
        {
            float deltaTime = (float)(DateTime.Now.Subtract(previousTimeSinceStartup).TotalMilliseconds / 1000.0f);
            previousTimeSinceStartup = DateTime.Now;

            if (pause)
                return;
            GameManager.OnRefresh();
            GameManager.OnElapseRefresh(deltaTime);
            //模块刷新之后统一执行后台线程投递的回调（网络通道的 await 续体等）
            UnityMainThread.Drain();
        }
        private void LateUpdate()
        {
            if (pause)
                return;
            GameManager.OnLateRefresh();
        }
        private void OnApplicationQuit()
        {
            onApplicationQuitHandler?.Invoke();
        }
    }
}