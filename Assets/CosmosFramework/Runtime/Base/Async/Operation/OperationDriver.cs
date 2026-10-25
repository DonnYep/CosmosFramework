using UnityEngine;
namespace Cosmos.Operation
{
    /// <summary>
    /// 异步操作每帧驱动组件（由 <see cref="OperationSystem"/> 自动创建、常驻 DontDestroyOnLoad）。
    /// <para>同时充当"框架内部协程执行器"：等待协程的操作（<see cref="WaitForCoroutineOperation"/>）
    /// 通过它跑协程，业务侧不需要自己找 MonoBehaviour。</para>
    /// </summary>
    public class OperationDriver : MonoBehaviour
    {
        /// <summary>
        /// 当前实例（由 Awake 赋值；未创建时为 null）
        /// </summary>
        internal static OperationDriver Instance { get; private set; }
        void Awake()
        {
            Instance = this;
        }
        void OnDestroy()
        {
            if (ReferenceEquals(Instance, this))
                Instance = null;
        }
        private void Update()
        {
            OperationSystem.Update();
        }
    }
}
