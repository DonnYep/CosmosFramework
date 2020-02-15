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
    public abstract class Singleton<T>:IDisposable
        where T : new()
    {
        static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                    instance = new T();
                return instance;
            }
        }

        public void Dispose()
        {
        }
        protected virtual void ClearSingleton()
        {
            instance= default(T);
        }
    }
    /// <summary>
    /// 多线程单例基类，内部包含线程锁
    /// 慎用!!!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MultiThreadSingleton<T>
       where T : new()
    {
        static T instance;
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
    }
    /// <summary>
    /// 继承mono的单例基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour
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
                    }
                }
                return instance;
            }
        }
        protected virtual void Awake()
        {
            instance = this as T;
        }

        private void OnApplicationQuit()
        {
            instance = null;
        }
        virtual public void Init() { }//抽象初始化
    }
}
