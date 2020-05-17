using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos {
    /// <summary>
    /// GameManager是静态的，因此由此类代理完成生命周期
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1000)]
    public sealed class GameManagerAgent :MonoSingleton<GameManagerAgent>
    {
        bool isPause = false;
        public bool Pause
        {
            get { return isPause; }
            set
            {
                if (isPause == value)
                    return;
                isPause = value;
                if (isPause)
                {
                    OnPause();
                }
                else
                {
                    OnUnPause();
                }
            }
        }
        public void LaunchAgent()
        {
            Utility.DebugLog("Inited GameManagerAgent");
        }
        Dictionary<string, IModule> moduleDict=GameManager.ModuleDict;
        event CFAction CFrameworkUpdateHandler;
        event CFAction CFrameworkFixedUpdateHandler;
        event CFAction CFrameworkLateUpdateHandler;
        event CFAction CFrameworkApplicationQuitHandler;
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);
        }
        public void AddFixedUpdateListener(CFAction handler)
        {
            CFrameworkFixedUpdateHandler += handler;
        }
        private void FixedUpdate()
        {
            CFrameworkFixedUpdateHandler?.Invoke();
        }
        private void Update()
        {
            CFrameworkUpdateHandler?.Invoke();
            if (isPause)
                return;
            foreach ( KeyValuePair<string,IModule> module in moduleDict)
            {
                module.Value.OnRefresh();
            }
        }
        private void LateUpdate()
        {
            CFrameworkLateUpdateHandler?.Invoke();
        }
        protected override void OnDestroy()
        {
            foreach (KeyValuePair<string, IModule> module in moduleDict)
            {
                module.Value.OnTermination();
            }
        }
        void OnPause()
        {
            foreach (KeyValuePair<string, IModule> module in moduleDict)
            {
                module.Value.OnPause();
            }
        }
        void OnUnPause()
        {
            foreach (KeyValuePair<string, IModule> module in moduleDict)
            {
                module.Value.OnUnPause();
            }
        }
        private void OnApplicationQuit()
        {
            CFrameworkApplicationQuitHandler?.Invoke();
        }
    }
}