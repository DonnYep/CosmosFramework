using Cosmos.Resource;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public class AssetDatabaseObjectLabel
    {
        SearchField searchField;
        TreeViewState treeViewState;
        AssetDatabaseObjectTreeView treeView;
        public int ObjectCount { get { return treeView.ObjectCount; } }
        public void OnEnable()
        {
            searchField = new SearchField();
            treeViewState = new TreeViewState();
            var multiColumnHeaderState = new MultiColumnHeader(ResourceBuilderWindowUtility.CreateResourceObjectMultiColumnHeader());
            treeView = new AssetDatabaseObjectTreeView(treeViewState, multiColumnHeaderState);
            searchField.downOrUpArrowKeyPressed += treeView.SetFocusAndEnsureSelectedItem;
        }
        public void Clear()
        {
            treeView.Clear();
        }
        public void Reload()
        {
            treeView.Reload();
        }
        public void AddObject(ResourceObjectInfo objectItem)
        {
            treeView.AddObject(objectItem);
        }
        public void OnGUI(Rect rect)
        {
            GUILayout.BeginVertical();
            DrawTreeView(rect);
            GUILayout.EndVertical();
        }
        void DrawTreeView(Rect rect)
        {
            var width = rect.width * ResourceBuilderWindowConstant.MAX_WIDTH;
            GUILayout.BeginVertical(GUILayout.MaxWidth(width));
            {
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(ResourceBuilderWindowConstant.SERACH, GUILayout.MaxWidth(48));
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
