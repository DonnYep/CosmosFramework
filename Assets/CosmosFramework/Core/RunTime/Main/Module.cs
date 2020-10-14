using UnityEngine;
using System.Collections;
using System;
namespace Cosmos
{

    /// <summary>
    /// 模块的抽象基类；
    /// 外部可扩展；
    /// </summary>
    public abstract class Module<T> : IModule
        where T : Module<T>, new()
    {
        #region Properties
        #region IMountPoint
        GameObject mountPoint;
        public GameObject MountPoint
        {
            get
            {
                if (mountPoint == null)
                {
                    mountPoint = new GameObject(ModuleName + "Module-->>Container");
                    mountPoint.transform.SetParent(GameManager.InstanceObject.transform);
                }
                return mountPoint;
            }
        }
        #endregion
        public bool IsPause { get; protected set; }
        /// <summary>
        /// 模块的非完全限定名 
        /// </summary>
        string moduleName = null;
        public string ModuleName
        {
            get
            {
                if (string.IsNullOrEmpty(moduleName))
                    moduleName = typeof(T).Name;
                    //moduleName = Utility.Text.StringSplit(Utility.Assembly.GetTypeFullName<T>(), new string[] { "." }, true, 2);
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
                    moduleFullName = typeof(T).FullName;
                return moduleFullName;
            }
        }
        #endregion

        #region Methods
        #region interface IModule
        /// <summary>
        /// 空虚函数;
        /// </summary>
        public virtual void OnInitialization(){}
        /// <summary>
        /// 非空虚函数，停止模块
        /// 在子类调用时，建议保留执行父类函数
        /// </summary>
        public virtual void OnTermination()
        {
            //TODO 生命周期销毁问题 ，module
            mountPoint = null;
            moduleName = null;
            moduleFullName = null;
        }
        /// <summary>
        /// 非空虚函数
        /// 覆写时请尽量保留父类方法
        /// </summary>
        public virtual void OnRefresh() { if (IsPause) return; }
        public virtual void OnFixRefresh() { if (IsPause) return; }
        public virtual void OnLateRefresh() { if (IsPause) return; }
        public virtual void OnPreparatory() { }
        public virtual void OnPause() { IsPause = true; }
        public virtual void OnUnPause() { IsPause = false; }
        #endregion
        #endregion
    }
}