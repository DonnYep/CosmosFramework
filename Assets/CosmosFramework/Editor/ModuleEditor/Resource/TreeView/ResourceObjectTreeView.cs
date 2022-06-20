using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
namespace Cosmos.Editor.Resource
{
    public class ResourceObjectTreeView : TreeView
    {
        List<ResourceObjectInfo> objectList = new List<ResourceObjectInfo>();

        public ResourceObjectTreeView(TreeViewState state) : base(state)
        {
            Reload();
            showAlternatingRowBackgrounds = true;
        }
        public void AddObject(ResourceObjectInfo objectInfo)
        {
            if (!objectList.Contains(objectInfo))
            {
                objectList.Add(objectInfo);
                Reload();
            }
        }
        public void RemoveObject(ResourceObjectInfo objectInfo)
        {
            if (objectList.Contains(objectInfo))
            {
                objectList.Remove(objectInfo);
                Reload();
            }
        }
        public void Clear()
        {
            objectList.Clear();
            Reload();
        }
        protected override void DoubleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            EditorUtil.PingAndActiveObject(objectList[id].AssetPath);
        }
        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = -1, depth = -1, displayName = "Root" };
            var assetIcon = ResourceWindowUtil.GetFolderIcon();
            var sceneIcon = ResourceWindowUtil.GetSceneIcon();
            var allItems = new List<TreeViewItem>();
            {
                for (int i = 0; i < objectList.Count; i++)
                {
                    Texture2D objIcon;
                    var obj = objectList[i];
                    if (obj.AssetPath.EndsWith(".unity"))
                        objIcon = sceneIcon;
                    else objIcon = assetIcon;
                    var item = new TreeViewItem { id = i, depth = 1, displayName = obj.AssetPath, icon = objIcon };
                    allItems.Add(item);
                }
                SetupParentsAndChildrenFromDepths(root, allItems);
                return root;
            }
        }
    }
}
