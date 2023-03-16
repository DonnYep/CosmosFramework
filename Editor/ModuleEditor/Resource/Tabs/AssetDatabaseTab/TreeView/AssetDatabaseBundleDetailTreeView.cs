﻿using Cosmos.Resource;
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
            var subBundleIcom = ResourceWindowUtility.GetHorizontalLayoutGroupIcon();
            Texture2D icon = null;
            var bundleItemList = new List<TreeViewItem>();
            var bundleInfoLength = bundleInfoList.Count;
            for (int i = 0; i < bundleInfoLength; i++)
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

                var dependentRootItem = new TreeViewItem((i + 1) * ResourceEditorConstant.MULTIPLE_VALUE + 1, 2, $"Dependencies: - {dependentString}") { icon = dependenciesIcon };

                var subBundleLen = bundleInfo.ResourceSubBundleInfoList.Count;
                var subBundleString = string.Empty;
                if (!bundleInfo.Splittable)
                {
                    subBundleString = Constants.NONE;
                }
                else
                {
                    if (subBundleLen == 0)
                        subBundleString = Constants.NONE;
                    else
                        subBundleString = "Count: " + subBundleLen.ToString();
                }
                var subBundleRootItem = new TreeViewItem((i + 1) * ResourceEditorConstant.MULTIPLE_VALUE + 2, 2, $"SubBundles: - {subBundleString}") { icon = dependenciesIcon };

                var subBundleItemList = new List<TreeViewItem>();

                if (bundleInfo.Splittable)
                {
                    var subBundleLength = bundleInfo.ResourceSubBundleInfoList.Count;
                    for (int j = 0; j < subBundleLength; j++)
                    {
                        var subBundle = bundleInfo.ResourceSubBundleInfoList[j];
                        int subBundleItemId = subBundleRootItem.id + j + 2 + ResourceEditorConstant.SBU_MULTIPLE_VALUE;//拆分子包区间数值
                        var subBundleItem = new TreeViewItem(subBundleItemId, 3, subBundle.BundleName)
                        {
                            icon = subBundleIcom
                        };
                        subBundleItemList.Add(subBundleItem);
                        SetupParentsAndChildrenFromDepths(subBundleRootItem, subBundleItemList);
                    }
                }
                var bundleSubItemList = new List<TreeViewItem>() { formatBytesItem, dependentRootItem, subBundleRootItem };

                var dependentItemList = new List<TreeViewItem>();
                for (int j = 0; j < dependentLen; j++)
                {
                    var bundleKey = bundleInfo.DependentBundleKeyList[j];
                    int dependentItemId = dependentRootItem.id + j + 2;
                    var dependentItem = new TreeViewItem(dependentItemId, 3, bundleKey)
                    {
                        icon = folderIcon
                    };
                    dependentItemList.Add(dependentItem);
                    SetupParentsAndChildrenFromDepths(dependentRootItem, dependentItemList);
                }
                bundleItemList.Add(bundleItem);
                SetupParentsAndChildrenFromDepths(bundleItem, bundleSubItemList);
            }

            SetupParentsAndChildrenFromDepths(root, bundleItemList);
            return root;
        }
        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);
            var items = FindRows(new int[] { id });
            var item = items[0];
            var bundleInfo = ResourceWindowDataProxy.ResourceDataset.ResourceBundleInfoList.Find((b) => b.BundleName == item.displayName);
            var has = bundleInfo != null;
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
            var bundleInfo = ResourceWindowDataProxy.ResourceDataset.ResourceBundleInfoList.Find((b) => b.BundleName == item.displayName);
            var has = bundleInfo != null;
            if (has)
            {
                EditorUtil.ActiveObject(bundleInfo.BundlePath);
            }
        }
    }
}
