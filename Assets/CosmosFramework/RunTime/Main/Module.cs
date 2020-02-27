using UnityEngine;
using System.Collections;
namespace Cosmos {

    /// <summary>
    /// 模块的抽象基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class  Module<T> : IModule
        where T:Module<T>,new()
    {
        static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                    instance.InitModule();
                }
                instance.OnModuleInstanceCalled();
                return instance;
            }
        }
        GameObject moduleMountObject;
        public GameObject ModuleMountObject
        {
            get
            {
                if (moduleMountObject == null)
                {
                    moduleMountObject = new GameObject(moduleName + "Module-->>Container");
                    moduleMountObject.transform.SetParent(GameManager.Instance.InstanceObject.transform);
                }
                return moduleMountObject;
            }
        }
        /// <summary>
        ///模块的枚举
        /// </summary>
        protected string moduleName="";
        protected abstract void InitModule();
        /// <summary>
        /// 注册模块
        /// </summary>
        protected virtual void RegisterModule(string moduleName)
        {
            this.moduleName = moduleName;
            RegisterModule();
        }
         void RegisterModule()
        {
            GameManager.Instance.RegisterModule(moduleName, this);
            Utility.DebugLog("Module:\"" + moduleName + "Manager\"" + " is registered !" + "\n based on Module register function");
        }
        /// <summary>
        /// 注销模块
        /// </summary>
        public virtual void DeregisterModule()
        {
            instance = null;
        }
        /// <summary>
        /// 当模块对象被调用时执行，每次调用都会执行
        /// </summary>
        protected virtual void OnModuleInstanceCalled() { }
        #region interface IModule
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual  void OnInitialization()
        {
            //这部分当前为测试，可删
            Utility.DebugLog("Module:\"" + moduleName + "Manager\"" + "is OnInitialization" + "\n based on Module register function");
            Utility.DebugLog(ModuleMountObject.name);
        }
        /// <summary>
        /// 暂停
        /// </summary>
        public virtual  void OnPause() { }
        /// <summary>
        /// 停止
        /// </summary>
        public virtual  void OnTermination() { }
        /// <summary>
        /// 恢复暂停
        /// </summary>
        public virtual void OnUnPause() { }
        #endregion
    }
}