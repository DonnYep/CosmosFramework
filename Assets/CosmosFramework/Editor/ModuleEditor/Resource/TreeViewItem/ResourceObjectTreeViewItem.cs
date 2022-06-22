using UnityEditor.IMGUI.Controls;
using UnityEngine;
namespace Cosmos.Editor.Resource
{
    public class ResourceObjectTreeViewItem : TreeViewItem
    {
        public ResourceObjectTreeViewItem(int id, int depth, string displayName,Texture2D icon) : base(id, depth, displayName)
        {
            this.icon = icon;
        }
        public string ObjectName { get; set; }
        /// <summary>
        /// 当前对象状态，INVALID , VALID
        /// </summary>
        public string ObjectState{ get; set; }
        public Texture2D ObjectStateIcon{ get; set; }
    }
}
