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
            menu.AddItem(new GUIContent("Copy name to clipboard"), false, CopyNameToClipboard, id);
            menu.AddItem(new GUIContent("Copy asset path to clipboard"), false, CopyAssetPathToClipboard, id);
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
                    var objectInfo = objectList[i];
                    var obj = AssetDatabase.LoadMainAssetAtPath(objectInfo.AssetPath);
                    Texture2D objectIcon = null;
                    Texture2D stateIcon = null;
                    bool isValidAsset = obj != null;
                    string objectState = string.Empty;
                    if (isValidAsset)
                    {
                        objectIcon = EditorUtil.ToTexture2D(EditorGUIUtility.ObjectContent(obj, obj.GetType()).image);
                        objectState = "VALID";
                        stateIcon = ResourceEditorUtility.GetAssetValidIcon();
                    }
                    else
                    {
                        objectIcon = EditorGUIUtility.FindTexture("console.erroricon");
                        objectState = "INVALID";
                        stateIcon = ResourceEditorUtility.GetAssetInvalidIcon();
                    }
                    var item = new ResourceObjectTreeViewItem(i, 1, objectInfo.AssetPath, objectIcon)
                    {
                        ObjectName = objectInfo.ObjectName,
                        ObjectState = objectState,
                        ObjectStateIcon = stateIcon,
                        ObjectSize = objectInfo.FileSize,
                        ObjectBundleName=objectInfo.AssetBundleName
                    };
                    allItems.Add(item);
                }
                SetupParentsAndChildrenFromDepths(root, allItems);
                return root;
            }
        }
        protected override void RowGUI(RowGUIArgs args)
        {
            var length = args.GetNumVisibleColumns();
            for (int i = 0; i < length; i++)
            {
                DrawCellGUI(args.GetCellRect(i), args.item as ResourceObjectTreeViewItem, args.GetColumn(i), ref args);
            }
        }
        void DrawCellGUI(Rect cellRect, ResourceObjectTreeViewItem treeView, int column, ref RowGUIArgs args)
        {
            switch (column)
            {
                case 0:
                    {
                        var iconRect = new Rect(cellRect.x + 4, cellRect.y, cellRect.height, cellRect.height);
                        if (treeView.icon != null)
                            GUI.DrawTexture(iconRect, treeView.icon, ScaleMode.ScaleToFit);
                        var lablCellRect = new Rect(cellRect.x + iconRect.width + 4, cellRect.y, cellRect.width - iconRect.width, cellRect.height);
                        DefaultGUI.Label(lablCellRect, treeView.ObjectName, args.selected, args.focused);
                    }
                    break;
                case 1:
                    {
                        var iconRect = new Rect(cellRect.x, cellRect.y, cellRect.height, cellRect.height);
                        if (treeView.icon != null)
                            GUI.DrawTexture(iconRect, treeView.ObjectStateIcon, ScaleMode.ScaleToFit);
                        var lablCellRect = new Rect(cellRect.x + iconRect.width + 2, cellRect.y, cellRect.width - iconRect.width, cellRect.height);

                        DefaultGUI.Label(lablCellRect, treeView.ObjectState, args.selected, args.focused);
                    }
                    break;
                case 2:
                    {
                        DefaultGUI.Label(cellRect, treeView.ObjectSize, args.selected, args.focused);
                    }
                    break;
                case 3:
                    {
                        DefaultGUI.Label(cellRect, treeView.ObjectBundleName, args.selected, args.focused);
                    }
                    break;
                case 4:
                    {
                        DefaultGUI.Label(cellRect, treeView.displayName, args.selected, args.focused);
                    }
                    break;
            }
        }
        void CopyNameToClipboard(object context)
        {
            var id = System.Convert.ToInt32(context);
            var path = objectList[id].ObjectName;
            GUIUtility.systemCopyBuffer = path;
        }
        void CopyAssetPathToClipboard(object context)
        {
            var id = System.Convert.ToInt32(context);
            var path = objectList[id].AssetPath;
            GUIUtility.systemCopyBuffer = path;
        }
    }
}
