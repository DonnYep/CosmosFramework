using System.Collections.Generic;

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
    }
}
