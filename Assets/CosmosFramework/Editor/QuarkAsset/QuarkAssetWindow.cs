using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Cosmos.QuarkAsset;
namespace Cosmos.CosmosEditor
{
    public class QuarkAssetWindow : EditorWindow
    {
        enum AssetInfoBar : int
        {
            BuildAsset = 0,
            PlatfromBuild = 1
        }
        static string[] projectFiles = new string[]
        {
        ".3ds",".bmp",".blend",".eps",".exif",".gif",".icns",".ico",".jpeg",
        ".jpg",".ma",".max",".mb",".pcx",".png",".psd",".svg",".controller",
        ".wav",".txt",".prefab",".xml",".shadervariants",".shader",".anim",
        ".unity",".mat",".mask",".overrideController",".tif",".spriteatlas"
        };
        int selectedBar = 0;
        string[] barArray = new string[] { "FastEditor", "AssetBundle" };
        static int filterLength;
        static QuarkAssetDataset QuarkAssetDataset { get { return QuarkAssetEditorUtility.Dataset.QuarkAssetDatasetInstance; } }
        /// <summary>
        /// Editor配置文件；
        /// </summary>
        static QuarkAssetConfigData quarkAssetConfigData;
        const string QuarkAssetConfigDataFileName = "QuarkAssetConfigData.json";
        Vector2 dirScrollPos;

        public QuarkAssetWindow()
        {
            this.titleContent = new GUIContent("QuarkAsset");
        }
        [MenuItem("Window/Cosmos/QuarkAsset")]
        public static void OpenWindow()
        {
            var window = GetWindow<QuarkAssetWindow>();
            ((EditorWindow)window).maxSize = CosmosEditorUtility.CosmosMaxWinSize;
            ((EditorWindow)window).minSize = CosmosEditorUtility.CosmosDevWinSize;
        }
        [InitializeOnLoadMethod]
        static void InitData()
        {
            filterLength = Application.dataPath.Length - 6;
        }
        private void OnEnable()
        {
            try
            {
                quarkAssetConfigData = CosmosEditorUtility.GetData<QuarkAssetConfigData>(QuarkAssetConfigDataFileName);
                if (quarkAssetConfigData.IncludeDirectories == null)
                    quarkAssetConfigData.IncludeDirectories = new List<string>();
            }
            catch
            {
                quarkAssetConfigData = new QuarkAssetConfigData();
                quarkAssetConfigData.IncludeDirectories = new List<string>();
            }
        }
        private void OnGUI()
        {
            selectedBar = GUILayout.Toolbar(selectedBar, barArray, GUILayout.Height(24));
            GUILayout.Space(16);
            var bar = (AssetInfoBar)selectedBar;
            switch (bar)
            {
                case AssetInfoBar.BuildAsset:
                    DrawBuildAsset();
                    break;
                case AssetInfoBar.PlatfromBuild:
                    DrawPlatformBuild();
                    break;
            }
        }
        void DrawBuildAsset()
        {
            GUILayout.BeginVertical();
            CosmosEditorUtility.DrawHorizontalContext(() =>
            {
                EditorGUILayout.LabelField("LoadMode", GUILayout.Width(128));
                QuarkAssetDataset.QuarkAssetLoadMode = (QuarkAssetLoadMode)EditorGUILayout.EnumPopup(QuarkAssetDataset.QuarkAssetLoadMode);
            });
            CosmosEditorUtility.DrawHorizontalContext(() =>
            {
                EditorGUILayout.LabelField("GenerateAssetPath", GUILayout.Width(128));
                quarkAssetConfigData.GenerateAssetPathCode = EditorGUILayout.Toggle(quarkAssetConfigData.GenerateAssetPathCode);
            });
            CosmosEditorUtility.DrawVerticalContext(() =>
            {
                //CosmosEditorUtility.DrawHorizontalContext(() =>
                //{
                //    if (GUILayout.Button("AddNewPath", GUILayout.Height(24), GUILayout.Width(92)))
                //    {
                //        quarkAssetConfigData.IncludeDirectories.Add("");
                //    }
                //    if (GUILayout.Button("RemovePath", GUILayout.Height(24), GUILayout.Width(92)))
                //    {
                //        quarkAssetConfigData.IncludeDirectories.Clear();
                //    }
                //});
                //if (quarkAssetConfigData.IncludeDirectories.Count > 0)
                //{
                //    dirScrollPos = EditorGUILayout.BeginScrollView(dirScrollPos);
                //    var dirs = quarkAssetConfigData.IncludeDirectories;
                //    for (int i = 0; i < dirs.Count; i++)
                //    {
                //        dirs[i] = EditorGUILayout.TextField(dirs[i]);
                //    }
                //    EditorGUILayout.EndScrollView();
                //}
            });
            CosmosEditorUtility.DrawHorizontalContext(() =>
            {
                if (GUILayout.Button("Build", GUILayout.Height(32)))
                {
                    this.StartCoroutine(EnumBuildQuarkAssetDataset());
                }
                if (GUILayout.Button("Clear", GUILayout.Height(32)))
                {
                    quarkAssetConfigData.IncludeDirectories?.Clear();
                    QuarkAssetEditorUtility.Dataset.QuarkAssetDatasetInstance.Dispose();
                    CosmosEditorUtility.ClearData(QuarkAssetConfigDataFileName);
                    CosmosEditorUtility.LogInfo("Quark asset clear done ");
                }
            });
            GUILayout.EndVertical();
        }
        IEnumerator EnumBuildQuarkAssetDataset()
        {
            EditorUtility.ClearProgressBar();
            var count = Utility.IO.FolderFileCount(Application.dataPath);
            int currentIndex = 0;
            List<QuarkAssetObject> quarkAssetList = new List<QuarkAssetObject>();
            quarkAssetList?.Clear();
            string path = Application.dataPath;
            Utility.IO.TraverseFolderFile(path, (file) =>
            {
                currentIndex++;
                if (currentIndex < count)
                {
                    EditorUtility.DisplayCancelableProgressBar("QuarkAssetDataset", "Building", (float)currentIndex / (float)count);
                }
                else
                {
                    EditorUtility.ClearProgressBar();
                }
                var length = projectFiles.Length;
                for (int i = 0; i < length; i++)
                {
                    if (projectFiles[i].Equals(file.Extension))
                    {
                        var assetPath = file.FullName.Remove(0, filterLength);
                        var assetName = file.Name.Replace(file.Extension, string.Empty);
                        var type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                        var assetObj = new QuarkAssetObject()
                        {
                            AssetExtension = file.Extension,
                            AssetName = assetName,
                            AssetPath = assetPath,
                            AssetType = type.ToString(),
                            AssetGuid = AssetDatabase.AssetPathToGUID(assetPath)
                        };
                        quarkAssetList.Add(assetObj);
                    }
                }
            });
            QuarkAssetDataset.QuarkAssetObjectList = quarkAssetList;
            QuarkAssetDataset.QuarkAssetCount = quarkAssetList.Count;
            if (quarkAssetConfigData.GenerateAssetPathCode)
                CreatePathScript();
            EditorUtility.SetDirty(QuarkAssetDataset);
            CosmosEditorUtility.SaveData(QuarkAssetConfigDataFileName, quarkAssetConfigData);
            yield return null;
            CosmosEditorUtility.LogInfo("Quark asset  build done ");
        }
        void CreatePathScript()
        {
            var str = "public static class QuarkAssetDefine\n{\n";
            var con = "    public static string ";
            for (int i = 0; i < QuarkAssetDataset.QuarkAssetCount; i++)
            {
                var srcName = QuarkAssetDataset.QuarkAssetObjectList[i].AssetName;
                var fnlName = srcName.Contains(".") == true ? srcName.Replace(".", "_") : srcName;
                str = Utility.Text.Append(str, con, fnlName, "= \"", srcName, "\"", " ;\n");
            }
            str += "\n}";
            Utility.IO.OverwriteTextFile(Application.dataPath, "QuarkAssetDefine.cs", str);
            CosmosEditorUtility.RefreshEditor();
        }
        void DrawPlatformBuild()
        {
        }
    }
}