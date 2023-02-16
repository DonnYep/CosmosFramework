using Cosmos.Resource;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
namespace Cosmos.Editor.Resource
{
    public class AssetDatabaseBundleTreeView : TreeView
    {
        readonly List<ResourceBundleInfo> bundleInfoList = new List<ResourceBundleInfo>();

        public Action<IList<int>> onBundleSelectionChanged;
        public Action<IList<int>, IList<int>> onBundleDelete;
        public Action onAllBundleDelete;
        public Action<int, string> onBundleRenamed;
        public Action<IList<string>, IList<int>> onBundleSort;
        /// <summary>
        /// 上一行的cellRect
        /// </summary>
        Rect latestBundleCellRect;
        string originalName;
        /// <summary>
        /// 正在重命名的itemId
        /// </summary>
        int renamingItemId = -1;
        List<string> sortedBundleNames = new List<string>();
        public int BundleCount { get { return bundleInfoList.Count; } }
        public AssetDatabaseBundleTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
            Reload();
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            multiColumnHeader.sortingChanged += OnMultiColumnHeaderSortingChanged;
        }
        public void Clear()
        {
            bundleInfoList.Clear();
        }
        public bool AddBundle(ResourceBundleInfo bundleInfo)
        {
            if (!bundleInfoList.Contains(bundleInfo))
            {
                bundleInfoList.Add(bundleInfo);
                return true;
            }
            return false;
        }
        public void RemoveBundle(ResourceBundleInfo bundleInfo)
        {
            if (bundleInfoList.Contains(bundleInfo))
            {
                bundleInfoList.Remove(bundleInfo);
                Reload();
            }
        }
        protected override void DoubleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            EditorUtil.PingAndActiveObject(bundleInfoList[id].BundlePath);
        }
        protected override void ContextClickedItem(int id)
        {
            var selected = GetSelection();
            GenericMenu menu = new GenericMenu();
            if (selected.Count == 1)
            {
                menu.AddItem(new GUIContent("Delete bundle"), false, DeleteBundles, selected);
                menu.AddItem(new GUIContent("Delete all bundle"), false, DeleteAllBundles);
                menu.AddItem(new GUIContent("Reset bundle name"), false, ResetBundlesName, selected);
                menu.AddItem(new GUIContent("Reset all bundle name"), false, ResetAllBundleName);
                menu.AddItem(new GUIContent("Copy bundle name to clipboard"), false, CopyBundleNameToClipboard, id);
                menu.AddItem(new GUIContent("Copy bundle path to clipboard"), false, CopyBundlePathToClipboard, id);
                //menu.AddItem(new GUIContent("Split bundle"), false, SplitBundles, selected);
                //menu.AddItem(new GUIContent("Merge bundle"), false, MergeBundles, selected);
            }
            else if (selected.Count > 1)
            {
                menu.AddItem(new GUIContent("Delete bundles"), false, DeleteBundles, selected);
                menu.AddItem(new GUIContent("Delete all bundles"), false, DeleteAllBundles);
                menu.AddItem(new GUIContent("Reset bundles name"), false, ResetBundlesName, selected);
                menu.AddItem(new GUIContent("Reset the names of all bundles"), false, ResetAllBundleName);
                //menu.AddItem(new GUIContent("Split bundles"), false, SplitBundles, selected);
                //menu.AddItem(new GUIContent("Merge bundles"), false, MergeBundles, selected);
            }
            menu.ShowAsContext();
        }
        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            EditorUtil.ActiveObject(bundleInfoList[id].BundlePath);
        }
        protected override void RowGUI(RowGUIArgs args)
        {
            var length = args.GetNumVisibleColumns();
            for (int i = 0; i < length; i++)
            {
                DrawCellGUI(args.GetCellRect(i), args.item as AssetDatabaseBundleTreeViewItem, args.GetColumn(i), ref args);
            }
        }
        protected override bool CanRename(TreeViewItem item)
        {
            originalName = item.displayName;
            renamingItemId = item.id;
            BeginRename(item);
            return item != null;
        }
        protected override void RenameEnded(RenameEndedArgs args)
        {
            var item = FindItem(args.itemID, rootItem);
            var newName = args.newName;
            if (!string.IsNullOrWhiteSpace(newName))
            {
                newName = ResourceUtility.FilterName(newName);
                //防止重名
                var canUse = true;
                var length = bundleInfoList.Count;
                for (int i = 0; i < length; i++)
                {
                    if (bundleInfoList[i].BundleName == newName)
                    {
                        canUse = false;
                        break;
                    }
                }
                if (canUse)
                {
                    var bundleInfo = bundleInfoList[args.itemID];
                    bundleInfo.BundleName = newName;
                    item.displayName = newName;
                    onBundleRenamed?.Invoke(args.itemID, newName);
                }
                else
                {
                    item.displayName = originalName;
                }
            }
            else
            {
                item.displayName = originalName;
            }
            base.RenameEnded(args);
            renamingItemId = -1;
        }
        protected override Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
        {
            return new Rect(latestBundleCellRect.x + 2, latestBundleCellRect.height * row, latestBundleCellRect.width, latestBundleCellRect.height);
        }
        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = -1, depth = -1, displayName = "Root" };
            var folderIcon = ResourceWindowUtility.GetFolderIcon();
            var folderEmptyIcon = ResourceWindowUtility.GetFolderEmptyIcon();
            var d_folderIcon = ResourceWindowUtility.GetD_FolderIcon();
            var d_folderEmptyIcon = ResourceWindowUtility.GetD_FolderEmptyIcon();
            Texture2D icon = null;
            var treeItemList = new List<TreeViewItem>();
            {
                for (int i = 0; i < bundleInfoList.Count; i++)
                {
                    var bundleInfo = bundleInfoList[i];
                    if (bundleInfo.ResourceObjectInfoList.Count == 0)
                        icon = folderEmptyIcon;
                    else
                        icon = folderIcon;
                    var item = new AssetDatabaseBundleTreeViewItem(i, 1, bundleInfo.BundleName, icon)
                    {
                        BundleFormatSize = bundleInfo.BundleFormatBytes,
                        ObjectCount = bundleInfo.ResourceObjectInfoList.Count
                    };
                    treeItemList.Add(item);
                }
                SetupParentsAndChildrenFromDepths(root, treeItemList);
                return root;
            }
        }
        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
            onBundleSelectionChanged?.Invoke(selectedIds);
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
                        //Size
                        if (ascending)
                            bundleInfoList.Sort((lhs, rhs) => lhs.BundleSize.CompareTo(rhs.BundleSize));
                        else
                            bundleInfoList.Sort((lhs, rhs) => rhs.BundleSize.CompareTo(lhs.BundleSize));
                    }
                    break;
                case 1:
                    {
                        //Amount
                        if (ascending)
                            bundleInfoList.Sort((lhs, rhs) => lhs.ResourceObjectInfoList.Count.CompareTo(rhs.ResourceObjectInfoList.Count));
                        else
                            bundleInfoList.Sort((lhs, rhs) => rhs.ResourceObjectInfoList.Count.CompareTo(lhs.ResourceObjectInfoList.Count));
                    }
                    break;
                case 2:
                    {
                        //Bundle
                        if (ascending)
                            bundleInfoList.Sort((lhs, rhs) => lhs.BundleName.CompareTo(rhs.BundleName));
                        else
                            bundleInfoList.Sort((lhs, rhs) => rhs.BundleName.CompareTo(lhs.BundleName));
                    }
                    break;
            }
            sortedBundleNames.Clear();
            var bundleLength = bundleInfoList.Count;
            for (int i = 0; i < bundleLength; i++)
            {
                sortedBundleNames.Add(bundleInfoList[i].BundleName);
            }
            onBundleSort?.Invoke(sortedBundleNames, GetSelection());
            Reload();
        }
        void DrawCellGUI(Rect cellRect, AssetDatabaseBundleTreeViewItem treeView, int column, ref RowGUIArgs args)
        {
            switch (column)
            {
                case 0:
                    {
                        var lablCellRect = new Rect(cellRect.x + 4, cellRect.y, cellRect.width, cellRect.height);
                        DefaultGUI.Label(lablCellRect, treeView.BundleFormatSize, args.selected, args.focused);
                    }
                    break;
                case 1:
                    {
                        var lablCellRect = new Rect(cellRect.x + 4, cellRect.y, cellRect.width, cellRect.height);
                        DefaultGUI.Label(lablCellRect, treeView.ObjectCount.ToString(), args.selected, args.focused);
                    }
                    break;
                case 2:
                    {
                        var iconRect = new Rect(cellRect.x + 2, cellRect.y, cellRect.height, cellRect.height);
                        if (treeView.icon != null)
                            GUI.DrawTexture(iconRect, treeView.icon, ScaleMode.ScaleToFit);
                        var labelCellRect = new Rect(cellRect.x + iconRect.width + 4, cellRect.y, cellRect.width - iconRect.width, cellRect.height);
                        if (treeView.id != renamingItemId)
                            DefaultGUI.Label(labelCellRect, treeView.displayName, args.selected, args.focused);
                        latestBundleCellRect = labelCellRect;
                    }
                    break;
            }
        }
        void DeleteAllBundles()
        {
            bundleInfoList.Clear();
            onAllBundleDelete?.Invoke();
            Reload();
        }
        void DeleteBundles(object context)
        {
            try
            {
                var list = context as IList<int>;
                var length = list.Count;
                var rmBundleInfos = new ResourceBundleInfo[length];
                for (int i = 0; i < length; i++)
                {
                    var rmId = list[i];
                    rmBundleInfos[i] = bundleInfoList[rmId];
                }
                for (int i = 0; i < length; i++)
                {
                    bundleInfoList.Remove(rmBundleInfos[i]);
                }
                onBundleDelete?.Invoke(list, GetSelection());
                SetSelection(new int[0]);
                Reload();
            }
            catch (Exception e)
            {
                EditorUtil.Debug.LogError(e);
            }
        }
        void ResetBundlesName(object context)
        {
            try
            {
                var list = context as IList<int>;
                var length = list.Count;
                for (int i = 0; i < length; i++)
                {
                    var itemId = list[i];
                    var item = FindItem(itemId, rootItem);
                    var bundleInfo = bundleInfoList[itemId];
                    var bundleName = ResourceUtility.FilterName(bundleInfo.BundlePath);
                    bundleInfo.BundleName = bundleName;
                    item.displayName = bundleInfo.BundleName;
                    onBundleRenamed?.Invoke(itemId, bundleInfo.BundlePath);
                }
            }
            catch (Exception e)
            {
                EditorUtil.Debug.LogError(e);
            }
        }
        void ResetAllBundleName()
        {
            var length = bundleInfoList.Count;
            for (int i = 0; i < length; i++)
            {
                var item = FindItem(i, rootItem);
                var bundleInfo = bundleInfoList[i];
                var bundleName = ResourceUtility.FilterName(bundleInfo.BundlePath);
                bundleInfo.BundleName = bundleName;
                item.displayName = bundleInfo.BundleName;
                onBundleRenamed?.Invoke(i, bundleInfo.BundlePath);
            }
        }
        void CopyBundleNameToClipboard(object context)
        {
            var id = Convert.ToInt32(context);
            var bundle = bundleInfoList[id];
            GUIUtility.systemCopyBuffer = bundle.BundleName;
        }
        void CopyBundlePathToClipboard(object context)
        {
            var id = Convert.ToInt32(context);
            var bundle = bundleInfoList[id];
            GUIUtility.systemCopyBuffer = bundle.BundlePath;
        }
        void SplitBundles(object context)
        {
            try
            {
                var list = context as IList<int>;
                var items = FindRows(list);
                var length = items.Count;
                for (int i = 0; i < length; i++)
                {
                    var item = items[i];
                    var has = ResourceWindowDataProxy.ResourceDataset.PeekResourceBundleInfo(item.displayName, out var bundleInfo);
                    if (has)
                    {
                        var bundlePath = bundleInfo.BundlePath;
                        var subPaths = AssetDatabase.GetSubFolders(bundlePath);
                        for (int j = 0; j < subPaths.Length; j++)
                        {
                            EditorUtil.Debug.LogInfo(subPaths[j]);
                            var subPath = subPaths[j];
                            var isSceneInSameBundle = ResourceWindowUtility.CheckAssetsAndScenesInOneAssetBundle(subPath);
                            if (isSceneInSameBundle)
                            {
                                var invalidBundleName = ResourceUtility.FilterName(subPath);
                                EditorUtil.Debug.LogError($"Cannot mark assets and scenes in one AssetBundle. AssetBundle name is {invalidBundleName}");
                                continue;
                            }
                            var newBundleInfo = new ResourceBundleInfo()
                            {
                                BundleName = subPath,
                                BundlePath = subPath
                            };
                            if (!bundleInfo.ResourceSubBundleInfoList.Contains(newBundleInfo))
                            {
                                bundleInfo.ResourceSubBundleInfoList.Add(newBundleInfo);
                                newBundleInfo.BundleKey = newBundleInfo.BundleName;
                                ResourceWindowDataProxy.ResourceDataset.IsChanged = true;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                EditorUtil.Debug.LogError(e);
            }
        }
        void MergeBundles(object context)
        {
            try
            {
                var list = context as IList<int>;
                var items = FindRows(list);
                var length = items.Count;
                for (int i = 0; i < length; i++)
                {
                    var item = items[i];
                    var has = ResourceWindowDataProxy.ResourceDataset.PeekResourceBundleInfo(item.displayName, out var bundleInfo);
                    if (has)
                    {

                    }
                }
            }
            catch (Exception e)
            {
                EditorUtil.Debug.LogError(e);
            }
        }
    }
}
