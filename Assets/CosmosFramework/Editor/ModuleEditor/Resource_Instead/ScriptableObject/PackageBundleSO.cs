using System;
using System.Collections.Generic;
using UnityEngine;

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
