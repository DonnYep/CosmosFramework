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
            foreach (var manifest in bundleManifest.Values)
            {
                resourceBundleDict.TryAdd(manifest.BundleName, new ResourceBundleWarpper(manifest.ResourceBundle));
                var objList = manifest.ResourceBundle.ResourceObjectList;
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
