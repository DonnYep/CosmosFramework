using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.Resource
{
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
        /// extract all files from the folder as an individual assetbundle.
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
                foreach (AssetEntry entry in AssetEntries)
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
                try
                {
                    assetEntry.Parent = this;
                    assetEntryDict.Add(assetEntry.Guid, assetEntry);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
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
    }
}
