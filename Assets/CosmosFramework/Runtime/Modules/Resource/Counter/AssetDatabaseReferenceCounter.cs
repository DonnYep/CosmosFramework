namespace Cosmos.Resource
{
    internal class AssetDatabaseReferenceCounter:ReferenceCounterBase
    {
        ResourceDataset resourceDataset;
        public AssetDatabaseReferenceCounter(ResourceDataset resourceDataset)
        {
            this.resourceDataset = resourceDataset;
            InitData();
        }
        /// <summary>
        /// 初始化数据；
        /// </summary>
        void InitData()
        {
            var objList = resourceDataset.ResourceObjectList;
            var length = objList.Count;
            for (int i = 0; i < length; i++)
            {
                var resObj = objList[i];
                resourceObjectDict.TryAdd(resObj.AssetName, new ResourceObjectWarpper(resObj));
            }
            var bundles= resourceDataset.ResourceBundleList;
            foreach (var bundle in bundles)
            {
                resourceBundleDict.Add(bundle.BundleName, new ResourceBundleWarpper(bundle));
            }
        }
    }
}
