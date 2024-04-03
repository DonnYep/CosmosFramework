using Cosmos.Resource;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
namespace Cosmos.Editor.Resource
{
    public class AssetDatabaseBundleTreeView : TreeView
    {
        readonly List<ResourceBundleInfo> bundleInfoList = new List<ResourceBundleInfo>();
        public Rect TreeViewRect { get { return treeViewRect; } }
        public Action<IList<int>> onBundleSelectionChanged;
        public Action<IList<int>, IList<int>> onBundleDelete;
        public Action onAllBundleDelete;
        public Action<int, string> onBundleRenamed;
        public Action<IList<string>, IList<int>> onBundleSort;
        public Action<IList<int>> onMarkAsSplit;
        public Action<IList<int>> onMarkAsNotSplit;
        public Action<IList<int>> onMarkAsExtract;
        public Action<IList<int>> onMarkAsNotExtract;
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
            }
            else if (selected.Count > 1)
            {
                menu.AddItem(new GUIContent("Delete bundles"), false, DeleteBundles, selected);
                menu.AddItem(new GUIContent("Delete all bundles"), false, DeleteAllBundles);
                menu.AddItem(new GUIContent("Reset bundles name"), false, ResetBundlesName, selected);
                menu.AddItem(new GUIContent("Reset the names of all bundles"), false, ResetAllBundleName);
            }
            menu.AddItem(new GUIContent("Mark as splittable"), false, SplitBundles, selected);
            menu.AddItem(new GUIContent("Mark as unsplittable"), false, MergeBundles, selected);

            menu.AddItem(new GUIContent("Mark as separately"), false, MarkAsSeparatelyBundles, selected);
            menu.AddItem(new GUIContent("Mark as together"), false, MarkAsTogetherBundles, selected);
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
            var folderIcon = ResourceEditorUtility.GetFolderIcon();
            var folderEmptyIcon = ResourceEditorUtility.GetFolderEmptyIcon();
            var vaildIcon = ResourceEditorUtility.GetValidIcon();
            var ignoredIcon = ResourceEditorUtility.GetIgnoredcon();
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
                        ObjectCount = bundleInfo.ResourceObjectInfoList.Count,
                        SplitBundleCount = bundleInfo.ResourceSubBundleInfoList.Count,
                        SplitIcon = vaildIcon,
                        NotSplitIcon = ignoredIcon,
                        ExtractIcon = vaildIcon,
                        NotExtractIcon = ignoredIcon,
                        Split = bundleInfo.Split,
                        Extract = bundleInfo.Extract
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
                        //Split
                        ResourceBundleInfo[] orderedList;
                        if (ascending)
                        {
                            orderedList = bundleInfoList.OrderBy((b) => b.Split).ThenBy((b) => { return b.ResourceSubBundleInfoList.Count; }).ToArray();
                        }
                        else
                        {
                            orderedList = bundleInfoList.OrderByDescending((b) => b.Split).ThenByDescending((b) => { return b.ResourceSubBundleInfoList.Count; }).ToArray();
                        }
                        bundleInfoList.Clear();
                        bundleInfoList.AddRange(orderedList);
                    }
                    break;
                case 3:
                    {
                        //Extract
                        ResourceBundleInfo[] orderedList;
                        if (ascending)
                        {
                            orderedList = bundleInfoList.OrderBy((b) => b.Extract).ToArray();
                        }
                        else
                        {
                            orderedList = bundleInfoList.OrderByDescending((b) => b.Extract).ToArray();
                        }
                        bundleInfoList.Clear();
                        bundleInfoList.AddRange(orderedList);
                    }
                    break;
                case 4:
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
                        var split = treeView.Split;
                        var iconRect = new Rect(cellRect.x, cellRect.y, cellRect.height, cellRect.height);
                        if (split)
                        {
                            if (treeView.SplitIcon != null)
                                GUI.DrawTexture(iconRect, treeView.SplitIcon, ScaleMode.ScaleToFit);
                            var labelCellRect = new Rect(cellRect.x + iconRect.width + 4, cellRect.y, cellRect.width - iconRect.width, cellRect.height);
                            DefaultGUI.Label(labelCellRect, treeView.SplitBundleCount.ToString(), args.selected, args.focused);
                        }
                        else
                        {
                            if (treeView.NotSplitIcon != null)
                                GUI.DrawTexture(iconRect, treeView.NotSplitIcon, ScaleMode.ScaleToFit);
                        }
                    }
                    break;
                case 3:
                    {
                        var extract = treeView.Extract;
                        var iconRect = new Rect(cellRect.x, cellRect.y, cellRect.height, cellRect.height);
                        if (extract)
                        {
                            if (treeView.ExtractIcon != null)
                                GUI.DrawTexture(iconRect, treeView.ExtractIcon, ScaleMode.ScaleToFit);
                            //var labelCellRect = new Rect(cellRect.x + iconRect.width + 4, cellRect.y, cellRect.width - iconRect.width, cellRect.height);
                            //DefaultGUI.Label(labelCellRect, treeView.SplitBundleCount.ToString(), args.selected, args.focused);
                        }
                        else
                        {
                            if (treeView.NotExtractIcon != null)
                                GUI.DrawTexture(iconRect, treeView.NotExtractIcon, ScaleMode.ScaleToFit);
                        }
                    }
                    break;
                case 4:
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
                SetSelection(new int[0]);
                onBundleDelete?.Invoke(list, GetSelection());
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
                bool hasChanged = false;
                var list = context as IList<int>;
                var items = FindRows(list);
                var length = items.Count;
                for (int i = 0; i < length; i++)
                {
                    var item = items[i];
                    var bundleInfo = ResourceBuilderWindowDataProxy.ResourceDataset.ResourceBundleInfoList.Find((b) => b.BundleName == item.displayName);
                    var has = bundleInfo != null;
                    if (has)
                    {
                        if (!bundleInfo.Split)
                        {
                            hasChanged = true;
                        }
                        bundleInfo.Split = true;
                    }
                }
                if (hasChanged)
                    onMarkAsSplit?.Invoke(list);
                Reload();
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
                bool hasChanged = false;
                var list = context as IList<int>;
                var items = FindRows(list);
                var length = items.Count;
                for (int i = 0; i < length; i++)
                {
                    var item = items[i];
                    var bundleInfo = ResourceBuilderWindowDataProxy.ResourceDataset.ResourceBundleInfoList.Find((b) => b.BundleName == item.displayName);
                    var has = bundleInfo != null;
                    if (has)
                    {
                        if (bundleInfo.Split)
                        {
                            hasChanged = true;
                        }
                        bundleInfo.Split = false;
                    }
                }
                if (hasChanged)
                    onMarkAsNotSplit?.Invoke(list);
                Reload();
            }
            catch (Exception e)
            {
                EditorUtil.Debug.LogError(e);
            }
        }
        void MarkAsSeparatelyBundles(object context)
        {
            try
            {
                bool hasChanged = false;
                var list = context as IList<int>;
                var items = FindRows(list);
                var length = items.Count;
                for (int i = 0; i < length; i++)
                {
                    var item = items[i];
                    var bundleInfo = ResourceBuilderWindowDataProxy.ResourceDataset.ResourceBundleInfoList.Find((b) => b.BundleName == item.displayName);
                    var has = bundleInfo != null;
                    if (has)
                    {
                        if (!bundleInfo.Extract)
                        {
                            hasChanged = true;
                        }
                        bundleInfo.Extract = true;
                    }
                }
                if (hasChanged)
                    onMarkAsExtract?.Invoke(list);
                Reload();
            }
            catch (Exception e)
            {
                EditorUtil.Debug.LogError(e);
            }
        }
        void MarkAsTogetherBundles(object context)
        {
            try
            {
                bool hasChanged = false;
                var list = context as IList<int>;
                var items = FindRows(list);
                var length = items.Count;
                for (int i = 0; i < length; i++)
                {
                    var item = items[i];
                    var bundleInfo = ResourceBuilderWindowDataProxy.ResourceDataset.ResourceBundleInfoList.Find((b) => b.BundleName == item.displayName);
                    var has = bundleInfo != null;
                    if (has)
                    {
                        if (bundleInfo.Extract)
                        {
                            hasChanged = true;
                        }
                        bundleInfo.Extract = false;
                    }
                }
                if (hasChanged)
                    onMarkAsNotExtract?.Invoke(list);
                Reload();
            }
            catch (Exception e)
            {
                EditorUtil.Debug.LogError(e);
            }
        }
    }
}
