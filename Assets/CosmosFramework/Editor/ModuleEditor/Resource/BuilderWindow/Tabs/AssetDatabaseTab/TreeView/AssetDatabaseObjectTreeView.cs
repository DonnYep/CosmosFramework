using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEditor;
using Cosmos.Resource;
using System;

namespace Cosmos.Editor.Resource
{
    public class AssetDatabaseObjectTreeView : TreeView
    {
        readonly List<ResourceObjectInfo> objectInfoList = new List<ResourceObjectInfo>();
        readonly List<ResourceObjectInfo> selectedObjectInfos = new List<ResourceObjectInfo>();
        public Action<List<ResourceObjectInfo>> onObjectInfoSelectionChanged;
        public int ObjectCount { get { return objectInfoList.Count; } }
        public int SelectedCount { get { return selectedObjectInfos.Count; } }
        GUIStyle invalidStyle;
        public AssetDatabaseObjectTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
            Reload();
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            multiColumnHeader.sortingChanged += OnMultiColumnHeaderSortingChanged;
            invalidStyle = new GUIStyle();
            invalidStyle.normal.textColor = Color.gray;
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
            selectedObjectInfos.Clear();
            onObjectInfoSelectionChanged?.Invoke(selectedObjectInfos);
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
        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            EditorUtil.ActiveObject(objectInfoList[id].ObjectPath);
        }
        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
            selectedObjectInfos.Clear();
            var length = selectedIds.Count;
            for (int i = 0; i < length; i++)
            {
                var idx = selectedIds[i];
                selectedObjectInfos.Add(objectInfoList[idx]);
            }
            onObjectInfoSelectionChanged?.Invoke(selectedObjectInfos);
        }
        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = -1, depth = -1, displayName = "Root" };
            var allItems = new List<TreeViewItem>();
            {
                Texture2D validIcon = ResourceEditorUtility.GetValidIcon();
                Texture2D invalidIcon = ResourceEditorUtility.GetInvalidIcon();
                for (int i = 0; i < objectInfoList.Count; i++)
                {
                    var objectInfo = objectInfoList[i];
                    Texture2D objectIcon = null;
                    Texture2D objectValidIcon = null;
                    var validState = string.Empty;
                    if (objectInfo.ObjectVaild)
                    {
                        objectIcon = AssetDatabase.GetCachedIcon(objectInfo.ObjectPath) as Texture2D;
                        validState = ResourceBuilderWindowConstant.VALID;
                        objectValidIcon = validIcon;
                    }
                    else
                    {
                        objectIcon = EditorGUIUtility.FindTexture("DefaultAsset Icon");
                        validState = ResourceBuilderWindowConstant.INVALID;
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
        void DrawCellGUI(Rect cellRect, AssetDatabaseObjectTreeViewItem treeView, int column, ref RowGUIArgs args)
        {
            switch (column)
            {
                case 0:
                    {
                        var iconRect = new Rect(cellRect.x + 8, cellRect.y, cellRect.height, cellRect.height);
                        if (treeView.icon != null)
                            GUI.DrawTexture(iconRect, treeView.icon, ScaleMode.ScaleToFit);
                    }
                    break;
                case 1:
                    {
                        if (!treeView.ObjectValid)
                            GUI.Label(cellRect, treeView.ObjectName, invalidStyle);
                        else
                            DefaultGUI.Label(cellRect, treeView.ObjectName, args.selected, args.focused);
                    }
                    break;
                case 2:
                    {
                        if (!treeView.ObjectValid)
                            GUI.Label(cellRect, treeView.ObjectExtension, invalidStyle);
                        else
                            DefaultGUI.Label(cellRect, treeView.ObjectExtension, args.selected, args.focused);
                    }
                    break;
                case 3:
                    {
                        var iconRect = new Rect(cellRect.x, cellRect.y, cellRect.height, cellRect.height);
                        GUI.DrawTexture(iconRect, treeView.ObjectValidIcon, ScaleMode.ScaleToFit);
                    }
                    break;
                case 4:
                    {
                        if (!treeView.ObjectValid)
                            GUI.Label(cellRect, treeView.ObjectSize, invalidStyle);
                        else
                            DefaultGUI.Label(cellRect, treeView.ObjectSize, args.selected, args.focused);
                    }
                    break;
                case 5:
                    {
                        if (!treeView.ObjectValid)
                            GUI.Label(cellRect, treeView.ObjectBundleName, invalidStyle);
                        else
                            DefaultGUI.Label(cellRect, treeView.ObjectBundleName, args.selected, args.focused);
                    }
                    break;
                case 6:
                    {
                        if (!treeView.ObjectValid)
                            GUI.Label(cellRect, treeView.displayName, invalidStyle);
                        else
                            DefaultGUI.Label(cellRect, treeView.displayName, args.selected, args.focused);
                    }
                    break;
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
                case 0://Icon
                    break;
                case 1:
                    {
                        //Name
                        if (ascending)
                            objectInfoList.Sort((lhs, rhs) => lhs.ObjectName.CompareTo(rhs.ObjectName));
                        else
                            objectInfoList.Sort((lhs, rhs) => rhs.ObjectName.CompareTo(lhs.ObjectName));
                    }
                    break;
                case 2:
                    {
                        //Extension
                        if (ascending)
                            objectInfoList.Sort((lhs, rhs) => lhs.Extension.CompareTo(rhs.Extension));
                        else
                            objectInfoList.Sort((lhs, rhs) => rhs.Extension.CompareTo(lhs.Extension));
                    }
                    break;
                case 3:
                    {
                        //State
                        if (ascending)
                            objectInfoList.Sort((lhs, rhs) => lhs.ObjectVaild.CompareTo(rhs.ObjectVaild));
                        else
                            objectInfoList.Sort((lhs, rhs) => rhs.ObjectVaild.CompareTo(lhs.ObjectVaild));
                    }
                    break;
                case 4:
                    {
                        //Size
                        if (ascending)
                            objectInfoList.Sort((lhs, rhs) => lhs.ObjectSize.CompareTo(rhs.ObjectSize));
                        else
                            objectInfoList.Sort((lhs, rhs) => rhs.ObjectSize.CompareTo(lhs.ObjectSize));
                    }
                    break;
                case 5:
                    {
                        //AssetBundle
                        if (ascending)
                            objectInfoList.Sort((lhs, rhs) => lhs.BundleName.CompareTo(rhs.BundleName));
                        else
                            objectInfoList.Sort((lhs, rhs) => rhs.BundleName.CompareTo(lhs.BundleName));
                    }
                    break;
                case 6:
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
