using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos
{
    /// <summary>
    /// GameManager是静态的，因此由此类代理完成生命周期
    /// </summary>
    [DisallowMultipleComponent]
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
                if (pause)
                {
                    GameManager.Pause = true;
                }
                else
                {
                    GameManager.Pause = true;
                }
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