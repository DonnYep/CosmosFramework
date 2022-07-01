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
        public string AssetBundlePath { get { return assetBundlePath; } }
        public string AssetBundleName { get { return assetBundleName; } }
        public QuarkBundleInfo(string assetBundlePath,  string assetBundleName)
        {
            this.assetBundlePath = assetBundlePath;
            this.assetBundleName = assetBundleName;
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
            return  AssetBundlePath.GetHashCode()^ AssetBundleName.GetHashCode();
        }
        public override string ToString()
        {
            return $"AssetBundleName: {(string.IsNullOrEmpty(AssetBundleName) == true ? "Null" : AssetBundleName)} ; AssetBundlePath :{(string.IsNullOrEmpty(AssetBundlePath) == true ? "Null" : AssetBundlePath)}";
        }
        public bool Equals(QuarkBundleInfo other)
        {
            return other.AssetBundlePath == this.AssetBundlePath &&
                other.AssetBundleName == this.AssetBundleName;
        }
        public static QuarkBundleInfo None { get { return new QuarkBundleInfo(); } }
    }
}
