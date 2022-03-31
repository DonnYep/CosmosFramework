using FixMath.NET;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Cosmos
{
    public partial class AOIZoneFix64<T>
        where T : class
    {
        public class AOIEntity : IDisposable
        {
            T handle;
            long entityKey;

            Fix64 positionX;
            Fix64 positionY;

            Fix64 viewDistance;

            HashSet<AOIEntity> viewEntities;
            /// <summary>
            /// 实体的备份；
            /// </summary>
            HashSet<AOIEntity> viewEntitiesBak;
            public AOIEntity()
            {
                viewEntities = new HashSet<AOIEntity>();
                viewEntitiesBak = new HashSet<AOIEntity>();
            }
            public Fix64 ViewDistance
            {
                get { return viewDistance; }
                set
                {
                    if (value.RawValue < 0)
                        viewDistance = Fix64.Zero;
                    viewDistance = value;
                }
            }
            public long EntityKey { get { return entityKey; } set { entityKey = value; } }
            public Fix64 PositionX { get { return positionX; } set { positionX = value; } }
            public Fix64 PositionY { get { return positionY; } set { positionY = value; } }
            /// <summary>
            /// X轴节点；
            /// </summary>
            public AOISkipList<AOIEntity, Fix64>.AOISkipListNode XNode { get; set; }
            /// <summary>
            /// Y轴节点；
            /// </summary>
            public AOISkipList<AOIEntity, Fix64>.AOISkipListNode YNode { get; set; }
            /// <summary>
            /// 实际的数据；
            /// </summary>
            public T Handle { get { return handle; } set { handle = value; } }
            /// <summary>
            /// 当前能够看见的实体；
            /// </summary>
            public HashSet<AOIEntity> ViewEntities { get { return viewEntities; } }
            /// <summary>
            /// 保持视线内的实体；
            /// </summary>
            public IEnumerable<AOIEntity> Staying { get { return ViewEntities.Union(viewEntitiesBak); } }
            /// <summary>
            /// 离开视线的实体；
            /// </summary>
            public IEnumerable<AOIEntity> Exited { get { return viewEntitiesBak.Except(ViewEntities); } }
            /// <summary>
            /// 新进入视线的实体；
            /// </summary>
            public IEnumerable<AOIEntity> Entered { get { return ViewEntities.Except(viewEntitiesBak); } }
            public void SwapViewEntity()
            {
                viewEntitiesBak.Clear();
                var tmp = viewEntities;
                viewEntities = viewEntitiesBak;
                viewEntitiesBak = tmp;
            }
            public void Dispose()
            {
                ViewEntities.Clear();
                viewEntitiesBak.Clear();
                entityKey = 0;
            }
        }
    }
}