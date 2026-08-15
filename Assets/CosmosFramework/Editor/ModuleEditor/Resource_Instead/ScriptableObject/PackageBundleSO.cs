using Cosmos.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Cosmos.Editor.Resource
{
    /// <summary>
    /// Editor模式运行时资产组，包含被加载的资产信息
    /// </summary>
    [CreateAssetMenu(fileName = "PackageBundleSO_new", menuName = "Cosmos/PackageBundleSO")]
    public class PackageBundleSO : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 资源包名
        /// </summary>
        public string BundleName;
        [SerializeField]
        public List<AssetEntry> AssetEntries = new List<AssetEntry>();
        Dictionary<string, AssetEntry> assetEntryDict = new Dictionary<string, AssetEntry>();
        [SerializeField]
        bool packSeparately;
        /// <summary>
        /// 每个资产是否拆分为独立ab包
        /// </summary>
        public bool PackSeparately
        {
            get { return packSeparately; }
            set { packSeparately = value; }
        }

        internal ICollection<AssetEntry> assetEntries
        {
            get { return assetEntryDict.Values; }
        }
        /// <summary>
        /// 所属包裹的文件hash值
        /// </summary>
        public string ParentPackageHash;
        public void OnBeforeSerialize()
        {
            if (AssetEntries == null)
            {
                AssetEntries = new List<AssetEntry>();
                foreach (AssetEntry entry in assetEntries)
                {
                    AssetEntries.Add(entry);
                }
            }
        }
        public void OnAfterDeserialize()
        {
            ResetEntryMap();
        }
        internal void ResetEntryMap()
        {
            assetEntryDict.Clear();
            foreach (var assetEntry in AssetEntries)
            {
                assetEntryDict[assetEntry.Guid] = assetEntry;
            }
        }
        internal void AddAssetEntry(AssetEntry entry)
        {
            assetEntryDict[entry.Guid] = entry;
        }
        /// <summary>
        /// 获取包内全部资产条目
        /// </summary>
        public List<AssetEntry> GetAssetEntries()
        {
            return AssetEntries;
        }
        /// <summary>
        /// 获取解析后的包体名
        /// </summary>
        public string GetResolvedBundleName()
        {
            if (!string.IsNullOrEmpty(BundleName))
                return BundleName;
            return name;
        }
        /// <summary>
        /// 为包内所有资产设置AssetBundleName
        /// </summary>
        public void AssignBundleNameToAssets()
        {
            var bundleName = GetResolvedBundleName();
            var entries = AssetEntries;
            var count = entries.Count;
            for (int i = 0; i < count; i++)
            {
                var entry = entries[i];
                var assetPath = AssetDatabase.GUIDToAssetPath(entry.Guid);
                if (string.IsNullOrEmpty(assetPath))
                    continue;
                var importer = AssetImporter.GetAtPath(assetPath);
                if (importer == null)
                    continue;
                var finalBundleName = PackSeparately ? $"{bundleName}_{ResourceUtility.FilterName(Path.GetFileNameWithoutExtension(assetPath))}" : bundleName;
                if (importer.assetBundleName != finalBundleName)
                    importer.assetBundleName = finalBundleName;
            }
        }
        /// <summary>
        /// 清除包内所有资产的AssetBundleName
        /// </summary>
        public void ClearBundleNameOnAssets()
        {
            var entries = AssetEntries;
            var count = entries.Count;
            for (int i = 0; i < count; i++)
            {
                var entry = entries[i];
                var assetPath = AssetDatabase.GUIDToAssetPath(entry.Guid);
                if (string.IsNullOrEmpty(assetPath))
                    continue;
                var importer = AssetImporter.GetAtPath(assetPath);
                if (importer == null)
                    continue;
                if (!string.IsNullOrEmpty(importer.assetBundleName))
                    importer.assetBundleName = string.Empty;
            }
        }
        public AssetEntry GetAssetEntry(string guid)
        {
            assetEntryDict.TryGetValue(guid, out var assetEntry);
            return assetEntry;
        }
        public void RemoveAssetEntry(AssetEntry entry)
        {
            assetEntryDict.Remove(entry.Guid);
        }
        public void RemoveAssetEntry(string guid)
        {
            assetEntryDict.Remove(guid);
        }
    }
}
