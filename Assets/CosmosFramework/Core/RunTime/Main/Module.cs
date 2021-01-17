using UnityEngine;
using System.Collections;
using System;
namespace Cosmos
{

    /// <summary>
    /// 模块的抽象基类；
    /// 外部可扩展；
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Module<T> : IModule
        where T : Module<T>, new()
    {
        #region Properties
        /// <summary>
        /// 获取游戏框架模块优先级。
        /// </summary>
        public virtual int Priority { get { return 0; } }
        #region IMountPoint
        public Type MountType { get { return typeof(T); } }
        GameObject mountPoint;
        public GameObject MountPoint
        {
            get
            {
                if (mountPoint == null)
                {
                    mountPoint = new GameObject(ModuleName + "Module-->>Container");
                    mountPoint.transform.SetParent(GameManager.Instance.InstanceObject.transform);
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
                    moduleName = Utility.Text.StringSplit(Utility.Assembly.GetTypeFullName<T>(), new string[] { "." }, true, 2);
                return moduleName;
            }
        }
        public ModuleEnum ModuleEnum
        {
            get
            {
                var module = ModuleName.Replace("Manager", "");
                return Utility.Framework.GetModuleEnum(module);
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
        }
        /// <summary>
        /// 非空虚函数
        /// 覆写时请尽量保留父类方法
        /// </summary>
        public virtual void OnRefresh() { if (IsPause) return; }
        public virtual void OnPreparatory() { }
        public virtual void OnPause() { IsPause = true; }
        public virtual void OnUnPause() { IsPause = false; }
        public virtual void OnActive(){}
        public virtual void OnDeactive(){}
        #endregion
        #endregion
    }
}