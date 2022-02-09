using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public partial class AOIZone<T>
    {
        public class AOIEntity : IDisposable
        {
            T handle;
            HashSet<AOIEntity> viewEntity;
            HashSet<AOIEntity> viewEntityBak;
            public AOIEntity(T handle)
            {
                this.handle = handle;
                viewEntity = new HashSet<AOIEntity>();
                viewEntityBak = new HashSet<AOIEntity>();
            }
            public SkipListNode X { get; set; }
            public SkipListNode Y { get; set; }
            public T Handle { get { return handle; } }
            /// <summary>
            /// 当前能够看见的实体；
            /// </summary>
            public HashSet<AOIEntity> ViewEntity { get { return viewEntity; } }
            /// <summary>
            /// 实体的备份；
            /// </summary>
            public HashSet<AOIEntity> ViewEntityBak { get { return viewEntityBak; } }
            public void Dispose()
            {
                viewEntity.Clear();
                viewEntityBak.Clear();
            }
            public void OnEntityEnter(AOIEntity entity)
            {
            }
            public void OnEntityExit(AOIEntity entity)
            {
            }
        }
    }
}
