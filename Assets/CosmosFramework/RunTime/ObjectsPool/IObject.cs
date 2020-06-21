using System;
namespace Cosmos
{
    /// <summary>
    /// 对象池生成的对象目标的接口
    /// </summary>
    public interface IObject:IBehaviour
    {
        void OnSpawn();
        void OnDespawn();
        Type Type { get; }
    }
}
