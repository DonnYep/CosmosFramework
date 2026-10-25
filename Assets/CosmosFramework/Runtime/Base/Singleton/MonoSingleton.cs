using UnityEngine;
namespace Cosmos
{
    /// <summary>
    /// 继承mono的单例基类
    /// </summary>
    /// <typeparam name="TDerived"></typeparam>
  //  [DefaultExecutionOrder(-1000)]
    public abstract class MonoSingleton<TDerived> : MonoBehaviour
    where TDerived : MonoSingleton<TDerived>
    {
        protected static TDerived instance = null;
        public static TDerived Instance
        {
            get
            {
                if (instance == null)
                {
                    var type = typeof(TDerived);
                    instance = (TDerived)FindObjectOfType(type);//先从环境中查找类型
                    if (instance == null)//假如还是空，则生成一个
                    {
                        instance = new GameObject(type.ToString()).AddComponent<TDerived>();
                    }
                    //无论"找到"还是"新建"，首次获取都要走一次初始化：
                    //否则场景里预置的实例会漏掉 DontDestroyOnLoad 等初始化动作
                    instance.OnInitialize();
                }
                return instance;
            }
        }
        /// <summary>
        /// 首次被实例化调用
        /// </summary>
        protected virtual void OnInitialize()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
