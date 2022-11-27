using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public class ResourceObjectLabel
    {
        SearchField searchField;
        TreeViewState treeViewState;
        ResourceObjectTreeView treeView;
        public void OnEnable()
        {
            searchField = new SearchField();
            treeViewState = new TreeViewState();
            var multiColumnHeaderState = new MultiColumnHeader(ResourceWindowUtility.CreateResourceObjectMultiColumnHeader());
            treeView = new ResourceObjectTreeView(treeViewState, multiColumnHeaderState);
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
        public void AddObject(ResourceObjectInfo objectInfo)
        {
            treeView.AddObject(objectInfo);
        }
        public void OnGUI(Rect rect)
        {
            GUILayout.BeginVertical();
            DrawTreeView(rect);
            GUILayout.EndVertical();
        }
        void DrawTreeView(Rect rect)
        {
            GUILayout.BeginVertical(GUILayout.MaxWidth(rect.width * 0.618f));
            {
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Search object", EditorStyles.boldLabel,GUILayout.MaxWidth(128));
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
