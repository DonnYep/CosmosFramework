using UnityEngine;

namespace Cosmos.Test
{
    /// <summary>
    /// 配置测试类，仅用于案例部分；
    /// </summary>
    [DefaultExecutionOrder(2000)]
    public class Entry : CosmosEntryConfig
    {
        static Entry instance;
        public static Entry Instance { get { return instance; } }
        protected override void Awake()
        {
            if (instance != null)
                GameObject.Destroy(gameObject);
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                base.Awake();
            }
        }
    }
}