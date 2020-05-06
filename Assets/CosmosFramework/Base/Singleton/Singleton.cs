using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos
{
    /// <summary>
    /// 非mono单例的基类new()约束
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T>:IDisposable,IBehaviour
        where T : Singleton<T>,new()
    {
       protected static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                    instance.OnInitialization();
                }
                return instance;
            }
        }
        /// <summary>
        /// 空虚方法，IDispose接口
        /// </summary>
        public virtual void Dispose() { instance.OnTermination(); instance = default(T); }//TODO IDispose与OnTermination优先级
        public virtual void OnInitialization() { }
        public virtual void OnTermination() { }
    }
    /// <summary>
    /// 多线程单例基类，内部包含线程锁
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MultiThreadSingleton<T>: IDisposable
       where T : new()
    {
        protected static T instance;
        static readonly object locker = new object();
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if(instance==null)
                            instance = new T();
                    }
                }
                return instance;
            }
        }
        /// <summary>
        /// 空虚方法，IDispose接口
        /// </summary>
        public virtual void Dispose() { }

    }
    /// <summary>
    /// 继承mono的单例基类
    ///默认情况下, Awake自动适配，应用退出时释放。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour,IBehaviour
    where T : MonoSingleton<T>
    {
        /// <summary>
        /// 实例所挂载的对象Mountobject
        /// </summary>
        public  GameObject InstanceObject { get { return Instance.gameObject; } }
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
                        instance = new GameObject( typeof(T).ToString(), typeof(T)).GetComponent<T>();
                        instance.Init();
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
        [Obsolete("即将弃用，请移步至OnInitialization()")]
        virtual public void Init() { }
        /// <summary>
        //空的虚方法，在当前单例对象为空初始化时执行一次
        /// </summary>
        public virtual void OnInitialization() { }
        /// <summary>
        //空的虚方法，在当前单例对象被销毁时执行一次
        /// </summary>
        public virtual void OnTermination() { }
    }
}
