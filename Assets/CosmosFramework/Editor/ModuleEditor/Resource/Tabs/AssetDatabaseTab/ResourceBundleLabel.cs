using Cosmos.Resource;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public class ResourceBundleLabel
    {
        SearchField searchField;
        TreeViewState treeViewState;
        ResourceBundleTreeView treeView;

        public event Action<IList<int>> OnDelete
        {
            add { treeView.onDelete += value; }
            remove { treeView.onDelete -= value; }
        }
        public event Action OnAllDelete
        {
            add { treeView.onAllDelete += value; }
            remove { treeView.onAllDelete -= value; }
        }
        public event Action<IList<int>> OnSelectionChanged
        {
            add { treeView.onSelectionChanged += value; }
            remove { treeView.onSelectionChanged -= value; }
        }
        public event Action<int, string> OnRenameBundle
        {
            add { treeView.onRenameBundle += value; }
            remove { treeView.onRenameBundle -= value; }
        }
        public event Action<IList<string>> OnSort
        {
            add { treeView.onSort += value; }
            remove { treeView.onSort -= value; }
        }
        public void OnEnable()
        {
            searchField = new SearchField();
            treeViewState = new TreeViewState();
            var multiColumnHeaderState = new MultiColumnHeader(ResourceWindowUtility.CreateResourceBundleMultiColumnHeader());
            treeView = new ResourceBundleTreeView(treeViewState, multiColumnHeaderState);
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
            treeView.SetSelection(selectedIds);
        }
        void DrawTreeView(Rect rect)
        {
            var width = rect.width * 0.382f;
            GUILayout.BeginVertical(GUILayout.MaxWidth(width));
            {
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Search bundle", EditorStyles.boldLabel, GUILayout.MaxWidth(128));
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
