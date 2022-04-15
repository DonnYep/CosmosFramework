using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using Cosmos.Editor;

namespace Quark.Editor
{
    public class QuarkDirectoriesOperation
    {
        QuarkDirectoriesTreeView quarjTreeView;
        TreeViewState treeViewState;
        SearchField searchField;

        public void Clear()
        {
            quarjTreeView.Clear();
        }
        public void OnEnable()
        {
            if (treeViewState == null)
                treeViewState = new TreeViewState();
            quarjTreeView = new QuarkDirectoriesTreeView(treeViewState);
            searchField = new SearchField();
            searchField.downOrUpArrowKeyPressed += quarjTreeView.SetFocusAndEnsureSelectedItem;
        }
        public void OnGUI(Rect rect)
        {
            if (UnityEngine.Event.current.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                UnityEngine.Event.current.Use();
            }
            else if (UnityEngine.Event.current.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                if (DragAndDrop.paths.Length == 0 && DragAndDrop.objectReferences.Length > 0)
                {
                    //EditorUtil.Debug.LogInfo("GameObject");
                    foreach (Object obj in DragAndDrop.objectReferences)
                    {
                        EditorUtil.Debug.LogInfo("- " + obj);
                    }
                }
                // Object outside project. It mays from File Explorer (Finder in OSX).
                else if (DragAndDrop.paths.Length > 0 && DragAndDrop.objectReferences.Length == 0)
                {
                    //EditorUtil.Debug.LogInfo("File");
                    foreach (string path in DragAndDrop.paths)
                    {
                        EditorUtil.Debug.LogInfo("- " + path);
                    }
                }
                // Unity Assets including folder.
                else if (DragAndDrop.paths.Length == DragAndDrop.objectReferences.Length)
                {
                    for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                    {
                        Object obj = DragAndDrop.objectReferences[i];
                        string path = DragAndDrop.paths[i];
                        // Folder.
                        //if (obj is DefaultAsset)
                        //{
                        //}
                        //else if (!(obj is MonoScript))
                        //{
                        //	treeView.AddPath(path);
                        //}
                        if (!(obj is MonoScript)/*&&!(obj is SceneAsset)*/)
                        {
                            quarjTreeView.AddPath(path);
                        }
                    }
                }
                // Log to make sure we cover all cases.
                else
                {
                    EditorUtil.Debug.LogInfo("Out of reach");
                    EditorUtil.Debug.LogInfo("Paths:");
                    foreach (string path in DragAndDrop.paths)
                    {
                        EditorUtil.Debug.LogInfo("- " + path);
                    }
                    EditorUtil.Debug.LogInfo("ObjectReferences:");
                    foreach (Object obj in DragAndDrop.objectReferences)
                    {
                        EditorUtil.Debug.LogInfo("- " + obj);
                    }
                }
            }
            DoToolbar();
            DoTreeView();
        }

        void DoToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Space(128);
            GUILayout.FlexibleSpace();
            quarjTreeView.searchString = searchField.OnToolbarGUI(quarjTreeView.searchString);
            GUILayout.EndHorizontal();
        }
        void DoTreeView()
        {
            Rect rect = GUILayoutUtility.GetRect(0, 8192, 0, 8192);
            quarjTreeView.OnGUI(rect);
        }
    }
}