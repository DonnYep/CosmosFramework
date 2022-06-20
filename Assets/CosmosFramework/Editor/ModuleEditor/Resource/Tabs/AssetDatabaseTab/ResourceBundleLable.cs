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
        public void OnEnable()
        {
            searchField = new SearchField();
            treeViewState = new TreeViewState();
            treeView = new ResourceBundleTreeView(treeViewState);
            searchField.downOrUpArrowKeyPressed += treeView.SetFocusAndEnsureSelectedItem;
        }
        public void OnGUI()
        {
            GUILayout.BeginVertical();
            DrawTreeView();
            GUILayout.EndVertical();
        }
        public void Clear()
        {
            treeView.Clear();
        }
        public void AddBundle(ResourceBundleInfo bundleInfo)
        {
            treeView.AddBundle(bundleInfo);
        }
        void DrawTreeView()
        {
            GUILayout.Label("Bundle lable");
            GUILayout.BeginVertical("box");
            treeView.searchString = searchField.OnToolbarGUI(treeView.searchString);
            Rect rect = GUILayoutUtility.GetRect(32, 8192, 32, 8192);
            treeView.OnGUI(rect);
            GUILayout.EndVertical();
        }


    }
}
