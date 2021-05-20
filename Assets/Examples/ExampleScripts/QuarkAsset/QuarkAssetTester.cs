using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Cosmos.QuarkAsset;
using System.Diagnostics;
namespace Cosmos.Test
{
    public class QuarkAssetTester: MonoBehaviour
    {
        void Start()
        {
            CosmosEntry.ResourceManager.AddOrUpdateBuildInLoadHelper(Resource.ResourceLoadMode.QuarkAsset, new QuarkAssetLoaderHelper());
            CosmosEntry.ResourceManager.SwitchBuildInLoadMode(Resource.ResourceLoadMode.QuarkAsset);
            var go = CosmosEntry.ResourceManager.LoadPrefab(new AssetInfo() { AssetName= "YBot_LM_Local" },true);
            if (go == null)
                UnityEngine. Debug.LogError("go null");
        }
        private void OnGUI()
        {
            if (GUILayout.Button("CaptureScreenshot",GUILayout.Width(256),GUILayout.Height(64)))
            {
                Utility.Unity.CaptureScreen2PersistentDataPath( "YBot_LM_Local.png");
            }
            if (GUILayout.Button("OpenCaptureFolder", GUILayout.Width(256), GUILayout.Height(64)))
            {
                //UnityEditor.EditorUtility.RevealInFinder(Application.persistentDataPath);
            }
        }
    }
}