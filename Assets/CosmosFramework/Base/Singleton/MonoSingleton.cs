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
                        instance.OnInitialization();
                    }
                }
                return instance;
            }
        }
        protected virtual void Awake()
        {
            instance = this as T;
        }
        protected virtual void OnDestroy()
        {
            OnTermination();
        }
        /// <summary>
        //空的虚方法，在当前单例对象为空初始化时执行一次
        /// </summary>
        protected virtual void OnInitialization() { }
        /// <summary>
        //空的虚方法，在当前单例对象被销毁时执行一次
        /// </summary>
        protected virtual void OnTermination() { }
    }
}
