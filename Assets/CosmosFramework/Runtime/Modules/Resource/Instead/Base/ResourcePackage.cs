namespace Cosmos.Resource
{
    /// <summary>
    ///  资源包裹，一个资源包含多个PackageBundle
    /// </summary>
    public class ResourcePackage
    {
        readonly string packageName;
        public string PackageName
        {
            get { return packageName; }
        }
        private ResourcePackage()
        {
        }
        internal ResourcePackage(string packageName)
        {
            this.packageName = packageName;
        }

        public void UnloadAsset(string asset)
        {
            //UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(null);
        }
        public AssetHandle<T> LoadAssetAsync<T>(string asset)
            where T : UnityEngine.Object
        {
            var assetInfo = GetAssetInfo(asset);
            AssetHandle<T> handle = default;
            switch (CFResources.ResourceLoadMode)
            {
                case CFResourceLoadMode.AssetDatabase:
                    {
                        var loader = new DatabaseAssetProvider(assetInfo);
                        handle = loader.CreateHandle<AssetHandle<T>, T>();
                    }
                    break;
                case CFResourceLoadMode.AssetBundle:
                    break;
            }

            return handle;
        }
        AssetInfo GetAssetInfo(string asset)
        {
            //如何获得寻址资源
            return default;

        }
    }
}
