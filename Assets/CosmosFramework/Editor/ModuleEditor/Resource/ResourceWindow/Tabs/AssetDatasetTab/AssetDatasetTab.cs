using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public class AssetDatasetTab : ResourceWindowTabBase
    {
        ReorderableList reorderableList;
        Vector2 scrollPosition;
        SearchField searchField;
        string searchText;
        List<string> extensionList = new List<string>();
        bool datasetAssigned = false;
        public override void OnGUI(Rect rect)
        {
            EditorGUILayout.LabelField("Resource Available Extenison", EditorStyles.boldLabel);
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Clear extensions"))
                {
                    if (!datasetAssigned)
                        return;
                    extensionList.Clear();
                    ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList.Clear();
                    EditorUtility.SetDirty(ResourceWindowDataProxy.ResourceDataset);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                if (GUILayout.Button("Reset extensions"))
                {
                    if (!datasetAssigned)
                        return;
                    var datasetExtensionList = ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList;
                    extensionList.Clear();
                    extensionList.AddRange(ResourceEditorConstant.Extensions);
                    datasetExtensionList.Clear();
                    datasetExtensionList.AddRange(ResourceEditorConstant.Extensions);
                    EditorUtility.SetDirty(ResourceWindowDataProxy.ResourceDataset);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Search extension", EditorStyles.boldLabel, GUILayout.MaxWidth(128));
                searchText = searchField.OnToolbarGUI(searchText);
                DrawSearchList();
            }
            GUILayout.EndHorizontal();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            reorderableList.DoLayoutList();
            EditorGUILayout.EndScrollView();
        }
        public override void OnEnable()
        {
            searchField = new SearchField();
            reorderableList = new ReorderableList(extensionList, typeof(string), true, true, true, true);
            reorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, $"Extension count : {extensionList.Count}");
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2f;
                rect.height = EditorGUIUtility.singleLineHeight;
                var srcExt = extensionList[index];
                var newExt = EditorGUI.TextField(rect, srcExt);
                if (string.IsNullOrEmpty(newExt))
                    return;
                var lowerStr = newExt.ToLower();
                if (!extensionList.Contains(lowerStr))
                {
                    if (!lowerStr.StartsWith("."))
                        return;
                    extensionList[index] = lowerStr;
                    if (!datasetAssigned)
                        return;
                    var datasetExtList = ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList;
                    datasetExtList[index] = lowerStr;
                    EditorUtility.SetDirty(ResourceWindowDataProxy.ResourceDataset);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            };
            if (ResourceWindowDataProxy.ResourceDataset != null)
            {
                datasetAssigned = true;
                extensionList.AddRange(ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList);
            }
            reorderableList.onAddCallback = (list) =>
            {
                list.list.Add("<none>");
            };
            reorderableList.onChangedCallback = (list) =>
            {
                if (!datasetAssigned)
                    return;
                var datasetExtList = ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList;
                datasetExtList.Clear();
                datasetExtList.AddRange(extensionList);
                EditorUtility.SetDirty(ResourceWindowDataProxy.ResourceDataset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            };
            reorderableList.onRemoveCallback = (list) =>
            {
                var removeIndex = list.index;
                list.list.RemoveAt(removeIndex);
            };
        }
        public override void OnDisable()
        {
            datasetAssigned = false;
            extensionList.Clear();
        }
        public override void OnDatasetAssign()
        {
            datasetAssigned = true;
            extensionList.AddRange(ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList);
        }
        public override void OnDatasetRefresh()
        {
            if (!datasetAssigned)
                return;
            extensionList.Clear();
            extensionList.AddRange(ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList);
        }
        public override void OnDatasetUnassign()
        {
            datasetAssigned = false;
            extensionList.Clear();
        }
        void DrawSearchList()
        {
            if (string.IsNullOrEmpty(searchText))
            {
                if (!datasetAssigned)
                    return;
                extensionList.Clear();
                extensionList.AddRange(ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList);
            }
            else
            {
                if (!datasetAssigned)
                    return;
                extensionList.Clear();
                var datasetExtList = ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList;
                var length = datasetExtList.Count;
                for (int i = 0; i < length; i++)
                {
                    var ext = datasetExtList[i];
                    if (ext.Contains(searchText.ToLower()))
                    {
                        extensionList.Add(ext);
                    }
                }
            }
        }
    }
}
