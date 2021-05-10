using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace Cosmos.CosmosEditor
{
    public class QuarkAssetDragDropTab
	{
		QuarkAssetTreeView treeView;
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
			treeView = new QuarkAssetTreeView(treeViewState);
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
					CosmosEditorUtility.LogInfo("GameObject");
					foreach (Object obj in DragAndDrop.objectReferences)
					{
						CosmosEditorUtility.LogInfo("- " + obj);
					}
				}
				// Object outside project. It mays from File Explorer (Finder in OSX).
				else if (DragAndDrop.paths.Length > 0 && DragAndDrop.objectReferences.Length == 0)
				{
					CosmosEditorUtility.LogInfo("File");
					foreach (string path in DragAndDrop.paths)
					{
						CosmosEditorUtility.LogInfo("- " + path);
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
						if (obj is DefaultAsset)
						{
							//CosmosEditorUtility.LogInfo(path);
							treeView.AddPath(path);
						}
						else if (obj is MonoScript)
						{
						}
						else if (obj is Texture2D)
						{
						}
					}
				}
				// Log to make sure we cover all cases.
				else
				{
					CosmosEditorUtility.LogInfo("Out of reach");
					CosmosEditorUtility.LogInfo("Paths:");
					foreach (string path in DragAndDrop.paths)
					{
						CosmosEditorUtility.LogInfo("- " + path);
					}
					CosmosEditorUtility.LogInfo("ObjectReferences:");
					foreach (Object obj in DragAndDrop.objectReferences)
					{
						CosmosEditorUtility.LogInfo("- " + obj);
					}
				}
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("IncludeAssets：", new GUIStyle() { fontSize = 16 });
			DoToolbar();
			DoTreeView();
		}

		void DoToolbar()
		{
			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			GUILayout.Space(100);
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