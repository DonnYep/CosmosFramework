using Cosmos.Resource;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public class AssetDatabaseBundleDetailLabel
    {
        SearchField searchField;
        TreeViewState treeViewState;
        AssetDatabaseBundleDetailTreeView treeView;
        public int BundleDetailCount { get { return treeView.BundleDetailCount; } }
        public void OnEnable()
        {
            searchField = new SearchField();
            treeViewState = new TreeViewState();
            treeView = new AssetDatabaseBundleDetailTreeView(treeViewState);
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
        public void AddBundle(ResourceBundleInfo bundleInfo)
        {
            treeView.AddBundle(bundleInfo);
        }
        public void SetSelection(IList<int> selectedIds)
        {
            var bundleInfos = ResourceBuilderWindowDataProxy.ResourceDataset.ResourceBundleInfoList;
            var idlen = selectedIds.Count;
            treeView.Clear();
            for (int i = 0; i < idlen; i++)
            {
                var id = selectedIds[i];
                if (id >= bundleInfos.Count)
                    continue;
                var bundleInfo = bundleInfos[id];
                treeView.AddBundle(bundleInfo);
            }
            treeView.Reload();
        }
        void DrawTreeView(Rect rect)
        {
            GUILayout.BeginVertical(GUILayout.Width(rect.width));
            {
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(ResourceBuilderWindowConstant.SERACH, GUILayout.MaxWidth(48));
                    treeView.searchString = searchField.OnToolbarGUI(treeView.searchString);
                    if (GUILayout.Button("ExpandAll", EditorStyles.miniButton, GUILayout.MaxWidth(92)))
                    {
                        treeView.ExpandAll();
                    }
                    if (GUILayout.Button("CollapseAll", EditorStyles.miniButton, GUILayout.MaxWidth(92)))
                    {
                        treeView.CollapseAll();
                    }
                }
                GUILayout.EndHorizontal();
                Rect viewRect = GUILayoutUtility.GetRect(32, 8192, 32, 8192);
                treeView.OnGUI(viewRect);
            }
            GUILayout.EndVertical();
        }
    }
}
