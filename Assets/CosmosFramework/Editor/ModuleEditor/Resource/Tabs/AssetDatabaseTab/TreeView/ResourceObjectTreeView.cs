using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEditor;
using Cosmos.Resource;

namespace Cosmos.Editor.Resource
{
    public class ResourceObjectTreeView : TreeView
    {
        List<ResourceObjectInfo> objectInfoList = new List<ResourceObjectInfo>();
        public ResourceObjectTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
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
                for (int i = 0; i < objectInfoList.Count; i++)
                {
                    var objectInfo = objectInfoList[i];
                    var treeViewItem = new ResourceObjectTreeViewItem(i, 1, objectInfo.ObjectPath, objectInfo.ObjectIcon)
                    {
                        ObjectName = objectInfo.ObjectName,
                        ObjectState = objectInfo.ObjectState,
                        ObjectStateIcon = objectInfo.ObjectStateIcon,
                        ObjectSize = objectInfo.ObjectFormatBytes,
                        ObjectBundleName = objectInfo.BundleName,
                        ObjectExtension = objectInfo.Extension
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
                DrawCellGUI(args.GetCellRect(i), args.item as ResourceObjectTreeViewItem, args.GetColumn(i), ref args);
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
                            objectInfoList.Sort((lhs, rhs) => lhs.ObjectState.CompareTo(rhs.ObjectState));
                        else
                            objectInfoList.Sort((lhs, rhs) => rhs.ObjectState.CompareTo(lhs.ObjectState));
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
                        DefaultGUI.Label(cellRect, treeView.ObjectExtension, args.selected, args.focused);
                    }
                    break;
                case 2:
                    {
                        var iconRect = new Rect(cellRect.x, cellRect.y, cellRect.height, cellRect.height);
                        if (treeView.icon != null)
                            GUI.DrawTexture(iconRect, treeView.ObjectStateIcon, ScaleMode.ScaleToFit);
                        var lablCellRect = new Rect(cellRect.x + iconRect.width + 2, cellRect.y, cellRect.width - iconRect.width, cellRect.height);

                        DefaultGUI.Label(lablCellRect, treeView.ObjectState, args.selected, args.focused);
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
