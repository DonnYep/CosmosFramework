using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using Cosmos.Resource;
using UnityEngine;
namespace Cosmos.Editor.Resource
{
    public class ResourceObjectTreeView : TreeView
    {
        List<ResourceObject> objectList = new List<ResourceObject>();
        public override void OnGUI(Rect rect)
        {
            Reload();
            base.OnGUI(rect);
        }
        public ResourceObjectTreeView(TreeViewState state) : base(state)
        {
            showAlternatingRowBackgrounds = true;
        }
        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            EditorUtil.PingAndActiveObject(objectList[id].AssetPath);
        }
        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = -1, depth = -1, displayName = "Root" };
            var allItems = new List<TreeViewItem>();
            {
                for (int i = 0; i < objectList.Count; i++)
                {
                    var item = new TreeViewItem { id = i, depth = 1, displayName = objectList[i].AssetPath};
                    allItems.Add(item);
                }
                SetupParentsAndChildrenFromDepths(root, allItems);
                return root;
            }
        }
    }
}
