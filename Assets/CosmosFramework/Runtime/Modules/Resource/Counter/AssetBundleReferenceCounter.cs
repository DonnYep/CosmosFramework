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
            var bundleBuildInfoDict = resourceManifest.ResourceBundleBuildInfoDict;
            foreach (var bundleBuildInfo in bundleBuildInfoDict.Values)
            {
                resourceBundleDict.TryAdd(bundleBuildInfo.ResourceBundle.BundleName, new ResourceBundleWarpper(bundleBuildInfo.ResourceBundle));
                var objList = bundleBuildInfo.ResourceBundle.ResourceObjectList;
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
