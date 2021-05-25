using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace Cosmos.CosmosEditor
{
    public class IncludeDirectoriesOperation
	{
		IncludeDirectoriesTreeView treeView;
		TreeViewState treeViewState;
		SearchField searchField;

		public List<string> FolderPath { get { return treeView.PathList; }set { treeView.PathList = value; } }
		public void Clear()
        {
			treeView.Clear();
		}
		public void OnEnable()
        {
			if (treeViewState == null)
				treeViewState = new TreeViewState();
			treeView = new IncludeDirectoriesTreeView(treeViewState);
			searchField = new SearchField();
			searchField.downOrUpArrowKeyPressed += treeView.SetFocusAndEnsureSelectedItem;
		}
        public void OnGUI()
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
					EditorUtilities.Debug.LogInfo("GameObject");
					foreach (Object obj in DragAndDrop.objectReferences)
					{
						EditorUtilities.Debug.LogInfo("- " + obj);
					}
				}
				// Object outside project. It mays from File Explorer (Finder in OSX).
				else if (DragAndDrop.paths.Length > 0 && DragAndDrop.objectReferences.Length == 0)
				{
					EditorUtilities.Debug.LogInfo("File");
					foreach (string path in DragAndDrop.paths)
					{
						EditorUtilities.Debug.LogInfo("- " + path);
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
						if (!(obj is MonoScript))
						{
							treeView.AddPath(path);
						}
					}
				}
				// Log to make sure we cover all cases.
				else
				{
					EditorUtilities.Debug.LogInfo("Out of reach");
					EditorUtilities.Debug.LogInfo("Paths:");
					foreach (string path in DragAndDrop.paths)
					{
						EditorUtilities.Debug.LogInfo("- " + path);
					}
					EditorUtilities.Debug.LogInfo("ObjectReferences:");
					foreach (Object obj in DragAndDrop.objectReferences)
					{
						EditorUtilities.Debug.LogInfo("- " + obj);
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
			treeView.searchString = searchField.OnToolbarGUI(treeView.searchString);
			GUILayout.EndHorizontal();
		}
		void DoTreeView()
		{
			Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
			treeView.OnGUI(rect);
		}
	}
}