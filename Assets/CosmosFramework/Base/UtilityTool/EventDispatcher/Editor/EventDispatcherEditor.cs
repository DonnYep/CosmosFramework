using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Cosmos.CosmosEditor
{
    [CustomEditor(typeof(EventDispatcher))]
    public class EventDispatcherEditor:Editor
    {
        SerializedObject targetObject;
        EventDispatcher eventDispatcher;
        /// <summary>
        /// keyContent表示StringContent类型，
        /// selectedKeyContent表示被选中后得到的字段
        /// </summary>
        SerializedProperty keyContentDataSet, selectedKeyContent, previousSelectedIndex;
        StringContent stringContentResult;
        int selectedIndex=0 ;
        int contentSize = -1;

        int[] OptionValues { get { return optionValues.ToArray(); } }
        List<int> optionValues = new List<int>();

        string[] DisplayedOptions { get { return displayedOptions.ToArray(); } }
        List<string> displayedOptions = new List<string>();

        private void OnEnable()
        {
            eventDispatcher = target as EventDispatcher;
            targetObject = new SerializedObject(this);
            keyContentDataSet = targetObject.FindProperty("keyContentDataSet");
            selectedKeyContent = targetObject.FindProperty("selectedKeyContent");
            previousSelectedIndex = targetObject.FindProperty("previousSelectedIndex");
            Init();
        }
        void Init()
        {
            stringContentResult = keyContentDataSet.objectReferenceValue as StringContent;
            if (stringContentResult == null || !CanDraw())
            {
                previousSelectedIndex.intValue = 0;
                return;
            }
            if (!IsEqual())
                Refresh();
            if (previousSelectedIndex.intValue < displayedOptions.Count)
                selectedIndex = previousSelectedIndex.intValue;
            else
                previousSelectedIndex.intValue = 0;
        }
        public override void OnInspectorGUI()
        {
            targetObject.Update();
            Draw();
            targetObject.ApplyModifiedProperties();
        }
        void Draw()
        {
            EditorGUILayout.PropertyField(keyContentDataSet);
            stringContentResult = keyContentDataSet.objectReferenceValue as StringContent;
            if (stringContentResult == null || !CanDraw())
            {
                EditorGUILayout.HelpBox("KeyContentDataSet or content is empty!", MessageType.Error);
                return;
            }
            if (!IsEqual())
            {
                Refresh();
            }
            selectedIndex = EditorGUILayout.IntPopup("EventKey", selectedIndex, DisplayedOptions, OptionValues);
            previousSelectedIndex.intValue = selectedIndex;
            selectedKeyContent.stringValue = displayedOptions[selectedIndex];
        }

        void Refresh()
        {
            optionValues.Clear();
            displayedOptions.Clear();
            contentSize = -1;
            for (int i = 0; i < stringContentResult.Content.Length; i++)
            {
                if (!displayedOptions.Contains(stringContentResult.Content[i]) && !string.IsNullOrEmpty(stringContentResult.Content[i]))
                {
                    displayedOptions.Add(stringContentResult.Content[i]);
                    contentSize++;
                    optionValues.Add(contentSize);
                }
            }
        }
        bool IsEqual()
        {
            if (stringContentResult.Content.Length == 0)
                return false;
            else if (optionValues.Count == stringContentResult.Content.Length)
            {
                for (int i = 0; i < stringContentResult.Content.Length; i++)
                {
                    if (stringContentResult.Content[i].ToString() == optionValues[i].ToString())
                        return true;
                }
                return false;
            }
            else return false;
        }
        bool CanDraw()
        {
            if (stringContentResult.Content.Length == 0)
                return false;
            return true;
        }

    }
}