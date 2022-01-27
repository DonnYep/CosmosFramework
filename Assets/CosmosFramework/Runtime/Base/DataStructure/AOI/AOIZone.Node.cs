using System.Collections.Generic;

namespace Cosmos
{
    public partial class AOIZone<T>
    {
        /// <summary>
        /// 节点表示一个AOI网格区域；
        /// </summary>
        public class Node
        {
            HashSet<AOIEntity> aoiEntities = new HashSet<AOIEntity>();
            public int NodeIndex { get; private set; }
            public Square NodeArea { get; private set; }
            public HashSet<AOIEntity> AoiEntities { get { return aoiEntities; } }
            public Node(int nodeIndex, Square nodeArea)
            {
                NodeIndex = nodeIndex;
                NodeArea = nodeArea;
            }
            public bool Enter(AOIEntity entity)
            {
                return aoiEntities.Add(entity);
            }
            public bool Exit(AOIEntity entity)
            {
                return aoiEntities.Remove(entity);
            }
            public bool Contains(AOIEntity entity)
            {
                return aoiEntities.Contains(entity);
            }
        }
    }
}
