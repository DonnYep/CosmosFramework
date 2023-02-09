using UnityEditor.IMGUI.Controls;
using UnityEngine;
namespace Cosmos.Editor.Resource
{
    public class AssetDatabaseBundleTreeViewItem : TreeViewItem
    {
        string bundleFormatSize;
        int objectCount;
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
                    return ResourceEditorConstant.UNKONM;
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
    }
}
