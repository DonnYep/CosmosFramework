namespace Cosmos.Resource
{
    internal class AssetBundleReferenceCounter : ReferenceCounterBase
    {
        ResourceManifest resourceManifest;
        public AssetBundleReferenceCounter(ResourceManifest resourceManifest)
        {
            this.resourceManifest = resourceManifest;
            InitData();
        }
        /// <summary>
        /// 初始化数据；
        /// </summary>
        void InitData()
        {
            var bundleManifest = resourceManifest.BundleManifestDict;
            foreach (var bundle in bundleManifest.Values)
            {
                resourceBundleDict.TryAdd(bundle.BundleName, new ResourceBundleWarpper(bundle));
                var objList = bundle.ResourceObjectList;
                var length = objList.Count;
                for (int i = 0; i < length; i++)
                {
                    var obj = objList[i];
                    resourceObjectDict.TryAdd(obj.AssetName, new ResourceObjectWarpper(obj));
                }
            }
        }
    }
}
