using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if UNITY_EDITOR

[CustomEditor(typeof(InventoryDataSet))]
public class InventoryDataSetEditor : Editor
{
    InventoryDataSet inventoryDataSet;
    SerializedObject targetObject;
    private void OnEnable()
    {
        inventoryDataSet = target as InventoryDataSet;
        targetObject = new SerializedObject(inventoryDataSet);
    }
    public override void OnInspectorGUI()
    {
        GUILayout.BeginVertical("box");
        DrawCFButton();
        GUILayout.EndVertical();
        LimitInventoryCapacity();
        base.OnInspectorGUI();
    }
    private void LimitInventoryCapacity()
    {
        inventoryDataSet.InventoryCapacity = Mathf.Clamp(inventoryDataSet.InventoryCapacity,0, 32);
        if (inventoryDataSet.ItemDataSets.Count == inventoryDataSet.InventoryCapacity)
            return;
        if (inventoryDataSet.ItemDataSets.Count > inventoryDataSet.InventoryCapacity) 
        {
            for (int i = 0; i < inventoryDataSet.ItemDataSets.Count; i++)
            {
                if (i >= inventoryDataSet.InventoryCapacity)
                    inventoryDataSet.ItemDataSets.RemoveAt(i);
            }
        }
        else if(inventoryDataSet.ItemDataSets.Count < inventoryDataSet.InventoryCapacity)
        {
            for (int i = 0; i < inventoryDataSet.InventoryCapacity; i++)
            {
                if (i >= inventoryDataSet.ItemDataSets.Count)
                    inventoryDataSet.ItemDataSets.Add(null);
            }
        }
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
            inventoryDataSet.Preview();
        }
        if (GUILayout.Button("Dispose", GUILayout.Height(20)))
        {
            var canReset = EditorUtility.DisplayDialog("Reset ScriptableObject", "You will reset ScriptableObject Properties", "Reset", "Cancel");
            if (canReset)
                inventoryDataSet.Dispose();
        }
        GUILayout.EndHorizontal();
    }
}
#endif

