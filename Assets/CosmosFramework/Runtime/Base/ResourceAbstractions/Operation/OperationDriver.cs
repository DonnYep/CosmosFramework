using UnityEngine;
namespace Cosmos.Operation
{
    /// <summary>
    /// 异步操作每帧驱动组件，由 <see cref="OperationSystem"/> 自动创建。
    /// </summary>
    public class OperationDriver : MonoBehaviour
    {
        private void Update()
        {
            OperationSystem.Update();
        }
    }
}
