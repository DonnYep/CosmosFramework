using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    /// <summary>
    /// 所有CosmosFramework ScriptableObject 对象的基类
    /// 原则上打包发布后时不允许修改的，因此所有继承此类的子类都只有只读属性
    /// </summary>
    public abstract class CFScriptableObject : ScriptableObject
    {
        /// <summary>
        /// 所有对象共有的名称
        /// </summary>
        [SerializeField]
        protected string objectName = "Newobject";
        public string ObjectName { get { return objectName; } }
        /// <summary>
        /// 重置清空内容
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// 仅仅在Editor模式下使用
        /// </summary>
        public virtual void Preview() { }

        /// <summary>
        /// 执行对象中的函数
        /// </summary>
        public virtual void Execute() { }
    }
}