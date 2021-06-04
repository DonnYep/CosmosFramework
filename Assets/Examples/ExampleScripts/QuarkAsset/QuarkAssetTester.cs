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
        void Start()
        {
            var go = CosmosEntry.ResourceManager.LoadPrefab(new AssetInfo() { AssetName = "YBot_LM_Local" }, true);
            if (go == null)
                UnityEngine.Debug.LogError("go null");
        }
        private void OnGUI()
        {
            if (GUILayout.Button("CaptureScreenshot", GUILayout.Width(256), GUILayout.Height(64)))
            {
                var fullPath = Path.Combine(Application.persistentDataPath, "YBot_LM_Local.png");
                var texture = ScreenCapture.CaptureScreenshotAsTexture();
                var textureBytes = texture.EncodeToPNG();
                File.WriteAllBytes(fullPath, textureBytes);

                //Texture2D texture2D = new Texture2D(texture.width, texture.height);
                //texture2D.LoadImage(textureBytes);
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