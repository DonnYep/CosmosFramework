using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    //TODO 设计通用池数据
    /// <summary>
    /// 对象池数据，临时只能由对象池使用
    /// </summary>
    public class ObjectPoolVariable : Variable
    {
        public IObject SpawnItem { get; set; }
        public CFAction<IObject> OnSpawnHandler { get; set; }
        public CFAction<IObject> OnDespawnHandler { get; set; }
        public CFResultAction<IObject> CreateHandler { get; set; }
        /// <summary>
        /// 对象池变量构造
        /// </summary>
        /// <param name="spawnItem">需要生成的目标对象</param>
        /// <param name="onSpawn">池生成对象事件</param>
        /// <param name="onDespawn">池回收对象事件</param>
        /// <param name="onCreate">池为空时，生成函数</param>
        public ObjectPoolVariable(IObject spawnItem, CFAction<IObject> onSpawn, CFAction<IObject> onDespawn, CFResultAction<IObject> onCreate)
        {
            this.SpawnItem = spawnItem;
            this.OnSpawnHandler = onSpawn;
            this.OnDespawnHandler = onDespawn;
            this.CreateHandler = onCreate;
        }
        /// <summary>
        /// 对象池变量构造
        /// </summary>
        /// <param name="onSpawn">池生成对象事件</param>
        /// <param name="onDespawn">池回收对象事件</param>
        /// <param name="onCreate">池为空时，生成函数</param>
        public ObjectPoolVariable( CFAction<IObject> onSpawn, CFAction<IObject> onDespawn, CFResultAction<IObject> onCreate)
        {
            this.OnSpawnHandler = onSpawn;
            this.OnDespawnHandler = onDespawn;
            this.CreateHandler = onCreate;
        }
        public ObjectPoolVariable() { }
        public override void Clear()
        {
            this.OnDespawnHandler = null;
            this.OnSpawnHandler = null;
            this.CreateHandler = null;
            this.SpawnItem = null;
        }
    }
}
