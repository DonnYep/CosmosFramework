using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Cosmos.CosmosEditor
{
    [CustomEditor(typeof (MutiEventDispatcher))]
    public class MutiEventDispatcherEditor : Editor
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
            selectedIndexs = EditorGUILayout.MaskField("EventKeys",selectedIndexs,new string[] { "AAA","BBB","CCC","DDD","EEE","FFF","GGG"});
            Utility.DebugLog("selectedIndexs>>" + selectedIndexs);
            var byteResult = Utility.Encode.ConvertToBinary(selectedIndexs);
            Utility.DebugLog("byteResult>>" + byteResult);

            targetObject.ApplyModifiedProperties();
        }
    }
}