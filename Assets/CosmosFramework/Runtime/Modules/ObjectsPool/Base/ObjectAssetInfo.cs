using Cosmos.ObjectPool;
using Cosmos.Resource;
using System;

namespace Cosmos
{
    public class ObjectAssetInfo : AssetInfo
    {
        public ObjectPoolKey ObjectKey { get; private set; }
        public ObjectAssetInfo(string objectName,string assetPath) :base(assetPath)
        {
            this.ObjectKey = new ObjectPoolKey(typeof(object), objectName);
        }
        public ObjectAssetInfo(Type objectType, string objectName, string assetPath) : base(assetPath)
        {
            this.ObjectKey = new ObjectPoolKey(objectType, objectName);
        }
        public ObjectAssetInfo(string objectName, string assetBundleName, string assetPath)
            : base(assetBundleName, assetPath)
        {
            this.ObjectKey = new ObjectPoolKey(typeof(object), objectName);
        }
        public ObjectAssetInfo(Type objectType, string objectName, string assetBundleName, string assetPath)
             : base(assetBundleName, assetPath)
        {
            this.ObjectKey = new ObjectPoolKey(objectType, objectName);
        }
    }
}
