using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Cosmos.QuarkAsset;
namespace Cosmos.CosmosEditor
{
    public class AssetBundleTab
    {
        BuildTarget buildTarget= BuildTarget.StandaloneWindows;
        string outputPath = "AssetBundles/StandaloneWindows";
        public void Clear()
        {

        }
        public void OnEnable()
        {

        }
        public void OnGUI()
        {
            DrawAssetBundleTab();
        }
        void DrawAssetBundleTab()
        {
            buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("BuildTarget", buildTarget);
            GUILayout.Space(16);
            outputPath = EditorGUILayout.TextField("OutputPath", outputPath);
            CosmosEditorUtility.DrawHorizontalContext(() =>
            {
                if (GUILayout.Button("Build"))
                {
                }
            });
        }
    }
}
