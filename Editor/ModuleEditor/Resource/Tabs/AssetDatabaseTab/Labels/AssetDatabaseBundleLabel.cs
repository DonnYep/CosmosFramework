﻿using Cosmos.Resource;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public class AssetDatabaseBundleLabel
    {
        SearchField searchField;
        TreeViewState treeViewState;
        AssetDatabaseBundleTreeView treeView;
        public int BundleCount { get { return treeView.BundleCount; } }
        public event Action<IList<int>, IList<int>> OnBundleDelete
        {
            add { treeView.onBundleDelete += value; }
            remove { treeView.onBundleDelete -= value; }
        }
        public event Action OnAllBundleDelete
        {
            add { treeView.onAllBundleDelete += value; }
            remove { treeView.onAllBundleDelete -= value; }
        }
        public event Action<IList<int>> OnSelectionChanged
        {
            add { treeView.onBundleSelectionChanged += value; }
            remove { treeView.onBundleSelectionChanged -= value; }
        }
        public event Action<int, string> OnBundleRenamed
        {
            add { treeView.onBundleRenamed += value; }
            remove { treeView.onBundleRenamed -= value; }
        }
        public event Action<IList<string>, IList<int>> OnBundleSort
        {
            add { treeView.onBundleSort += value; }
            remove { treeView.onBundleSort -= value; }
        }
        public event Action<IList<int>> OnMarkAsSplittable
        {
            add { treeView.onMarkAsSplittable += value; }
            remove { treeView.onMarkAsSplittable -= value; }
        }
        public event Action<IList<int>> OnMarkAsUnsplittable
        {
            add { treeView.onMarkAsUnsplittable += value; }
            remove { treeView.onMarkAsUnsplittable -= value; }
        }
        public void OnEnable()
        {
            searchField = new SearchField();
            treeViewState = new TreeViewState();
            var multiColumnHeaderState = new MultiColumnHeader(ResourceWindowUtility.CreateResourceBundleMultiColumnHeader());
            treeView = new AssetDatabaseBundleTreeView(treeViewState, multiColumnHeaderState);
            searchField.downOrUpArrowKeyPressed += treeView.SetFocusAndEnsureSelectedItem;
        }
        public void OnGUI(Rect rect)
        {
            GUILayout.BeginVertical();
            DrawTreeView(rect);
            GUILayout.EndVertical();
        }
        public void Clear()
        {
            treeView.Clear();
        }
        public void Reload()
        {
            treeView.Reload();
        }
        public bool AddBundle(ResourceBundleInfo bundleInfo)
        {
            return treeView.AddBundle(bundleInfo);
        }
        public void SetSelection(IList<int> selectedIds)
        {
            treeView.SetSelection(selectedIds, TreeViewSelectionOptions.None);
        }
        void DrawTreeView(Rect rect)
        {
            var width = rect.width * ResourceEditorConstant.MIN_WIDTH;
            GUILayout.BeginVertical(GUILayout.MaxWidth(width));
            {
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(ResourceEditorConstant.SERACH, GUILayout.MaxWidth(48));
                    treeView.searchString = searchField.OnToolbarGUI(treeView.searchString);
                }
                GUILayout.EndHorizontal();
                Rect viewRect = GUILayoutUtility.GetRect(32, 8192, 32, 8192);
                treeView.OnGUI(viewRect);
            }
            GUILayout.EndVertical();
        }
    }
}
