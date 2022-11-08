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
            multiColumnHeader.sortingChanged += OnMultiColumnHeaderSortingChanged;
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
            menu.AddItem(new GUIContent("Copy object name to clipboard"), false, CopyNameToClipboard, id);
            menu.AddItem(new GUIContent("Copy object path to clipboard"), false, CopyAssetPathToClipboard, id);
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
                    string objectState = string.Empty;
                    if (objectList[i].Vaild)
                    {
                        objectIcon = EditorUtil.ToTexture2D(EditorGUIUtility.ObjectContent(obj, obj.GetType()).image);
                        objectState = "VALID";
                        stateIcon = ResourceWindowUtility.GetAssetValidIcon();
                    }
                    else
                    {
                        objectIcon = EditorGUIUtility.FindTexture("console.erroricon");
                        objectState = "INVALID";
                        stateIcon = ResourceWindowUtility.GetAssetInvalidIcon();
                    }
                    var item = new ResourceObjectTreeViewItem(i, 1, objectInfo.AssetPath, objectIcon)
                    {
                        ObjectName = objectInfo.ObjectName,
                        ObjectState = objectState,
                        ObjectStateIcon = stateIcon,
                        ObjectSize = objectInfo.FileSize,
                        ObjectBundleName = objectInfo.AssetBundleName,
                        ObjectExtension = objectInfo.Extension
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
                            objectList.Sort((lhs, rhs) => lhs.ObjectName.CompareTo(rhs.ObjectName));
                        else
                            objectList.Sort((lhs, rhs) => rhs.ObjectName.CompareTo(lhs.ObjectName));
                    }
                    break;
                case 1:
                    {
                        //Extension
                        if (ascending)
                            objectList.Sort((lhs, rhs) => lhs.Extension.CompareTo(rhs.Extension));
                        else
                            objectList.Sort((lhs, rhs) => rhs.Extension.CompareTo(lhs.Extension));
                    }
                    break;
                case 2:
                    {
                        //State
                        if (ascending)
                            objectList.Sort((lhs, rhs) => lhs.Vaild.CompareTo(rhs.Vaild));
                        else
                            objectList.Sort((lhs, rhs) => rhs.Vaild.CompareTo(lhs.Vaild));
                    }
                    break;
                case 3:
                    {
                        //Size
                        if (ascending)
                            objectList.Sort((lhs, rhs) => lhs.FileSize.CompareTo(rhs.FileSize));
                        else
                            objectList.Sort((lhs, rhs) => rhs.FileSize.CompareTo(lhs.FileSize));
                    }
                    break;
                case 4:
                    {
                        //AssetBundle
                        if (ascending)
                            objectList.Sort((lhs, rhs) => lhs.AssetBundleName.CompareTo(rhs.AssetBundleName));
                        else
                            objectList.Sort((lhs, rhs) => rhs.AssetBundleName.CompareTo(lhs.AssetBundleName));
                    }
                    break;
                case 5:
                    {
                        //AssetPath
                        if (ascending)
                            objectList.Sort((lhs, rhs) => lhs.AssetPath.CompareTo(rhs.AssetPath));
                        else
                            objectList.Sort((lhs, rhs) => rhs.AssetPath.CompareTo(lhs.AssetPath));
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
