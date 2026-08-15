using System.Collections.Generic;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源包裹清单。描述一个资源包裹内所有资产与包体的映射关系。
    /// </summary>
    [System.Serializable]
    public class PackageManifest
    {
        /// <summary>
        /// 资源包裹名称
        /// </summary>
        public string PackageName;
        /// <summary>
        /// 版本信息
        /// </summary>
        public string PackageVersion;
        /// <summary>
        /// 资源列表
        /// </summary>
        public List<PackageAsset> AssetList = new List<PackageAsset>();
        /// <summary>
        /// 资源包列表
        /// </summary>
        public List<PackageBundle> BundleList = new List<PackageBundle>();

        Dictionary<string, PackageBundle> bundleNameMap;
        Dictionary<string, PackageAsset> assetGuidMap;
        Dictionary<string, PackageAsset> assetPathMap;
        Dictionary<string, List<PackageAsset>> assetTagMap;

        /// <summary>
        /// 索引是否构建完毕
        /// </summary>
        public bool IsIndexBuilt { get; private set; }

        /// <summary>
        /// 构建运行时索引，用于快速寻址。
        /// <para>反序列化后需要调用一次。</para>
        /// </summary>
        public void BuildIndex()
        {
            bundleNameMap = new Dictionary<string, PackageBundle>();
            assetGuidMap = new Dictionary<string, PackageAsset>();
            assetPathMap = new Dictionary<string, PackageAsset>();
            assetTagMap = new Dictionary<string, List<PackageAsset>>();
            var bundleCount = BundleList.Count;
            for (int i = 0; i < bundleCount; i++)
            {
                var bundle = BundleList[i];
                bundleNameMap[bundle.BundleName] = bundle;
                bundle.ParseBundle(this);
            }
            var assetCount = AssetList.Count;
            for (int i = 0; i < assetCount; i++)
            {
                var asset = AssetList[i];
                if (!string.IsNullOrEmpty(asset.AssetGuid))
                    assetGuidMap[asset.AssetGuid] = asset;
                if (!string.IsNullOrEmpty(asset.AssetPath))
                    assetPathMap[asset.AssetPath] = asset;
                if (asset.AssetTags != null && asset.AssetTags.Length > 0)
                {
                    var tagLength = asset.AssetTags.Length;
                    for (int j = 0; j < tagLength; j++)
                    {
                        var tag = asset.AssetTags[j];
                        if (string.IsNullOrEmpty(tag))
                            continue;
                        if (!assetTagMap.TryGetValue(tag, out var list))
                        {
                            list = new List<PackageAsset>();
                            assetTagMap.Add(tag, list);
                        }
                        list.Add(asset);
                    }
                }
            }
            IsIndexBuilt = true;
        }

        /// <summary>
        /// 通过bundle名获取包体信息
        /// </summary>
        public PackageBundle GetBundle(string bundleName)
        {
            if (string.IsNullOrEmpty(bundleName))
                return null;
            if (!IsIndexBuilt)
                BuildIndex();
            bundleNameMap.TryGetValue(bundleName, out var bundle);
            return bundle;
        }

        /// <summary>
        /// 通过资源GUID获取资产信息
        /// </summary>
        public PackageAsset GetAssetByGuid(string assetGuid)
        {
            if (string.IsNullOrEmpty(assetGuid))
                return null;
            if (!IsIndexBuilt)
                BuildIndex();
            assetGuidMap.TryGetValue(assetGuid, out var asset);
            return asset;
        }

        /// <summary>
        /// 通过资源路径获取资产信息
        /// </summary>
        public PackageAsset GetAssetByPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
                return null;
            if (!IsIndexBuilt)
                BuildIndex();
            assetPathMap.TryGetValue(assetPath, out var asset);
            return asset;
        }

        /// <summary>
        /// 通过分类标签获取资产信息集合
        /// </summary>
        public List<PackageAsset> GetAssetsByTag(string tag)
        {
            if (!IsIndexBuilt)
                BuildIndex();
            if (string.IsNullOrEmpty(tag))
                return null;
            assetTagMap.TryGetValue(tag, out var list);
            return list;
        }

        /// <summary>
        /// 序列化文件清单
        /// </summary>
        /// <param name="aesKey">加密密钥，可为空</param>
        /// <returns>序列化后的字符串</returns>
        public string Serialize(string aesKey)
        {
            return ResourceUtility.SerializeManifest(this, aesKey);
        }

        /// <summary>
        /// 反序列化文件清单
        /// </summary>
        /// <param name="context">清单字符串</param>
        /// <param name="aesKey">加密密钥，可为空</param>
        /// <returns>反序列化后的清单</returns>
        public static PackageManifest Deserialize(string context, string aesKey)
        {
            return ResourceUtility.DeserializeManifest(context, aesKey);
        }
    }
}
