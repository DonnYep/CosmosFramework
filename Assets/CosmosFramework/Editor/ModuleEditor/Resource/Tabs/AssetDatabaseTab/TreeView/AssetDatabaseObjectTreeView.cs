using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEditor;
using Cosmos.Resource;

namespace Cosmos.Editor.Resource
{
    public class AssetDatabaseObjectTreeView : TreeView
    {
        List<ResourceObjectInfo> objectInfoList = new List<ResourceObjectInfo>();
        public AssetDatabaseObjectTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
            Reload();
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            multiColumnHeader.sortingChanged += OnMultiColumnHeaderSortingChanged;
        }
        public void AddObject(ResourceObjectInfo objectInfo)
        {
            if (!objectInfoList.Contains(objectInfo))
            {
                objectInfoList.Add(objectInfo);
            }
        }
        public void Clear()
        {
            objectInfoList.Clear();
            Reload();
        }
        protected override void ContextClickedItem(int id)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Copy object name to clipboard"), false, CopyNameToClipboard, id);
            menu.AddItem(new GUIContent("Copy object path to clipboard"), false, CopyAssetPathToClipboard, id);
            menu.ShowAsContext();
        }
        protected override void DoubleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            EditorUtil.PingAndActiveObject(objectInfoList[id].ObjectPath);
        }
        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = -1, depth = -1, displayName = "Root" };
            var allItems = new List<TreeViewItem>();
            {
                Texture2D validIcon = ResourceWindowUtility.GetValidIcon();
                Texture2D invalidIcon = ResourceWindowUtility.GetInvalidIcon();
                for (int i = 0; i < objectInfoList.Count; i++)
                {
                    var objectInfo = objectInfoList[i];
                    Texture2D objectIcon = null;
                    Texture2D objectValidIcon = null;
                    var validState = string.Empty;
                    if (objectInfo.ObjectVaild)
                    {
                        objectIcon = AssetDatabase.GetCachedIcon(objectInfo.ObjectPath) as Texture2D;
                        validState = ResourceEditorConstant.VALID;
                        objectValidIcon = validIcon;
                    }
                    else
                    {
                        objectIcon = EditorGUIUtility.FindTexture("DefaultAsset Icon");
                        validState = ResourceEditorConstant.INVALID;
                        objectValidIcon = invalidIcon;
                    }
                    var treeViewItem = new AssetDatabaseObjectTreeViewItem(i, 1, objectInfo.ObjectPath, objectIcon)
                    {
                        ObjectName = objectInfo.ObjectName,
                        ObjectState = validState,
                        ObjectSize = objectInfo.ObjectFormatBytes,
                        ObjectBundleName = objectInfo.BundleName,
                        ObjectExtension = objectInfo.Extension,
                        ObjectValid = objectInfo.ObjectVaild,
                        ObjectValidIcon = objectValidIcon
                    };
                    allItems.Add(treeViewItem);
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
                DrawCellGUI(args.GetCellRect(i), args.item as AssetDatabaseObjectTreeViewItem, args.GetColumn(i), ref args);
            }
        }
        void OnMultiColumnHeaderSortingChanged(MultiColumnHeader multiColumnHeader)
        {
            var sortedColumns = multiColumnHeader.state.sortedColumns;
            if (sortedColumns.Length == 0)
                return;
            var sortedType = sortedColumns[0];
            var ascending = multiColumnHeader.IsSortedAscending(sortedType);
            switch (sortedType)
            {
                case 0:
                    {
                        //Name
                        if (ascending)
                            objectInfoList.Sort((lhs, rhs) => lhs.ObjectName.CompareTo(rhs.ObjectName));
                        else
                            objectInfoList.Sort((lhs, rhs) => rhs.ObjectName.CompareTo(lhs.ObjectName));
                    }
                    break;
                case 1:
                    {
                        //Extension
                        if (ascending)
                            objectInfoList.Sort((lhs, rhs) => lhs.Extension.CompareTo(rhs.Extension));
                        else
                            objectInfoList.Sort((lhs, rhs) => rhs.Extension.CompareTo(lhs.Extension));
                    }
                    break;
                case 2:
                    {
                        //State
                        if (ascending)
                            objectInfoList.Sort((lhs, rhs) => lhs.ObjectVaild.CompareTo(rhs.ObjectVaild));
                        else
                            objectInfoList.Sort((lhs, rhs) => rhs.ObjectVaild.CompareTo(lhs.ObjectVaild));
                    }
                    break;
                case 3:
                    {
                        //Size
                        if (ascending)
                            objectInfoList.Sort((lhs, rhs) => lhs.ObjectSize.CompareTo(rhs.ObjectSize));
                        else
                            objectInfoList.Sort((lhs, rhs) => rhs.ObjectSize.CompareTo(lhs.ObjectSize));
                    }
                    break;
                case 4:
                    {
                        //AssetBundle
                        if (ascending)
                            objectInfoList.Sort((lhs, rhs) => lhs.BundleName.CompareTo(rhs.BundleName));
                        else
                            objectInfoList.Sort((lhs, rhs) => rhs.BundleName.CompareTo(lhs.BundleName));
                    }
                    break;
                case 5:
                    {
                        //AssetPath
                        if (ascending)
                            objectInfoList.Sort((lhs, rhs) => lhs.ObjectPath.CompareTo(rhs.ObjectPath));
                        else
                            objectInfoList.Sort((lhs, rhs) => rhs.ObjectPath.CompareTo(lhs.ObjectPath));
                    }
                    break;
            }
            Reload();
        }
        void DrawCellGUI(Rect cellRect, AssetDatabaseObjectTreeViewItem treeView, int column, ref RowGUIArgs args)
        {
            switch (column)
            {
                case 0:
                    {
                        var iconRect = new Rect(cellRect.x + 4, cellRect.y, cellRect.height, cellRect.height);
                        if (treeView.icon != null)
                            GUI.DrawTexture(iconRect, treeView.icon, ScaleMode.ScaleToFit);
                        var labelCellRect = new Rect(cellRect.x + iconRect.width + 4, cellRect.y, cellRect.width - iconRect.width, cellRect.height);
                        DefaultGUI.Label(labelCellRect, treeView.ObjectName, args.selected, args.focused);
                    }
                    break;
                case 1:
                    {
                        DefaultGUI.Label(cellRect, treeView.ObjectExtension, args.selected, args.focused);
                    }
                    break;
                case 2:
                    {
                        var iconRect = new Rect(cellRect.x, cellRect.y, cellRect.height, cellRect.height);
                        GUI.DrawTexture(iconRect, treeView.ObjectValidIcon, ScaleMode.ScaleToFit);
                        //var labelCellRect = new Rect(cellRect.x + iconRect.width + 2, cellRect.y, cellRect.width - iconRect.width, cellRect.height);
                        //GUIStyle objectStateStyle = new GUIStyle();
                        //objectStateStyle.fontStyle = FontStyle.Bold;
                        //DefaultGUI.Label(labelCellRect, treeView.ObjectState, args.selected, args.focused);
                        //var valid = treeView.ObjectValid;
                        //if (!valid)
                        //    objectStateStyle.normal.textColor = Color.red;
                        //else
                        //    objectStateStyle.normal.textColor = Color.green;
                        //GUI.Label(labelCellRect, treeView.ObjectState, objectStateStyle);
                    }
                    break;
                case 3:
                    {
                        DefaultGUI.Label(cellRect, treeView.ObjectSize, args.selected, args.focused);
                    }
                    break;
                case 4:
                    {
                        DefaultGUI.Label(cellRect, treeView.ObjectBundleName, args.selected, args.focused);
                    }
                    break;
                case 5:
                    {
                        DefaultGUI.Label(cellRect, treeView.displayName, args.selected, args.focused);
                    }
                    break;
            }
        }
        void CopyNameToClipboard(object context)
        {
            var id = System.Convert.ToInt32(context);
            var path = objectInfoList[id].ObjectName;
            GUIUtility.systemCopyBuffer = path;
        }
        void CopyAssetPathToClipboard(object context)
        {
            var id = System.Convert.ToInt32(context);
            var path = objectInfoList[id].ObjectPath;
            GUIUtility.systemCopyBuffer = path;
        }
    }
}
