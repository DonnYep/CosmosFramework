using System;
using UnityEngine;
namespace Quark.Asset
{
    [Serializable]
    public struct QuarkBundleInfo : IEquatable<QuarkBundleInfo>
    {
        [SerializeField]
        string assetBundlePath;
        [SerializeField]
        string assetBundleName;
        [SerializeField]
        string assetBundlePathHash;
        public string AssetBundlePath { get { return assetBundlePath; } }
        public string AssetBundleName { get { return assetBundleName; } }
        public string AssetBundlePathHash { get { return assetBundlePathHash; } }
        public QuarkBundleInfo(string assetBundlePath, string assetBundlePathHash, string assetBundleName)
        {
            this.assetBundlePath = assetBundlePath;
            this.assetBundleName = assetBundleName;
            this.assetBundlePathHash = assetBundlePathHash;
        }
        public static bool operator ==(QuarkBundleInfo a, QuarkBundleInfo b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(QuarkBundleInfo a, QuarkBundleInfo b)
        {
            return !a.Equals(b);
        }
        public override bool Equals(object obj)
        {
            return obj is QuarkBundleInfo && Equals((QuarkBundleInfo)obj);
        }
        public override int GetHashCode()
        {
            return AssetBundlePathHash.GetHashCode() ^ AssetBundlePath.GetHashCode()^ AssetBundleName.GetHashCode();
        }
        public override string ToString()
        {
            return $"DirHash: {(string.IsNullOrEmpty(AssetBundlePathHash) == true ? "Null" : AssetBundlePathHash)} ; Dir :{(string.IsNullOrEmpty(AssetBundlePath) == true ? "Null" : AssetBundlePath)}";
        }
        public bool Equals(QuarkBundleInfo other)
        {
            return other.AssetBundlePathHash == this.AssetBundlePathHash &&
                other.AssetBundlePath == this.AssetBundlePath &&
                other.AssetBundleName == this.AssetBundleName;
        }
        public static QuarkBundleInfo None { get { return new QuarkBundleInfo(); } }
    }
}
