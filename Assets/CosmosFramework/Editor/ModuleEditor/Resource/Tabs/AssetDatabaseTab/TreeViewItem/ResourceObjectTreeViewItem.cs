using UnityEditor.IMGUI.Controls;
using UnityEngine;
namespace Cosmos.Editor.Resource
{
    public class ResourceObjectTreeViewItem : TreeViewItem
    {
        string objectSize;
        public ResourceObjectTreeViewItem(int id, int depth, string displayName, Texture2D icon) : base(id, depth, displayName)
        {
            this.icon = icon;
        }
        public string ObjectName { get; set; }
        /// <summary>
        /// 当前对象状态，INVALID , VALID
        /// </summary>
        public string ObjectState { get; set; }
        /// <summary>
        /// 文件大小；
        /// </summary>
        public string ObjectSize
        {
            get
            {
                if (string.IsNullOrEmpty(objectSize))
                    return "<NULL>";
                return objectSize;
            }
            set { objectSize = value; }
        }
        /// <summary>
        /// AB包名；
        /// </summary>
        public string ObjectBundleName { get; set; }
        /// <summary>
        /// 对象后缀名
        /// </summary>
        public string ObjectExtension { get; set; }
    }
}
