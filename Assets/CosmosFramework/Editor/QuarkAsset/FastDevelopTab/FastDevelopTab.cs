using Cosmos.QuarkAsset;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Cosmos.CosmosEditor
{
    public class FastDevelopTab
    {
        QuarkAssetDragDropOperation quarkAssetDragDropTab = new QuarkAssetDragDropOperation();
        QuarkAssetDataset QuarkAssetDataset { get { return QuarkAssetEditorUtility.Dataset.QuarkAssetDatasetInstance; } }
        QuarkAssetConfigData QuarkAssetConfigData { get { return QuarkAssetWindow.QuarkAssetConfigData; } }

        public void Clear(){}
        public void OnEnable()
        {
            try
            {
                if (QuarkAssetConfigData.IncludeDirectories == null)
                    QuarkAssetConfigData.IncludeDirectories = new List<string>();
                else
                {
                    quarkAssetDragDropTab.OnEnable();
                    quarkAssetDragDropTab.FolderPath = QuarkAssetConfigData.IncludeDirectories;
                }
            }
            catch
            {
                quarkAssetDragDropTab.OnEnable();
                quarkAssetDragDropTab.FolderPath = QuarkAssetConfigData.IncludeDirectories;
            }
        }
        public void OnGUI()
        {
            DrawFastDevelopTab();
        }
        void DrawFastDevelopTab()
        {
            CosmosEditorUtility.DrawHorizontalContext(() =>
            {
                EditorGUILayout.LabelField("LoadMode", GUILayout.Width(128));
                QuarkAssetDataset.QuarkAssetLoadMode = (QuarkAssetLoadMode)EditorGUILayout.EnumPopup(QuarkAssetDataset.QuarkAssetLoadMode);
            });
            CosmosEditorUtility.DrawHorizontalContext(() =>
            {
                EditorGUILayout.LabelField("UnderAssetsDirectory", GUILayout.Width(128));
                QuarkAssetConfigData.UnderAssetsDirectory = EditorGUILayout.Toggle(QuarkAssetConfigData.UnderAssetsDirectory);
            });
            CosmosEditorUtility.DrawHorizontalContext(() =>
            {
                EditorGUILayout.LabelField("GenerateAssetPath", GUILayout.Width(128));
                QuarkAssetConfigData.GenerateAssetPathCode = EditorGUILayout.Toggle(QuarkAssetConfigData.GenerateAssetPathCode);
            });
            if (!QuarkAssetConfigData.UnderAssetsDirectory)
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
                     CosmosEditorUtility.StartCoroutine(EnumBuildQuarkAssetDataset());
                }
                if (GUILayout.Button("Clear", GUILayout.Height(32)))
                {
                    QuarkAssetConfigData.IncludeDirectories?.Clear();
                    quarkAssetDragDropTab.Clear();
                    QuarkAssetEditorUtility.Dataset.QuarkAssetDatasetInstance.Dispose();
                    CosmosEditorUtility.ClearData(QuarkAssetWindow.QuarkAssetConfigDataFileName);
                    CosmosEditorUtility.LogInfo("Quark asset clear done ");
                }
            });
        }
        IEnumerator EnumBuildQuarkAssetDataset()
        {
            if (!QuarkAssetConfigData.UnderAssetsDirectory)
            {
                UnderAssetsDirectoryBuild();
            }
            else
            {
                NotUnderAssetsDirectoryBuild();
            }
            if (QuarkAssetConfigData.GenerateAssetPathCode)
                CreatePathScript();
            EditorUtility.SetDirty(QuarkAssetDataset);
            CosmosEditorUtility.SaveData(QuarkAssetWindow.QuarkAssetConfigDataFileName, QuarkAssetConfigData);
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
            var dirs = QuarkAssetConfigData.IncludeDirectories;
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
                    var projLength = QuarkAssetConst.Extensions.Length;
                    for (int i = 0; i < projLength; i++)
                    {
                        if (QuarkAssetConst.Extensions[i].Equals(file.Extension))
                        {
                            var assetPath = file.FullName.Remove(0, QuarkAssetWindow.FilterLength);
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
            QuarkAssetConfigData.IncludeDirectories = quarkAssetDragDropTab.FolderPath;
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
                var length = QuarkAssetConst.Extensions.Length;
                for (int i = 0; i < length; i++)
                {
                    if (QuarkAssetConst.Extensions[i].Equals(file.Extension))
                    {
                        var assetPath = file.FullName.Remove(0, QuarkAssetWindow.FilterLength);
                        assetPath = assetPath.Replace("\\", "/");
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
            QuarkAssetConfigData.IncludeDirectories = quarkAssetDragDropTab.FolderPath;
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
    }
}
