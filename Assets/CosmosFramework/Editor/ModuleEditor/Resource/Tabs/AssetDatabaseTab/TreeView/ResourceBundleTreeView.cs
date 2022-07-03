using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
namespace Cosmos.Editor.Resource
{
    public class ResourceBundleTreeView : TreeView
    {
        readonly List<ResourceBundleInfo> bundleList = new List<ResourceBundleInfo>();

        public Action<IList<int>> onSelectionChanged;
        public Action<IList<int>> onDelete;
        public Action onAllDelete;
        public Action<int, string> onRenameBundle;
        /// <summary>
        /// 上一行的cellRect
        /// </summary>
        Rect latestBundleCellRect;
        string originalName;
        public ResourceBundleTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
            Reload();
            showAlternatingRowBackgrounds = true;
            showBorder = true;
        }
        public void Clear()
        {
            bundleList.Clear();
            Reload();
        }
        public bool AddBundle(ResourceBundleInfo bundleInfo)
        {
            if (!bundleList.Contains(bundleInfo))
            {
                bundleList.Add(bundleInfo);
                Reload();
                return true;
            }
            return false;
        }
        public void RemoveBundle(ResourceBundleInfo bundleInfo)
        {
            if (bundleList.Contains(bundleInfo))
            {
                bundleList.Remove(bundleInfo);
                Reload();
            }
        }
        protected override void DoubleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            EditorUtil.PingAndActiveObject(bundleList[id].BundlePath);
        }
        protected override void ContextClickedItem(int id)
        {
            var selected = GetSelection();
            GenericMenu menu = new GenericMenu();
            if (selected.Count == 1)
            {
                menu.AddItem(new GUIContent("Delete bundle"), false, Delete, selected);
                menu.AddItem(new GUIContent("Delete all bundle"), false, DeleteAll);
                menu.AddItem(new GUIContent("Reset bundle name"), false, ResetBundleName, id);
                menu.AddItem(new GUIContent("Reset all bundle name"), false, ResetAllBundleName);
            }
            else if (selected.Count > 1)
            {
                menu.AddItem(new GUIContent("Delete bundle"), false, Delete, selected);
                menu.AddItem(new GUIContent("Delete all bundle"), false, DeleteAll);
                menu.AddItem(new GUIContent("Reset all bundle name"), false, ResetAllBundleName);
            }
            menu.ShowAsContext();
        }
        protected override void RowGUI(RowGUIArgs args)
        {
            var length = args.GetNumVisibleColumns();
            for (int i = 0; i < length; i++)
            {
                DrawCellGUI(args.GetCellRect(i), args.item as ResourceBundleTreeViewItem, args.GetColumn(i), ref args);
            }
        }
        protected override bool CanRename(TreeViewItem item)
        {
            originalName = item.displayName;
            item.displayName = null;
            BeginRename(item);
            return item != null;
        }
        protected override void RenameEnded(RenameEndedArgs args)
        {
            var item = FindItem(args.itemID, rootItem);
            if (!string.IsNullOrEmpty(args.newName))
            {
                //防止重名
                var canUse = true;
                var newName = args.newName;
                var length = bundleList.Count;
                for (int i = 0; i < length; i++)
                {
                    if (bundleList[i].BundleName == newName)
                    {
                        canUse = false;
                        break;
                    }
                }
                if (canUse)
                {
                    var bundleInfo = bundleList[args.itemID];
                    bundleList[args.itemID] = new ResourceBundleInfo(newName, bundleInfo.BundlePath, bundleInfo.BundleSize);
                    item.displayName = args.newName;
                    onRenameBundle?.Invoke(args.itemID, args.newName);
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
        }
        protected override Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
        {
            return new Rect(latestBundleCellRect.x, latestBundleCellRect.height * row, latestBundleCellRect.width, latestBundleCellRect.height);
        }
        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = -1, depth = -1, displayName = "Root" };
            var assetIcon = ResourceEditorUtility.GetFolderIcon();
            var allItems = new List<TreeViewItem>();
            {
                for (int i = 0; i < bundleList.Count; i++)
                {
                    var item = new ResourceBundleTreeViewItem(i, 1, bundleList[i].BundleName, assetIcon) { BundleSize = bundleList[i].BundleSize };
                    allItems.Add(item);
                }
                SetupParentsAndChildrenFromDepths(root, allItems);
                return root;
            }
        }
        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
            onSelectionChanged?.Invoke(selectedIds);
        }
        void DrawCellGUI(Rect cellRect, ResourceBundleTreeViewItem treeView, int column, ref RowGUIArgs args)
        {
            switch (column)
            {
                case 0:
                    {
                        var lablCellRect = new Rect(cellRect.x + 4, cellRect.y, cellRect.width, cellRect.height);
                        DefaultGUI.Label(lablCellRect, treeView.BundleSize, args.selected, args.focused);
                    }
                    break;
                case 1:
                    {
                        var iconRect = new Rect(cellRect.x + 2, cellRect.y, cellRect.height, cellRect.height);
                        if (treeView.icon != null)
                            GUI.DrawTexture(iconRect, treeView.icon, ScaleMode.ScaleToFit);
                        var lableCellRect = new Rect(cellRect.x + iconRect.width + 4, cellRect.y, cellRect.width - iconRect.width, cellRect.height);
                        DefaultGUI.Label(lableCellRect, treeView.displayName, args.selected, args.focused);
                        latestBundleCellRect = lableCellRect;
                    }
                    break;
            }
        }
        void DeleteAll()
        {
            bundleList.Clear();
            onAllDelete?.Invoke();
            Reload();
        }
        void Delete(object context)
        {
            try
            {
                var list = context as IList<int>;
                var length = list.Count;
                var rmbundles = new ResourceBundleInfo[length];
                for (int i = 0; i < length; i++)
                {
                    var rmid = list[i];
                    rmbundles[i] = bundleList[rmid];
                }
                var rmlen = rmbundles.Length;
                for (int i = 0; i < rmlen; i++)
                {
                    bundleList.Remove(rmbundles[i]);
                }
                onDelete?.Invoke(list);
                SetSelection(new int[0]);
                Reload();
            }
            catch (Exception e)
            {
                EditorUtil.Debug.LogError(e);
            }
        }
        void ResetBundleName(object context)
        {
            var itemId = Convert.ToInt32(context);
            var item = FindItem(itemId, rootItem);
            var bundleInfo = bundleList[itemId];
            item.displayName = bundleInfo.BundlePath;
            bundleList[itemId] = new ResourceBundleInfo(bundleInfo.BundlePath, bundleInfo.BundlePath, bundleInfo.BundleSize);
            onRenameBundle?.Invoke(itemId, bundleInfo.BundlePath);
        }
        void ResetAllBundleName()
        {
            var length = bundleList.Count;
            for (int i = 0; i < length; i++)
            {
                var item = FindItem(i, rootItem);
                var bundleInfo = bundleList[i];
                item.displayName = bundleInfo.BundlePath;
                bundleList[i] = new ResourceBundleInfo(bundleInfo.BundlePath, bundleInfo.BundlePath, bundleInfo.BundleSize);
                onRenameBundle?.Invoke(i, bundleInfo.BundlePath);
            }
        }
    }
}
