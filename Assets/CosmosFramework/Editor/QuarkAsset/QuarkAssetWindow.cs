using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Cosmos.Test;
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
        string[] barArray = new string[] { "BuildAsset", "PlatfromBuild" };
        static int filterLength;
        static QuarkAssetData quarkAssetData;
        /// <summary>
        /// Editor配置文件；
        /// </summary>
        static QuarkAssetConfigData quarkAssetConfigData;
        const string QuarkAssetConfigDataFileName = "QuarkAssetConfigData.json";
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
            var jsonHelper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IJsonHelper>();
            Utility.Json.SetHelper(jsonHelper);
        }
        private void OnEnable()
        {
            try
            {
                quarkAssetData = QuarkUtility.LoadQuarkAssetData();
                quarkAssetConfigData = CosmosEditorUtility.GetData<QuarkAssetConfigData>(QuarkAssetConfigDataFileName);
            }
            catch
            {
                quarkAssetData = new QuarkAssetData();
                quarkAssetConfigData = new QuarkAssetConfigData();
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
                EditorGUILayout.LabelField("QuarkAssetLoadMode", GUILayout.Width(128));
                quarkAssetData.QuarkAssetLoadMode = (QuarkAssetLoadMode)EditorGUILayout.EnumPopup(quarkAssetData.QuarkAssetLoadMode);
            });
            CosmosEditorUtility.DrawHorizontalContext(() =>
            {
                EditorGUILayout.LabelField("GenerateAssetPathCode", GUILayout.Width(128));
                quarkAssetConfigData.GenerateAssetPathCode = EditorGUILayout.Toggle(quarkAssetConfigData.GenerateAssetPathCode);
            });

            CosmosEditorUtility.DrawHorizontalContext(() =>
            {
                if (GUILayout.Button("Build", GUILayout.Height(32)))
                {
                    List<QuarkAssetObject> quarkAssetList = new List<QuarkAssetObject>();
                    quarkAssetList?.Clear();
                    string path = Application.dataPath;
                    Utility.IO.TraverseFolderFile(path, (file) =>
                    {
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
                                    AssetType = type.ToString()
                                };

                                quarkAssetList.Add(assetObj);
                            }
                        }
                    });
                    quarkAssetData.QuarkAssetObjectList = quarkAssetList;
                    quarkAssetData.QuarkAssetCount = quarkAssetList.Count;
                    if (quarkAssetConfigData.GenerateAssetPathCode)
                        CreatePathScript();
                    QuarkUtility.SetAndSaveQuarkAsset(quarkAssetData);
                    CosmosEditorUtility.SaveData(QuarkAssetConfigDataFileName, quarkAssetConfigData);
                    CosmosEditorUtility.LogInfo("Quark asset  build done ");
                }
                if (GUILayout.Button("Clear", GUILayout.Height(32)))
                {
                    QuarkUtility.ClearQuarkAsset();
                    CosmosEditorUtility.ClearData(QuarkAssetConfigDataFileName);
                    CosmosEditorUtility.LogInfo("Quark asset clear done ");
                }
            });
            GUILayout.EndVertical();
        }
        void CreatePathScript()
        {
            var str = "public static class QuarkAssetDefine\n{\n";
            var con = "    public static string ";
            for (int i = 0; i < quarkAssetData.QuarkAssetCount; i++)
            {
                var srcName = quarkAssetData.QuarkAssetObjectList[i].AssetName;
                var fnlName = srcName.Contains(".") == true ? srcName.Replace(".", "_") : srcName;
                str = Utility.Text.Append(str, con, fnlName,"= \"", srcName, "\""," ;\n");
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