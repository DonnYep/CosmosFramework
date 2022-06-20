using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public class ResourceObjectLable
    {
        SearchField searchField;
        TreeViewState treeViewState;
        ResourceObjectTreeView treeView;
        public void OnEnable()
        {
            searchField = new SearchField();
            treeViewState = new TreeViewState();
            treeView = new ResourceObjectTreeView(treeViewState);
            searchField.downOrUpArrowKeyPressed += treeView.SetFocusAndEnsureSelectedItem;
        }
        public void Clear()
        {
            treeView.Clear();
        }
        public void AddObject(ResourceObjectInfo objectInfo)
        {
            treeView.AddObject(objectInfo);
        }
        public void RemoveObject(ResourceObjectInfo objectInfo)
        {
            treeView.RemoveObject(objectInfo);
        }
        public void OnGUI()
        {
            GUILayout.BeginVertical();
            DrawTreeView();
            GUILayout.EndVertical();
        }
        void DrawTreeView()
        {
            GUILayout.Label("Object lable");
            GUILayout.BeginVertical("box");
            treeView.searchString = searchField.OnToolbarGUI(treeView.searchString);
            Rect rect = GUILayoutUtility.GetRect(32, 8192, 32, 8192);
            treeView.OnGUI(rect);
            GUILayout.EndVertical();
        }
    }
}
