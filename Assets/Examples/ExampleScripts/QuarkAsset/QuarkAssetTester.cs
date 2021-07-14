using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Cosmos.Quark;
using System.Diagnostics;
using System.IO;
using Unity.Collections;

namespace Cosmos.Test
{
    public class QuarkAssetTester : MonoBehaviour
    {
        GameObject go;
        private void OnGUI()
        {
            if (GUILayout.Button("InstantiateByQuark", GUILayout.Width(256), GUILayout.Height(64)))
            {
                go = CosmosEntry.ResourceManager.LoadPrefab(new AssetInfo() { AssetName = "YBot_LM_Local" }, true);
                if (go == null)
                    UnityEngine.Debug.LogError("go null");
            }
        }
    }
}