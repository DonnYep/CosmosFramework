using UnityEditor.IMGUI.Controls;
using UnityEngine;
namespace Cosmos.Editor.Resource
{
    public class AssetDatabaseBundleTreeViewItem : TreeViewItem
    {
        string bundleFormatSize;
        int objectCount;
        int splittedBundleCount;
        public AssetDatabaseBundleTreeViewItem(int id, int depth, string displayName, Texture2D icon) : base(id, depth, displayName)
        {
            this.icon = icon;
        }
        /// <summary>
        /// 包体大小；
        /// </summary>
        public string BundleFormatSize
        {
            get
            {
                if (string.IsNullOrEmpty(bundleFormatSize))
                    return Constants.UNKONM;
                return bundleFormatSize;
            }
            set { bundleFormatSize = value; }
        }
        public int ObjectCount
        {
            get { return objectCount; }
            set
            {
                objectCount = value;
                if (objectCount <= 0)
                    objectCount = 0;
            }
        }
        public int SplitBundleCount
        {
            get { return splittedBundleCount; }
            set
            {
                splittedBundleCount = value;
                if (splittedBundleCount <= 0)
                    splittedBundleCount = 0;
            }
        }
        public Texture2D SplitIcon { get; set; }
        public Texture2D NotSplitIcon { get; set; }
        /// <summary>
        /// split the directory into multiple subdirectories
        /// </summary>
        public bool Split{ get; set; }
        public Texture2D ExtractIcon { get; set; }
        public Texture2D NotExtractIcon { get; set; }
        /// <summary>
        /// extract all files from the folder as an individual assetbundle
        /// </summary>
        public bool Extract{ get; set; }
    }
}
