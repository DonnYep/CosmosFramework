using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Resource
{
    /// <summary>
    /// ResourceObject 计数器；
    /// </summary>
    internal class ResourceObjectCounter
    {
        Dictionary<int, ResourceObject> resObjDict = new Dictionary<int, ResourceObject>();
        public void OnLoad(AssetInfo info)
        {

        }
        public void OnUnload(AssetInfo info)
        {

        }
        ResourceObject GetResouceObject(AssetInfo info)
        {
            return new ResourceObject(info.AssetPath, info.AssetBundleName, 0);
        }
    }
}
