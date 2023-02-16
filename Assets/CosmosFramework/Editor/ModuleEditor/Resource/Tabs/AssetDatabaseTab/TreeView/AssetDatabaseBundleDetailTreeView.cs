using Cosmos.Resource;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public class AssetDatabaseBundleDetailTreeView : TreeView
    {
        readonly List<ResourceBundleInfo> bundleInfoList = new List<ResourceBundleInfo>();
        public int BundleDetailCount { get { return bundleInfoList.Count; } }
        public AssetDatabaseBundleDetailTreeView(TreeViewState state) : base(state)
        {
            Reload();
            showBorder = true;
            showAlternatingRowBackgrounds = true;
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
        public void Clear()
        {
            bundleInfoList.Clear();
        }
        public void RemoveBundle(ResourceBundleInfo bundleInfo)
        {
            if (bundleInfoList.Contains(bundleInfo))
            {
                bundleInfoList.Remove(bundleInfo);
                Reload();
            }
        }
        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = -1, depth = -1, displayName = "Root" };
            var folderIcon = ResourceWindowUtility.GetFolderIcon();
            var folderEmptyIcon = ResourceWindowUtility.GetFolderEmptyIcon();
            var dependenciesIcon = ResourceWindowUtility.GetFindDependenciesIcon();
            Texture2D icon = null;
            var bundleItemList = new List<TreeViewItem>();
            {
                for (int i = 0; i < bundleInfoList.Count; i++)
                {
                    var bundleInfo = bundleInfoList[i];
                    if (bundleInfo.ResourceObjectInfoList.Count == 0)
                        icon = folderEmptyIcon;
                    else
                        icon = folderIcon;
                    var bundleItem = new TreeViewItem(i, 1, bundleInfo.BundleName) { icon = icon };
                    var formatBytesItem = new TreeViewItem((i + 1) * ResourceEditorConstant.MULTIPLE_VALUE, 2, $"FormatBytes: {bundleInfo.BundleFormatBytes}");
                    var dependentLen = bundleInfo.DependentBundleKeyList.Count;
                    var dependentString = string.Empty;
                    if (dependentLen == 0)
                        dependentString = Constants.NONE;
                    else
                        dependentString = "Count: " + dependentLen.ToString();
                    var dependentItem = new TreeViewItem((i + 1) * ResourceEditorConstant.MULTIPLE_VALUE + 1, 2, $"Dependencies: - {dependentString}") { icon = dependenciesIcon };
                    var bundleSubItemList = new List<TreeViewItem>() { formatBytesItem, dependentItem };
                    //理论上bundle不会有上百万个，因此依赖区间使用百万位扩充
                    SetupParentsAndChildrenFromDepths(bundleItem, bundleSubItemList);

                    var dependentItemList = new List<TreeViewItem>();
                    bundleItemList.Add(bundleItem);
                    for (int j = 0; j < dependentLen; j++)
                    {
                        var bundleKey = bundleInfo.DependentBundleKeyList[j];
                        int depentId = dependentItem.id + j + 1;
                        var depentItem = new TreeViewItem(depentId, 3, bundleKey)
                        {
                            icon = folderIcon
                        };
                        dependentItemList.Add(depentItem);
                        SetupParentsAndChildrenFromDepths(dependentItem, dependentItemList);
                    }
                }
                SetupParentsAndChildrenFromDepths(root, bundleItemList);
                return root;
            }
        }
        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);
            var items = FindRows(new int[] { id });
            var item = items[0];
            var has = ResourceWindowDataProxy.ResourceDataset.PeekResourceBundleInfo(item.displayName, out var bundleInfo);
            if (has)
            {
                EditorUtil.PingAndActiveObject(bundleInfo.BundlePath);
            }
        }
        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            var items = FindRows(new int[] { id });
            var item = items[0];
            var has = ResourceWindowDataProxy.ResourceDataset.PeekResourceBundleInfo(item.displayName, out var bundleInfo);
            if (has)
            {
                EditorUtil.ActiveObject(bundleInfo.BundlePath);
            }
        }
    }
}
