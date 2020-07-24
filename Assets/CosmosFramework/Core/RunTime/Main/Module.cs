using UnityEngine;
using System.Collections;
namespace Cosmos
{

    /// <summary>
    /// 模块的抽象基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class Module<T> : IModule
        where T : Module<T>, new()
    {
        #region Properties
        GameObject moduleMountObject;
        public bool IsPause { get; set; }

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
        string moduleFullyQualifiedName = null;
        public string ModuleFullyQualifiedName
        {
            get
            {
                if (string.IsNullOrEmpty(moduleFullyQualifiedName))
                    moduleFullyQualifiedName = Utility.Assembly.GetTypeFullName<T>();
                return moduleFullyQualifiedName;
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
            moduleMountObject = null;
            moduleName = null;
            moduleFullyQualifiedName = null;
        }
        /// <summary>
        /// 非空虚函数
        /// 覆写时请尽量保留父类方法
        /// </summary>
        public virtual void OnRefresh() { if (IsPause) return; }
        public virtual void OnPreparatory() { }
        public virtual void OnPause() { IsPause = true; }
        public virtual void OnUnPause() { IsPause = false; }
        #endregion
        public void DebugModule() { }
        #endregion
    }
}