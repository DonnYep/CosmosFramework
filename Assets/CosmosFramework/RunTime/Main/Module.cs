using UnityEngine;
using System.Collections;
namespace Cosmos
{

    /// <summary>
    /// 模块的抽象基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Module<T> : Singleton<T>, IModule
        where T : Module<T>, new()
    {
        #region interface IModule
        /// <summary>
        /// 非空虚函数;
        /// 可覆写初始化方法，覆写时需要执行父类方法
        /// </summary>
        protected override void OnInitialization()
        {
            GameManager.Instance.RegisterModule(ModuleName, this);
        }
        public override void Dispose()
        {
            OnTermination();
        }
        /// 非空虚函数，停止模块
        /// 在子类调用时，建议保留执行父类函数
        /// </summary>
        protected override void OnTermination()
        {
            moduleMountObject = null;
            moduleName = null;
            moduleFullName = null;
        }
        public virtual void OnRefresh() { }
        public virtual void OnPreparatory() { }
        public virtual void OnPause() { }
        public virtual void OnUnPause() { }
        #endregion
        GameObject moduleMountObject;
        public GameObject ModuleMountObject
        {
            get
            {
                if (moduleMountObject == null)
                {
                    moduleMountObject = new GameObject(ModuleName + "Module-->>Container");
                    moduleMountObject.transform.SetParent(GameManager.Instance.InstanceObject.transform);
                }
                return moduleMountObject;
            }
        }
        /// <summary>
        /// 模块的非完全限定名 
        /// </summary>
        string moduleName = null;
        public string ModuleName
        {
            get
            {
                if (string.IsNullOrEmpty(moduleName))
                    moduleName = Utility.Text.StringSplit(Utility.Assembly.GetTypeFullName<T>(), new string[] { "." }, true, 2);
                return moduleName;
            }
        }
        /// <summary>
        /// 模块的完全限定名
        /// </summary>
        string moduleFullName = null;
        public string ModuleFullName
        {
            get
            {
                if (string.IsNullOrEmpty(moduleFullName))
                    moduleFullName = Utility.Assembly.GetTypeFullName<T>();
                return moduleFullName;
            }
        }
        public void DebugModule() { }
        public void Deregister()
        {
            Dispose();
        }

    }
}