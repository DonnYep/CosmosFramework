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
        /// 模块-mount字典；
        ///  key=>moduleType；value=>gameobject
        /// </summary>
        Dictionary<Type, GameObject> moduleMountDict;
        bool pause;
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

        Action onApplicationQuitHandler;
        public event Action OnApplicationQuitHandler
        {
            add { onApplicationQuitHandler += value; }
            remove { onApplicationQuitHandler -= value; }
        }
        public GameObject GetModuleGameObject(IModuleInstance module)
        {
            var type = module.GetType();
            var hasType = GameManager.HasModule(type);
            if (!hasType)
                return null;
            GameObject moduleMount;
            var hasMount = moduleMountDict.TryGetValue(type, out moduleMount);
            if (!hasMount)
            {
                moduleMount = new GameObject(type.Name + "-->>Instance");
                moduleMount.transform.SetParent(transform);
                moduleMountDict[type] = moduleMount;
            }
            else
            {
                if (moduleMount == null)
                {
                    moduleMount = new GameObject(type.Name + "-->>Instance");
                    moduleMount.transform.SetParent(transform);
                    moduleMountDict[type] = moduleMount;
                }
            }
            return moduleMount;
        }
        protected override void Awake()
        {
            base.Awake();
            gameObject.name = "CosmosRoot";
            DontDestroyOnLoad(this.gameObject);
            previousTimeSinceStartup = DateTime.Now;
            moduleMountDict = new Dictionary<Type, GameObject>();
        }
        protected override void OnDestroy()
        {
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