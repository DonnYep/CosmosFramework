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

        public Action<List<ResourceBundleInfo>> onDelete;
        public Action onAllDelete;
        public Action<int> onBundleClick;
        public ResourceBundleTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state,multiColumnHeader)
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
        public void AddBundle(ResourceBundleInfo bundleInfo)
        {
            if (!bundleList.Contains(bundleInfo))
            {
                bundleList.Add(bundleInfo);
                Reload();
            }
        }
        public void RemoveBundle(ResourceBundleInfo bundleInfo)
        {
            if (bundleList.Contains(bundleInfo))
            {
                bundleList.Remove(bundleInfo);
                Reload();
            }
        }
        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            onBundleClick?.Invoke(id);
        }
        protected override void DoubleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            EditorUtil.PingAndActiveObject(bundleList[id].BundlePath);
        }
        protected override void ContextClickedItem(int id)
        {
            List<ResourceBundleInfo> selectedNodes = new List<ResourceBundleInfo>();

            var selected = GetSelection();
            foreach (var nodeId in selected)
            {
                selectedNodes.Add(bundleList[nodeId]);
            }
            GenericMenu menu = new GenericMenu();
            if (selectedNodes.Count >= 1)
            {
                menu.AddItem(new GUIContent("Delete"), false, Delete, selectedNodes);
                menu.AddItem(new GUIContent("DeleteAll"), false, DeleteAll);
            }
            menu.ShowAsContext();
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
                var list = context as List<ResourceBundleInfo>;
                for (int i = 0; i < list.Count; i++)
                {
                    RemoveBundle(list[i]);
                }
                onDelete?.Invoke(list);
                Reload();
            }
            catch (Exception e)
            {
                EditorUtil.Debug.LogError(e);
            }
        }
        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = -1, depth = -1, displayName = "Root" };
            var assetIcon = ResourceEditorUtil.GetFolderIcon();
            var allItems = new List<TreeViewItem>();
            {
                for (int i = 0; i < bundleList.Count; i++)
                {
                    var item = new TreeViewItem { id = i, depth = 1, displayName = bundleList[i].BundlePath, icon = assetIcon };
                    allItems.Add(item);
                }
                SetupParentsAndChildrenFromDepths(root, allItems);
                return root;
            }
        }
    }
}
