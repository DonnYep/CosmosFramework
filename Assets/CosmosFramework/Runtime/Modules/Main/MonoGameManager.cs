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
            var type = moduleManager.GetType();
            var hasType = GameManager.HasModule(type);
            if (!hasType)
                return null;
            GameObject instanceObject;
            var hasMount = moduleInstanceObjectDict.TryGetValue(type, out instanceObject);
            if (!hasMount)
            {
                instanceObject = new GameObject(type.Name + "-->>InstanceObject");
                instanceObject.transform.SetParent(transform);
                moduleInstanceObjectDict[type] = instanceObject;
            }
            else
            {
                if (instanceObject == null)
                {
                    instanceObject = new GameObject(type.Name + "-->>InstanceObject");
                    instanceObject.transform.SetParent(transform);
                    moduleInstanceObjectDict[type] = instanceObject;
                }
            }
            return instanceObject;
        }
        private void Awake()
        {
            gameObject.name = "CosmosRoot";
            previousTimeSinceStartup = DateTime.Now;
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