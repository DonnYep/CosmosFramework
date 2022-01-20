using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if UNITY_EDITOR
[CustomEditor(typeof(ItemDataSet),true)]
public class ItemDataSetEditor : Editor
{
    SerializedObject targetObject;
    ItemDataSet itemDataSet;
    private void OnEnable()
    {
        itemDataSet = target as ItemDataSet;
        targetObject = new SerializedObject(itemDataSet);
    }
    public override void OnInspectorGUI()
    {
        GUILayout.BeginVertical("box");
        DrawCFButton();
        GUILayout.EndVertical();
        OnValidate();
        base.OnInspectorGUI();
    }
    void OnValidate()
    {
        itemDataSet.ItemNumber = Mathf.Clamp(itemDataSet.ItemNumber, 0, 999);
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
            itemDataSet.Preview();
        }
        if (GUILayout.Button("Reset", GUILayout.Height(20)))
        {
            var canReset = EditorUtility.DisplayDialog("Reset ScriptableObject", "You will reset ScriptableObject Properties", "Reset", "Cancel");
            if (canReset)
                itemDataSet.Dispose();
        }
        GUILayout.EndHorizontal();
    }
}
#endif
