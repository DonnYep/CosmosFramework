using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
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
        Dictionary<ModuleEnum, IModule> moduleDict;
        event Action CFrameworkUpdateHandler;
        event Action CFrameworkFixedUpdateHandler;
        event Action CFrameworkLateUpdateHandler;
        event Action CFrameworkApplicationQuitHandler;
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);
            moduleDict = GameManager.ModuleDict;
        }
        public void AddFixedUpdateListener(Action handler)
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
            foreach ( KeyValuePair<ModuleEnum,IModule> module in moduleDict)
            {
                module.Value?.OnRefresh();
            }
        }
        private void LateUpdate()
        {
            CFrameworkLateUpdateHandler?.Invoke();
        }
        protected override void OnDestroy()
        {
            foreach (KeyValuePair<ModuleEnum, IModule> module in moduleDict)
            {
                module.Value?.OnTermination();
            }
        }
        void OnPause()
        {
            foreach (KeyValuePair<ModuleEnum, IModule> module in moduleDict)
            {
                module.Value.OnPause();
            }
        }
        void OnUnPause()
        {
            foreach (KeyValuePair<ModuleEnum, IModule> module in moduleDict)
            {
                module.Value.OnUnPause();
            }
        }
        private void OnApplicationQuit()
        {
            CFrameworkApplicationQuitHandler?.Invoke();
        }
        #region GameManager

        public int ModuleCount { get { return GameManager.Instance.ModuleCount; } }
        /// <summary>
        /// 清除单个实例，有一个默认参数。
        /// 默认延迟为0，表示立刻删除、
        /// 仅在场景中删除对应对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="t">默认参数，表示延迟</param>
        public static void KillObject(Object obj, float delay = 0)
        {
           GameManager .KillObject(obj, delay);
        }
        /// <summary>
        /// 立刻清理实例对象
        /// 会在内存中清理实例
        /// </summary>
        /// <param name="obj"></param>
        public static void KillObjectImmediate(Object obj)
        {
            GameManager.KillObjectImmediate(obj);
        }
        /// <summary>
        /// 清除一组实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs"></param>
        public static void KillObjects<T>(List<T> objs) where T : Object
        {
            for (int i = 0; i < objs.Count; i++)
            {
              GameManager.KillObject (objs[i]);
            }
            objs.Clear();
        }
        public static void KillObjects<T>(HashSet<T> objs) where T : Object
        {
            foreach (var obj in objs)
            {
                GameManager.KillObject(obj);
            }
            objs.Clear();
        }
        #endregion
    }
}