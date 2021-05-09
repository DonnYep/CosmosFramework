using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
namespace Cosmos.CosmosEditor
{
    public class DragAndDropEditorWindow : EditorWindow
	{
		//[MenuItem("Window/Cosmos/Drag And Drop")]
		public static void Open()
		{
			GetWindow<DragAndDropEditorWindow>(false, "DragDrop", true);
		}

		private void OnGUI()
		{
			if (UnityEngine.Event.current.type == EventType.DragUpdated)
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				UnityEngine.Event.current.Use();
			}
			else if (UnityEngine.Event.current.type == EventType.DragPerform)
			{
				// To consume drag data.
				DragAndDrop.AcceptDrag();

				// GameObjects from hierarchy.
				if (DragAndDrop.paths.Length == 0 && DragAndDrop.objectReferences.Length > 0)
				{
					Debug.Log("GameObject");
					foreach (Object obj in DragAndDrop.objectReferences)
					{
						Debug.Log("- " + obj);
					}
				}
				// Object outside project. It mays from File Explorer (Finder in OSX).
				else if (DragAndDrop.paths.Length > 0 && DragAndDrop.objectReferences.Length == 0)
				{
					Debug.Log("File");
					foreach (string path in DragAndDrop.paths)
					{
						Debug.Log("- " + path);
					}
				}
				// Unity Assets including folder.
				else if (DragAndDrop.paths.Length == DragAndDrop.objectReferences.Length)
				{
					Debug.Log("UnityAsset");
					for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
					{
						Object obj = DragAndDrop.objectReferences[i];
						string path = DragAndDrop.paths[i];
						Debug.Log(obj.GetType().Name);

						// Folder.
						if (obj is DefaultAsset)
						{
							Debug.Log(path);
						}
						// C# or JavaScript.
						else if (obj is MonoScript)
						{
							Debug.Log(path + "\n" + obj);
						}
						else if (obj is Texture2D)
						{

						}

					}
				}
				// Log to make sure we cover all cases.
				else
				{
					Debug.Log("Out of reach");
					Debug.Log("Paths:");
					foreach (string path in DragAndDrop.paths)
					{
						Debug.Log("- " + path);
					}

					Debug.Log("ObjectReferences:");
					foreach (Object obj in DragAndDrop.objectReferences)
					{
						Debug.Log("- " + obj);
					}
				}
			}
		}
	}
}