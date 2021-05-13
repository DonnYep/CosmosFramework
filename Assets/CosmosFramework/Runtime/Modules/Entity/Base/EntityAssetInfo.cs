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
    public class EntityAssetInfo: AssetInfo
    {
        public EntityAssetInfo(string entityGroupName,  string assetBundleName, string assetPath, string resourcePath) :
            base(assetBundleName, assetPath, resourcePath) 
        {
            this.EntityGroupName = entityGroupName;
        }
        public EntityAssetInfo(string entityGroupName) 
        {
            this.EntityGroupName = entityGroupName;
        }
        public EntityAssetInfo() { }
        public string EntityGroupName { get; set; }
        public bool UseObjectPool { get; set; }
    }
}
