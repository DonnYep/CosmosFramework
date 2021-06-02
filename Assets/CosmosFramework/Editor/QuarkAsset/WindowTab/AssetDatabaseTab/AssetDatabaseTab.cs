using Cosmos.Quark;
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
    public class AssetDatabaseTab
    {
        IncludeDirectoriesOperation includeDirectoriesOperation = new IncludeDirectoriesOperation();
        WindowTabData WindowTabData { get { return QuarkAssetWindow.WindowTabData; } }
        QuarkAssetDataset QuarkAssetDataset { get { return QuarkAssetEditorUtility.Dataset.QuarkAssetDatasetInstance; } }
        public void Clear() { }
        public void OnDisable()
        {
        }
        public void OnEnable()
        {
            includeDirectoriesOperation.OnEnable();
            includeDirectoriesOperation.FolderPath = QuarkAssetDataset.IncludeDirectories;
        }
        public void OnGUI()
        {
            DrawFastDevelopTab();
        }
        void DrawFastDevelopTab()
        {
            EditorUtil.DrawVerticalContext(() =>
            {
                WindowTabData.UnderAssetsDirectory = EditorGUILayout.ToggleLeft("UnderAssetsDirectory", WindowTabData.UnderAssetsDirectory);
                WindowTabData.GenerateAssetPathCode = EditorGUILayout.ToggleLeft("GenerateAssetPath", WindowTabData.GenerateAssetPathCode);
            });
            GUILayout.Space(16);
            EditorUtil.DrawHorizontalContext(() =>
            {
                if (GUILayout.Button("Build"))
                {
                    EditorUtil.Coroutine.StartCoroutine(EnumBuildADBMode());
                }
                if (GUILayout.Button("Clear"))
                {
                    ADBModeClear();
                }
            });
            if (!WindowTabData.UnderAssetsDirectory)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Space(16);
                EditorUtil.DrawHorizontalContext(() =>
                {
                    if (GUILayout.Button("ClearAssets" ))
                    {
                        includeDirectoriesOperation.Clear();
                    }
                });
                includeDirectoriesOperation.OnGUI();
                GUILayout.EndVertical();
            }
        }

        #region AssetDataBaseMode
        IEnumerator EnumBuildADBMode()
        {
            if (WindowTabData.UnderAssetsDirectory)
            {
                ADBModeUnderAssetsDirectoryBuild();
            }
            else
            {
                ADBModeNotUnderAssetsDirectoryBuild();
            }
            if (WindowTabData.GenerateAssetPathCode)
                AssetDataBaseModeCreatePathScript();
            EditorUtility.SetDirty(QuarkAssetDataset);
            EditorUtil.SaveData(QuarkAssetWindow.QuarkAssetWindowTabDataFileName, WindowTabData);
            yield return null;
            EditorUtil.Debug.LogInfo("Quark asset  build done ");
        }
        void ADBModeClear()
        {
            QuarkAssetDataset.IncludeDirectories?.Clear();
            includeDirectoriesOperation.Clear();
            QuarkAssetDataset.Dispose();
            EditorUtil.ClearData(QuarkAssetWindow.QuarkAssetWindowTabDataFileName);
            EditorUtil.Debug.LogInfo("Quark asset clear done ");
        }
        void ADBModeNotUnderAssetsDirectoryBuild()
        {
            EditorUtility.ClearProgressBar();
            var count = Utility.IO.FolderFileCount(Application.dataPath);
            int currentBuildIndex = 0;
            List<QuarkAssetDatabaseObject> quarkAssetList = new List<QuarkAssetDatabaseObject>();
            quarkAssetList?.Clear();
            int currentDirIndex = 0;
            var dirs = QuarkAssetDataset.IncludeDirectories;
            Dictionary<string, FileSystemInfo> fileSysInfoDict = new Dictionary<string, FileSystemInfo>();
            if (dirs != null)
            {
                foreach (var dir in dirs)
                {
                    if (Directory.Exists(dir))
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
                    else if (File.Exists(dir))
                    {
                        var fullPath = Utility.IO.PathCombine(EditorUtil.ApplicationPath(), dir);
                        if (!fileSysInfoDict.ContainsKey(fullPath))
                        {
                            var fileInfo = new FileInfo(fullPath);
                            fileSysInfoDict.Add(fileInfo.FullName, fileInfo);
                        }
                    }
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
                            var assetObj = new QuarkAssetDatabaseObject()
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
            QuarkAssetDataset.IncludeDirectories = includeDirectoriesOperation.FolderPath;
        }
        void ADBModeUnderAssetsDirectoryBuild()
        {
            EditorUtility.ClearProgressBar();
            var count = Utility.IO.FolderFileCount(Application.dataPath);
            int currentBuildIndex = 0;
            List<QuarkAssetDatabaseObject> quarkAssetList = new List<QuarkAssetDatabaseObject>();
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
                        var assetObj = new QuarkAssetDatabaseObject()
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
            QuarkAssetDataset.IncludeDirectories = includeDirectoriesOperation.FolderPath;
        }
        void AssetDataBaseModeCreatePathScript()
        {
            var str = "public static class QuarkAssetDefine\n{\n";
            var con = "    public static string ";
            for (int i = 0; i < QuarkAssetDataset.QuarkAssetCount; i++)
            {
                var srcName = QuarkAssetDataset.QuarkAssetObjectList[i].AssetName;
                srcName = srcName.Trim();
                var fnlName = srcName.Contains(".") == true ? srcName.Replace(".", "_") : srcName;
                fnlName = srcName.Contains(" ") == true ? srcName.Replace(" ", "_") : srcName;
                str = Utility.Text.Append(str, con, fnlName, "= \"", srcName, "\"", " ;\n");
            }
            str += "\n}";
            Utility.IO.OverwriteTextFile(Application.dataPath, "QuarkAssetDefine.cs", str);
            EditorUtil.RefreshEditor();
        }
        #endregion
    }
}
