using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Cosmos
{
#if UNITY_EDITOR
    [CustomEditor(typeof(StringContent))]
    public class StringContentEditor : Editor
    {
        StringContent stringContent;
        SerializedObject targetObject;
        private void OnEnable()
        {
            stringContent = target as StringContent;
            targetObject = new SerializedObject(stringContent);
       }
        /// <summary>
        /// 限制数组长度
        /// </summary>
        void LimitContentCapacity()
        {
            if (stringContent.Content.Length >= 32)
            {
                Array.Resize(ref stringContent.content, 32);
            }
        }
        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical("box");
            DrawCFButton();
            GUILayout.EndVertical();
            LimitContentCapacity();
            base.OnInspectorGUI();
        }
        /// <summary>
        /// 绘制公共功能按钮
        /// </summary>
        void DrawCFButton()
        {
            EditorGUILayout.HelpBox("Preview预览，Reset按钮执行清空下列所有数值", MessageType.Info, true);
            //GUILayout.Label("Preview预览，Reset按钮执行清空下列所有数值");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Preview", GUILayout.Height(20)))
            {
               stringContent.Preview();
            }
            if (GUILayout.Button("Dispose", GUILayout.Height(20)))
            {
                var canReset =UnityEditor. EditorUtility.DisplayDialog("Reset ScriptableObject", "You will reset ScriptableObject Properties", "Reset", "Cancel");
                if (canReset)
                    stringContent.Dispose();
            }
            GUILayout.EndHorizontal();
        }
    }
#endif
}