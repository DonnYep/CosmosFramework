using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public partial class AOIZone<T>
    {
        public class AOIEntity
        {
            /// <summary>
            /// 当前实体AOI列表；
            /// </summary>
            public HashSet<AOIEntity> AoiEntities= new HashSet<AOIEntity>();
            public void OnEntityEnter(AOIEntity entity)
            {
                AoiEntities.Add(entity);
            }
            public void OnEntityExit(AOIEntity entity)
            {
                AoiEntities.Remove(entity);
            }
        }
    }
}
