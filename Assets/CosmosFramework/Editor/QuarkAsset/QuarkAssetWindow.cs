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
        public static string[] Extensions { get { return extensions; } }
        static string[] extensions = new string[]
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
        public static QuarkAssetConfigData QuarkAssetConfigData { get { return quarkAssetConfigData; } }
        QuarkAssetDragDropTab quarkAssetDragDropTab = new QuarkAssetDragDropTab();
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
                else
                {
                    quarkAssetDragDropTab.OnEnable();
                    quarkAssetDragDropTab.FolderPath = quarkAssetConfigData.IncludeDirectories;
                }
            }
            catch
            {
                quarkAssetConfigData = new QuarkAssetConfigData();
                quarkAssetConfigData.IncludeDirectories = new List<string>();
                quarkAssetDragDropTab.OnEnable();
                quarkAssetDragDropTab.FolderPath = quarkAssetConfigData.IncludeDirectories;
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
            CosmosEditorUtility.DrawHorizontalContext(() =>
            {
                EditorGUILayout.LabelField("LoadMode", GUILayout.Width(128));
                QuarkAssetDataset.QuarkAssetLoadMode = (QuarkAssetLoadMode)EditorGUILayout.EnumPopup(QuarkAssetDataset.QuarkAssetLoadMode);
            });
            CosmosEditorUtility.DrawHorizontalContext(() =>
            {
                EditorGUILayout.LabelField("UnderAssetsDirectory", GUILayout.Width(128));
                quarkAssetConfigData.UnderAssetsDirectory = EditorGUILayout.Toggle(quarkAssetConfigData.UnderAssetsDirectory);
            });
            CosmosEditorUtility.DrawHorizontalContext(() =>
            {
                EditorGUILayout.LabelField("GenerateAssetPath", GUILayout.Width(128));
                quarkAssetConfigData.GenerateAssetPathCode = EditorGUILayout.Toggle(quarkAssetConfigData.GenerateAssetPathCode);
            });
            if (!quarkAssetConfigData.UnderAssetsDirectory)
            {
                CosmosEditorUtility.DrawVerticalContext(() =>
                {
                    quarkAssetDragDropTab.OnGUI();
                });
            }
            CosmosEditorUtility.DrawHorizontalContext(() =>
            {
                if (GUILayout.Button("Build", GUILayout.Height(32)))
                {
                    this.StartCoroutine(EnumBuildQuarkAssetDataset());
                }
                if (GUILayout.Button("Clear", GUILayout.Height(32)))
                {
                    quarkAssetConfigData.IncludeDirectories?.Clear();
                    quarkAssetDragDropTab.Clear();
                    QuarkAssetEditorUtility.Dataset.QuarkAssetDatasetInstance.Dispose();
                    CosmosEditorUtility.ClearData(QuarkAssetConfigDataFileName);
                    CosmosEditorUtility.LogInfo("Quark asset clear done ");
                }
            });
        }
        IEnumerator EnumBuildQuarkAssetDataset()
        {
            if (!quarkAssetConfigData.UnderAssetsDirectory)
            {
                UnderAssetsDirectoryBuild();
            }
            else
            {
                NotUnderAssetsDirectoryBuild();
            }
            if (quarkAssetConfigData.GenerateAssetPathCode)
                CreatePathScript();
            EditorUtility.SetDirty(QuarkAssetDataset);
            CosmosEditorUtility.SaveData(QuarkAssetConfigDataFileName, quarkAssetConfigData);
            yield return null;
            CosmosEditorUtility.LogInfo("Quark asset  build done ");
        }
        void UnderAssetsDirectoryBuild()
        {
            EditorUtility.ClearProgressBar();
            var count = Utility.IO.FolderFileCount(Application.dataPath);
            int currentBuildIndex = 0;
            List<QuarkAssetObject> quarkAssetList = new List<QuarkAssetObject>();
            quarkAssetList?.Clear();
            int currentDirIndex = 0;
            var dirs = quarkAssetConfigData.IncludeDirectories;
            Dictionary<string, FileSystemInfo> fileSysInfoDict = new Dictionary<string, FileSystemInfo>();
            if (dirs != null)
            {
                foreach (var dir in dirs)
                {
                    Utility.IO.TraverseFolderFile(dir, (file) =>
                    {
                        currentDirIndex++;
                        if (currentDirIndex < dirs.Count)
                        {
                            EditorUtility.DisplayCancelableProgressBar("TraverseFolderFile", "Building", (float)currentDirIndex / (float)dirs.Count);
                        }
                        else
                        {
                            EditorUtility.ClearProgressBar();
                        }
                        if (!fileSysInfoDict.ContainsKey(file.FullName))
                        {
                            fileSysInfoDict.Add(file.FullName, file);
                        }
                    });
                }
                var fileCount = fileSysInfoDict.Count;
                foreach (var file in fileSysInfoDict.Values)
                {
                    currentBuildIndex++;
                    if (currentBuildIndex < fileCount)
                    {
                        EditorUtility.DisplayCancelableProgressBar("QuarkAssetDataset", "Building", (float)currentBuildIndex / (float)fileCount);
                    }
                    else
                    {
                        EditorUtility.ClearProgressBar();
                    }
                    var projLength = extensions.Length;
                    for (int i = 0; i < projLength; i++)
                    {
                        if (extensions[i].Equals(file.Extension))
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
                }
            }
            QuarkAssetDataset.QuarkAssetObjectList = quarkAssetList;
            QuarkAssetDataset.QuarkAssetCount = quarkAssetList.Count;
            quarkAssetConfigData.IncludeDirectories = quarkAssetDragDropTab.FolderPath;
        }
        void NotUnderAssetsDirectoryBuild()
        {
            EditorUtility.ClearProgressBar();
            var count = Utility.IO.FolderFileCount(Application.dataPath);
            int currentBuildIndex = 0;
            List<QuarkAssetObject> quarkAssetList = new List<QuarkAssetObject>();
            quarkAssetList?.Clear();
            string path = Application.dataPath;
            Utility.IO.TraverseFolderFile(path, (file) =>
            {
                currentBuildIndex++;
                if (currentBuildIndex < count)
                {
                    EditorUtility.DisplayCancelableProgressBar("QuarkAssetDataset", "Building", (float)currentBuildIndex / (float)count);
                }
                else
                {
                    EditorUtility.ClearProgressBar();
                }
                var length = extensions.Length;
                for (int i = 0; i < length; i++)
                {
                    if (extensions[i].Equals(file.Extension))
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
            quarkAssetConfigData.IncludeDirectories = quarkAssetDragDropTab.FolderPath;
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