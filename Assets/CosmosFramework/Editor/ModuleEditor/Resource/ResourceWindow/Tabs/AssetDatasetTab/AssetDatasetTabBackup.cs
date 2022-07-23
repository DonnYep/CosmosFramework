using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public class AssetDatasetTabBackup : ResourceWindowTabBase
    {
        ReorderableList reorderableList;
        SerializedObject datasetSerializedObject;
        SerializedProperty sp_availableExtenisonList;
        Vector2 scrollPosition;
        SearchField searchField;
        string searchText;
        List<string> extensionList = new List<string>();
        bool datasetAssigned = false;
        public override void OnGUI(Rect rect)
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Clear extensions"))
                {
                    if (!datasetAssigned)
                        return;
                    sp_availableExtenisonList?.ClearArray();
                    datasetSerializedObject?.ApplyModifiedProperties();
                    EditorUtility.SetDirty(ResourceWindowDataProxy.ResourceDataset);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                if (GUILayout.Button("Reset extensions"))
                {
                    if (!datasetAssigned)
                        return;
                    sp_availableExtenisonList?.ClearArray();
                    var exts = ResourceEditorConstant.Extensions;
                    var length = exts.Length;
                    for (int i = length - 1; i >= 0; i--)
                    {
                        sp_availableExtenisonList.InsertArrayElementAtIndex(0);
                        var element = sp_availableExtenisonList.GetArrayElementAtIndex(0);
                        element.stringValue = exts[i];
                    }
                    datasetSerializedObject?.ApplyModifiedProperties();
                    EditorUtility.SetDirty(ResourceWindowDataProxy.ResourceDataset);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(16);
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Search extension", EditorStyles.boldLabel, GUILayout.MaxWidth(128));
                var searchStr = searchField.OnToolbarGUI(searchText, new GUILayoutOption[0]);
                if (searchStr != searchText)
                {
                    searchText = searchStr;
                    if (string.IsNullOrEmpty(searchText))
                    {

                    }
                }
            }
            GUILayout.EndHorizontal();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            datasetSerializedObject?.Update();
            reorderableList?.DoLayoutList();
            datasetSerializedObject?.ApplyModifiedProperties();
            EditorGUILayout.EndScrollView();
        }
        public override void OnEnable()
        {
            searchField = new SearchField();

        }
        public override void OnDisable()
        {

        }
        public override void OnDatasetAssign()
        {
            datasetAssigned = true;
            datasetSerializedObject = new SerializedObject(ResourceWindowDataProxy.ResourceDataset);
            sp_availableExtenisonList = datasetSerializedObject.FindProperty("resourceAvailableExtenisonList");
            reorderableList = new ReorderableList(datasetSerializedObject, sp_availableExtenisonList, true, true, true, true);
            reorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, $"Resource available extenison list，current count : {sp_availableExtenisonList.arraySize}");
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2f;
                rect.height = EditorGUIUtility.singleLineHeight;
                var element = sp_availableExtenisonList.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element);
            };
        }
        public override void OnDatasetUnassign()
        {
            datasetAssigned = false;
            reorderableList = null;
            datasetSerializedObject = null;
            sp_availableExtenisonList = null;
        }
        void CreateListView()
        {

        }
    }
}
