using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public class ResourceBundleTreeView : TreeView
    {
        public ResourceBundleTreeView(TreeViewState state) : base(state)
        {
            showAlternatingRowBackgrounds = true;
        }
        List<ResourceBundleInfo> bundleList = new List<ResourceBundleInfo>();
        public override void OnGUI(Rect rect)
        {
            Reload();
            base.OnGUI(rect);
        }
        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            EditorUtil.PingAndActiveObject(bundleList[id].BundlePath);
        }
        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = -1, depth = -1, displayName = "Root" };
            var allItems = new List<TreeViewItem>();
            {
                for (int i = 0; i < bundleList.Count; i++)
                {
                    var item = new TreeViewItem { id = i, depth = 1, displayName = bundleList[i].BundleName };
                    allItems.Add(item);
                }
                SetupParentsAndChildrenFromDepths(root, allItems);
                return root;
            }
        }
    }
}
