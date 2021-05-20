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
        FastDevelopAssetPathOperation fastDevelopAssetPathOperation = new FastDevelopAssetPathOperation();
        FastDevelopTabData fastDevelopTabData;
        QuarkAssetDataset QuarkAssetDataset { get { return QuarkAssetEditorUtility.Dataset.QuarkAssetDatasetInstance; } }
        const string QuarkAssetConfigDataFileName = "QuarkAssetConfigData.json";
        public void Clear() { }
        public void OnDisable()
        {
            CosmosEditorUtility.SaveData(QuarkAssetConfigDataFileName, fastDevelopTabData);
        }
        public void OnEnable()
        {
            try
            {
                fastDevelopTabData = CosmosEditorUtility.GetData<FastDevelopTabData>(QuarkAssetConfigDataFileName);
            }
            catch
            {
                fastDevelopTabData = new FastDevelopTabData();
            }
            if (fastDevelopTabData.IncludeDirectories == null)
                fastDevelopTabData.IncludeDirectories = new List<string>();
            fastDevelopAssetPathOperation.OnEnable();
            fastDevelopAssetPathOperation.FolderPath = fastDevelopTabData.IncludeDirectories;
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
            GUILayout.Space(16);
            CosmosEditorUtility.DrawVerticalContext(() =>
            {
                fastDevelopTabData.UnderAssetsDirectory = EditorGUILayout.ToggleLeft("UnderAssetsDirectory", fastDevelopTabData.UnderAssetsDirectory);
                fastDevelopTabData.GenerateAssetPathCode = EditorGUILayout.ToggleLeft("GenerateAssetPath", fastDevelopTabData.GenerateAssetPathCode);
            });
            GUILayout.Space(16);

            CosmosEditorUtility.DrawHorizontalContext(() =>
            {
                if (GUILayout.Button("Build"))
                {
                    switch (QuarkAssetDataset.QuarkAssetLoadMode)
                    {
                        case QuarkAssetLoadMode.AssetDataBase:
                            CosmosEditorUtility.StartCoroutine(EnumBuildADBMode());
                            break;
                        case QuarkAssetLoadMode.BuiltAssetBundle:
                            GetBABData();
                            break;
                    }
                }
                if (GUILayout.Button("Clear"))
                {
                    switch (QuarkAssetDataset.QuarkAssetLoadMode)
                    {
                        case QuarkAssetLoadMode.AssetDataBase:
                            ADBModeClear();
                            break;
                        case QuarkAssetLoadMode.BuiltAssetBundle:
                            break;
                    }
                }
            });
            fastDevelopAssetPathOperation.OnGUI();
        }
        #region AssetDataBaseMode
        IEnumerator EnumBuildADBMode()
        {
            if (fastDevelopTabData.UnderAssetsDirectory)
            {
                ADBModeUnderAssetsDirectoryBuild();
            }
            else
            {
                ADBModeNotUnderAssetsDirectoryBuild();
            }
            if (fastDevelopTabData.GenerateAssetPathCode)
                AssetDataBaseModeCreatePathScript();
            EditorUtility.SetDirty(QuarkAssetDataset);
            CosmosEditorUtility.SaveData(QuarkAssetConfigDataFileName, fastDevelopTabData);
            yield return null;
            CosmosEditorUtility.LogInfo("Quark asset  build done ");
        }
        void ADBModeClear()
        {
            fastDevelopTabData.IncludeDirectories?.Clear();
            fastDevelopAssetPathOperation.Clear();
            QuarkAssetEditorUtility.Dataset.QuarkAssetDatasetInstance.Dispose();
            CosmosEditorUtility.ClearData(QuarkAssetConfigDataFileName);
            CosmosEditorUtility.LogInfo("Quark asset clear done ");
        }
        void ADBModeNotUnderAssetsDirectoryBuild()
        {
            EditorUtility.ClearProgressBar();
            var count = Utility.IO.FolderFileCount(Application.dataPath);
            int currentBuildIndex = 0;
            List<QuarkAssetObject> quarkAssetList = new List<QuarkAssetObject>();
            quarkAssetList?.Clear();
            int currentDirIndex = 0;
            var dirs = fastDevelopTabData.IncludeDirectories;
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
                                CosmosEditorUtility.LogInfo(file.FullName);
                                fileSysInfoDict.Add(file.FullName, file);
                            }
                        });
                    }
                    else if (File.Exists(dir))
                    {
                        var fullPath = Utility.IO.Combine(CosmosEditorUtility.ApplicationPath(), dir);
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
            fastDevelopTabData.IncludeDirectories = fastDevelopAssetPathOperation.FolderPath;
        }
        void ADBModeUnderAssetsDirectoryBuild()
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
            fastDevelopTabData.IncludeDirectories = fastDevelopAssetPathOperation.FolderPath;
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
            CosmosEditorUtility.RefreshEditor();
        }
        #endregion

        #region BuiltAssetBundle
        IEnumerator EnumBABMode()
        {
            return null;
        }
        void GetBABData()
        {
            var length = fastDevelopTabData.IncludeDirectories.Count;
            var dirs = fastDevelopTabData.IncludeDirectories;
            for (int i = 0; i < length; i++)
            {
                if (AssetDatabase.IsValidFolder(dirs[i]))
                {
                    CosmosEditorUtility.TraverseFolderFile(dirs[i], (obj) =>
                     {
                         var path = AssetDatabase.GetAssetPath(obj);
                         AssetImporter importer = AssetImporter.GetAtPath(path);
                         CosmosEditorUtility.LogInfo(path);
                         //var dependents = QuarkAssetEditorUtility.GetDependencises(path);
                         //Utility.Assert.Traverse(dependents, (str) => CosmosEditorUtility.LogInfo(str));
                     });
                }
                else
                {

                    var assets = AssetDatabase.LoadAllAssetsAtPath(dirs[i]);
                    Utility.Assert.Traverse(assets, (str) => CosmosEditorUtility.LogInfo(str.name));
                    //var path = AssetDatabase.LoadAssetAtPath(dirs[i]);

                    //AssetImporter importer = AssetImporter.GetAtPath(dirs[i]);


                    //var dependents = QuarkAssetEditorUtility.GetDependencises(dirs[i]);
                    //Utility.Assert.Traverse(dependents, (str) => CosmosEditorUtility.LogInfo(str));
                }

            }
        }
        #endregion
    }
}
