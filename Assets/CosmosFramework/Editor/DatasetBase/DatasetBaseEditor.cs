using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if UNITY_EDITOR
namespace Cosmos
{
    /// <summary>
    /// 子类也继承按钮
    /// </summary>
    [CustomEditor(typeof(DatasetBase),true)]
    public class DatasetBaseEditor : Editor
    {
        DatasetBase cfDataSet;
        private void OnEnable()
        {
            cfDataSet = target as DatasetBase;
        }
        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical("box");
            DrawCFButton();
            GUILayout.EndVertical();
            base.OnInspectorGUI();
        }
        /// <summary>
        /// 绘制公共功能按钮
        /// </summary>
       protected virtual void DrawCFButton()
        {
            EditorGUILayout.HelpBox("Preview预览，Reset按钮执行清空下列所有数值", MessageType.Info,true);
            //GUILayout.Label("Preview预览，Reset按钮执行清空下列所有数值");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Preview", GUILayout.Height(20)))
            {
                cfDataSet.Preview();
            }
            if (GUILayout.Button("Dispose", GUILayout.Height(20)))
            {
               var canReset=UnityEditor. EditorUtility.DisplayDialog("Reset ScriptableObject", "You will reset ScriptableObject Properties", "Reset", "Cancel");
                if (canReset)
                    cfDataSet.Dispose();
            }
            GUILayout.EndHorizontal();
        }
    }
}
#endif
