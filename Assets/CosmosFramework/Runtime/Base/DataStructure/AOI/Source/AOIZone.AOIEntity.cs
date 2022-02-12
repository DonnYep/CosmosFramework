using System;
using System.Collections.Generic;
using System.Linq;

namespace Cosmos
{
    public partial class AOIZone<T>
    {
        public class AOIEntity : IDisposable
        {
            T handle;
            long entityKey;

            float positionX;
            float positionY;

            float viewDistance;

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
            public float ViewDistance
            {
                get { return viewDistance; }
                set
                {
                    if (value < 0)
                        viewDistance = 0;
                    viewDistance = value;
                }
            }
            public long EntityKey { get { return entityKey; } set { entityKey = value; } }
            public float PositionX { get { return positionX; } set { positionX = value; } }
            public float PositionY { get { return positionY; } set { positionY = value; } }
            /// <summary>
            /// X轴节点；
            /// </summary>
            public AOISkipList<AOIEntity, float>.AOISkipListNode XNode { get; set; }
            /// <summary>
            /// Y轴节点；
            /// </summary>
            public AOISkipList<AOIEntity, float>.AOISkipListNode YNode { get; set; }
            /// <summary>
            /// 实际的数据；
            /// </summary>
            public T Handle { get { return handle; } set { handle = value; } }
            /// <summary>
            /// 当前能够看见的实体；
            /// </summary>
            public HashSet<AOIEntity> ViewEntities { get { return viewEntities; } }
            /// <summary>
            /// 保持视线内的实体集合；
            /// </summary>
            public IEnumerable<AOIEntity> Staying { get { return ViewEntities.Union(viewEntitiesBak); } }
            /// <summary>
            /// 离开视线的实体集合
            /// </summary>
            public IEnumerable<AOIEntity> Exited { get { return viewEntitiesBak.Except(ViewEntities); } }
            /// <summary>
            /// 新进入视线的实体集合；
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
