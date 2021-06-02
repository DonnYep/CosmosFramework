using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Cosmos.Quark;
using System.Diagnostics;
namespace Cosmos.Test
{
    public class QuarkAssetTester: MonoBehaviour
    {
        void Start()
        {
            QuarkUtility.QuarkAssetLoadMode = QuarkAssetLoadMode.AssetDatabase;
            var go = CosmosEntry.ResourceManager.LoadPrefab(new AssetInfo() { AssetName= "YBot_LM_Local" },true);
            if (go == null)
                UnityEngine. Debug.LogError("go null");
        }
        private void OnGUI()
        {
            if (GUILayout.Button("CaptureScreenshot",GUILayout.Width(256),GUILayout.Height(64)))
            {
                Utility.Unity.CaptureScreenshot2PersistentDataPath( "YBot_LM_Local.png");
            }
            if (GUILayout.Button("OpenCaptureFolder", GUILayout.Width(256), GUILayout.Height(64)))
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.RevealInFinder(Application.persistentDataPath);
#endif
            }
        }
    }
}