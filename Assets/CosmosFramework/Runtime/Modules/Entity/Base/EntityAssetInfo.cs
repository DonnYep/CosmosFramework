using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 实体资源信息；
    /// </summary>
    public class EntityAssetInfo : AssetInfo
    {
        readonly string entityGroupName;
        public string EntityGroupName { get { return entityGroupName; } }
        public bool UseObjectPool { get; set; }
        public EntityAssetInfo(string entityGroupName, string assetBundleName, string assetPath) :
            base(assetBundleName, assetPath)
        {
            this.entityGroupName = entityGroupName;
        }
        public EntityAssetInfo(string entityGroupName, string assetPath) : base(assetPath)
        {
            this.entityGroupName = entityGroupName;
        }
    }
}
