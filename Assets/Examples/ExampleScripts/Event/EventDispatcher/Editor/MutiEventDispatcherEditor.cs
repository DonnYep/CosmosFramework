using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if UNITY_EDITOR
namespace CosmosEditor
{
    [CustomEditor(typeof(MutiEventDispatcher))]
    public class MutiEventDispatcherEditor : UnityEditor.Editor
    {
        SerializedObject targetObject;
        MutiEventDispatcher mutiEventDispatcher;
        SerializedProperty keyContentDataSet;
        StringContent stringContentResult;
        private void OnEnable()
        {
            mutiEventDispatcher = target as MutiEventDispatcher;
            targetObject = new SerializedObject(mutiEventDispatcher);
            keyContentDataSet = targetObject.FindProperty("keyContentDataSet");

        }
        void Init()
        {

        }
        void Draw()
        {

        }

        bool CanDraw()
        {
            if (stringContentResult.Content.Length == 0)
                return false;
            return true;
        }
        int selectedIndexs = 0;
        public override void OnInspectorGUI()
        {
            targetObject.Update();
            //TODO解算二进制
            selectedIndexs = EditorGUILayout.MaskField("EventKeys", selectedIndexs, new string[] { "AAA", "BBB", "CCC", "DDD", "EEE", "FFF", "GGG" });
            Utility.Debug.LogInfo("selectedIndexs>>" + selectedIndexs);
            var byteResult = Utility.Converter.ConvertToString(selectedIndexs);
            Utility.Debug.LogInfo("byteResult>>" + byteResult);

            targetObject.ApplyModifiedProperties();
        }
    }
}
#endif
