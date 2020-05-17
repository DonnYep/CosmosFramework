using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos
{
    /// <summary>
    /// 继承mono的单例基类
    ///默认情况下, Awake自动适配，应用退出时释放。
    ///Awake与OnDestory是可覆写函数
    /// </summary>
    /// <typeparam name="T"></typeparam>
  //  [DefaultExecutionOrder(-1000)]
    public abstract class MonoSingleton<T> : MonoBehaviour
    where T : MonoSingleton<T>
    {
        /// <summary>
        /// 实例所挂载的对象Mountobject
        /// </summary>
        public GameObject InstanceObject { get { return Instance.gameObject; } }
        protected static T instance = null;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType(typeof(T)) as T;//先从环境中查找类型
                    if (instance == null)//假如还是空，则生成一个
                    {
                        instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
                    }
                }
                return instance;
            }
        }
        /// <summary>
        /// 非空，可覆写Awake函数
        /// </summary>
        protected virtual void Awake()
        {
            instance = this as T;
        }
        /// <summary>
        /// 空虚函数
        /// </summary>
        protected virtual void OnDestroy() { }
    }
}
