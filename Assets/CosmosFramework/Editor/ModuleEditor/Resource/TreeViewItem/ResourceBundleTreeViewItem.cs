using UnityEditor.IMGUI.Controls;
using UnityEngine;
namespace Cosmos.Editor.Resource
{
    public class ResourceBundleTreeViewItem : TreeViewItem
    {
        string bundleSize;
        public ResourceBundleTreeViewItem(int id, int depth, string displayName, Texture2D icon) : base(id, depth, displayName)
        {
            this.icon = icon;
        }
        /// <summary>
        /// 包体大小；
        /// </summary>
        public string BundleSize
        {
            get
            {
                if (string.IsNullOrEmpty(bundleSize))
                    return "<UNKONW>";
                return bundleSize;
            }
            set { bundleSize = value; }
        }
    }
}
