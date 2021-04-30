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
    /// <typeparam name="TDerived"></typeparam>
  //  [DefaultExecutionOrder(-1000)]
    public abstract class MonoSingleton<TDerived> : MonoBehaviour
    where TDerived : MonoSingleton<TDerived>
    {
        /// <summary>
        /// 实例所挂载的对象Mountobject
        /// </summary>
        public GameObject InstanceObject { get { return Instance.gameObject; } }
        protected static TDerived instance = null;
        public static TDerived Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType(typeof(TDerived)) as TDerived;//先从环境中查找类型
                    if (instance == null)//假如还是空，则生成一个
                    {
                        instance = new GameObject(typeof(TDerived).ToString(), typeof(TDerived)).GetComponent<TDerived>();
                    }
                }
                return instance;
            }
        }
        /// <summary>
        /// 非空虚函数；
        /// 覆写是需要保留Awake函数
        /// </summary>
        protected virtual void Awake()
        {
            instance = this as TDerived;
        }
        /// <summary>
        /// 空虚函数
        /// </summary>
        protected virtual void OnDestroy() { }
    }
}
