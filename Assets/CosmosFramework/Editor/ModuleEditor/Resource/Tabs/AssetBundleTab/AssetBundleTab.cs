using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Unity.EditorCoroutines.Editor;

namespace Cosmos.Editor.Resource
{
    public class AssetBundleTab
    {
        public Func<EditorCoroutine> BuildDataset;
        public const string AssetBundleTabDataName = "ResourceEditor_AsseBundleTabData.json";
        AssetBundleTabData tabData;

        public void OnEnable()
        {
        }
        public void OnDisable()
        {
        }
        public void OnGUI(Rect rect)
        {
            DrawLable();
        }
        public void OnDatasetAssign()
        {

        }
        public void OnDatasetUnassign()
        {

        }
        void DrawLable()
        {
            EditorGUILayout.LabelField("WIP");
        }
        void GetTabData()
        {
            try
            {
                tabData = EditorUtil.GetData<AssetBundleTabData>(AssetBundleTabDataName);
            }
            catch
            {
                tabData = new AssetBundleTabData();
                EditorUtil.SaveData(AssetBundleTabDataName, tabData);
            }
        }
        void SaveTabData()
        {
            EditorUtil.SaveData(AssetBundleTabDataName, tabData);
        }
    }
}
