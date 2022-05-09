using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Linq;
namespace Quark.Editor
{
    public class QuarkAssetObjectSearchLable
    {
        QuarkAssetObjectTreeView treeView;
        TreeViewState treeViewState;
        SearchField searchField;

        public void OnEnable()
        {
            searchField = new SearchField();
            treeViewState = new TreeViewState();
            treeView = new QuarkAssetObjectTreeView(treeViewState);
            searchField.downOrUpArrowKeyPressed += treeView.SetFocusAndEnsureSelectedItem;
        }
        void Refresh()
        {
            treeView.ClearPath();
            if (QuarkEditorDataProxy.QuarkAssetDataset != null)
            {
                var list = QuarkEditorDataProxy.QuarkAssetDataset.QuarkAssetObjectList;
                treeView.AddPaths(list.Select(o => o.AssetPath));
            }
        }
        public void OnGUI()
        {
            GUILayout.BeginVertical();
            DrawToolbar();
            DrawTreeView();
            Refresh();
            GUILayout.EndVertical();
        }
        public void Clear()
        {
            treeView.ClearPath();
        }
        void DrawToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            treeView.searchString = searchField.OnToolbarGUI(treeView.searchString);
            GUILayout.EndHorizontal();
        }
        void DrawTreeView()
        {
            GUILayout.BeginVertical("box");
            Rect rect = GUILayoutUtility.GetRect(32, 8192, 32, 8192);
            treeView.OnGUI(rect);
            GUILayout.EndVertical();
        }
    }
}
