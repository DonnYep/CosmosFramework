using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEditor;
namespace Cosmos.Editor.Resource
{
    public class ResourceObjectTreeView : TreeView
    {
        List<ResourceObjectInfo> objectList = new List<ResourceObjectInfo>();
        public ResourceObjectTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
            Reload();
            showAlternatingRowBackgrounds = true;
            showBorder = true;
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
        protected override void ContextClickedItem(int id)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Copy name to clipboard"), false, CopyToClipboard,id);
            menu.ShowAsContext();
        }
        protected override void DoubleClickedItem(int id)
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
                    var objInfo = objectList[i];
                    var obj = AssetDatabase.LoadMainAssetAtPath(objInfo.AssetPath);
                    Texture2D objIcon = null;
                    bool isValidAsset=obj!=null;
                    string itemDisplayName = string.Empty;
                    if (isValidAsset)
                    {
                        objIcon = EditorUtil.ToTexture2D(EditorGUIUtility.ObjectContent(obj, obj.GetType()).image);
                        itemDisplayName = objInfo.AssetPath;
                    }
                    else
                    {
                        objIcon = EditorGUIUtility.FindTexture("console.erroricon");
                        itemDisplayName = $"INVALID [ - ] {objInfo.AssetPath}";
                    }
                    var item = new TreeViewItem { id = i, depth = 1, displayName = itemDisplayName, icon = objIcon };
                    allItems.Add(item);
                }
                SetupParentsAndChildrenFromDepths(root, allItems);
                return root;
            }
        }
        void CopyToClipboard(object context)
        {
            var id = System.Convert.ToInt32(context);
            var path = objectList[id].ObjectName;
            GUIUtility.systemCopyBuffer = path;
        }
    }
}
