using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public class ResourceBundleLable
    {
        SearchField searchField;
        TreeViewState treeViewState;
        ResourceBundleTreeView treeView;

        public event Action<List<ResourceBundleInfo>> OnDelete
        {
            add { treeView.onDelete += value; }
            remove { treeView.onDelete -= value; }
        }
        public event Action OnAllDelete
        {
            add { treeView.onAllDelete += value; }
            remove { treeView.onAllDelete -= value; }
        }
        public event Action<int> onBundleClick
        {
            add { treeView.onBundleClick += value; }
            remove { treeView.onBundleClick -= value; }
        }
        public void OnEnable()
        {
            searchField = new SearchField();
            treeViewState = new TreeViewState();
            var multiColumnHeaderState = new MultiColumnHeader(ResourceEditorUtil.CreateResourceBundleMultiColumnHeader());
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
        public bool AddBundle(ResourceBundleInfo bundleInfo)
        {
            return treeView.AddBundle(bundleInfo);
        }
        public void SetSetSelectionBundle(int index)
        {
            treeView.SetSelection(new int[] { index });
        }
        void DrawTreeView(Rect rect)
        {
            var width= rect.width * 0.382f;
            GUILayout.BeginVertical(GUILayout.MaxWidth(width));
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Search bundle");
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
