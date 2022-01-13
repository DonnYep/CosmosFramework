using Cosmos;
using Quark.Asset;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace CosmosEditor.Quark
{
    public class QuarkDatasetTab
    {
        QuarkDirectoriesOperation quarkDirectoriesOperation = new QuarkDirectoriesOperation();
        QuarkWindowTabData WindowTabData { get { return QuarkAssetWindow.WindowTabData; } }
        QuarkAssetDataset quarkAssetDataset { get { return QuarkEditorDataProxy.QuarkAssetDataset; } }
        Rect position;
        public void OnDisable()
        {
        }
        public void OnEnable()
        {
            quarkDirectoriesOperation.OnEnable();
        }
        public void OnGUI(Rect rect)
        {
            position = rect;
            DrawFastDevelopTab(rect);
        }
        public EditorCoroutine EnumUpdateADBMode()
        {
            return EditorUtil.Coroutine.StartCoroutine(EnumBuildADBMode());
        }
        void DrawFastDevelopTab(Rect rect)
        {
            EditorUtil.DrawVerticalContext(() =>
            {
                WindowTabData.UnderAssetsDirectory = EditorGUILayout.ToggleLeft("UnderAssetsDirectory", WindowTabData.UnderAssetsDirectory);
                WindowTabData.GenerateAssetPathCode = EditorGUILayout.ToggleLeft("GenerateAssetPath", WindowTabData.GenerateAssetPathCode);
                WindowTabData.SortAssetObjectList = EditorGUILayout.ToggleLeft("SortAssetObjectList", WindowTabData.SortAssetObjectList);
                WindowTabData.SortAssetBundle = EditorGUILayout.ToggleLeft("SortAssetBundles", WindowTabData.SortAssetBundle);
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

            GUILayout.Space(16);
            GUILayout.BeginHorizontal();

            //GUILayout.BeginVertical("box",GUILayout.MaxWidth(rect.width * 0.382f));
            GUILayout.BeginVertical("box");

            if (GUILayout.Button("ClearAssetBundles"))
            {
                quarkDirectoriesOperation.Clear();
            }
            if (!WindowTabData.UnderAssetsDirectory)
            {
                quarkDirectoriesOperation.OnGUI(rect);
            }
            GUILayout.EndVertical();


            //GUILayout.BeginVertical();
            //GUILayout.Label("AssetObject List：");
            //GUILayout.EndVertical();

            GUILayout.EndHorizontal();

        }

        #region AssetDataBaseMode
        IEnumerator EnumBuildADBMode()
        {
            if (quarkAssetDataset == null)
            {
                EditorUtil.Debug.LogError("QuarkAssetDataset is invalid !");
                yield break;
            }
            if (WindowTabData.UnderAssetsDirectory)
            {
                try
                {
                    ADBModeUnderAssetsDirectoryBuild();
                }
                catch (Exception e)
                {
                    EditorUtility.ClearProgressBar();
                    EditorUtil.Debug.LogError($"Asset is invaild : {e.Message}");
                }
            }
            else
            {
                try
                {
                    ADBModeNotUnderAssetsDirectoryBuild();
                }
                catch (Exception e)
                {
                    EditorUtility.ClearProgressBar();
                    EditorUtil.Debug.LogError($"Asset is invaild : {e.Message}");
                }
            }
            EditorUtility.SetDirty(quarkAssetDataset);
            EditorUtil.SaveData(QuarkAssetWindow.QuarkAssetWindowTabDataFileName, WindowTabData);
            if (WindowTabData.GenerateAssetPathCode)
                AssetDataBaseModeCreatePathScript();
            yield return null;
            EditorUtil.Debug.LogInfo("Quark asset  build done ");
        }
        void ADBModeClear()
        {
            quarkDirectoriesOperation.Clear();
            quarkAssetDataset.Dispose();
            EditorUtility.SetDirty(quarkAssetDataset);
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
            var dirs = quarkAssetDataset.DirHashPairs;
            var dirHashPair = quarkAssetDataset.DirHashPairs.ToArray();
            Dictionary<string, FileSystemInfo> fileSysInfoDict = new Dictionary<string, FileSystemInfo>();
            if (dirs != null)
            {
                foreach (var dir in dirs)
                {
                    if (Directory.Exists(dir.Dir))
                    {
                        Utility.IO.TraverseFolderFile(dir.Dir, (file) =>
                        {
                            currentDirIndex++;
                            if (currentDirIndex < dirs.Count)
                            {
                                EditorUtility.DisplayCancelableProgressBar("QuarkAsset", "TraversingFolder", currentDirIndex / (float)dirs.Count);
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
                    else if (File.Exists(dir.Dir))
                    {
                        var fullPath = Utility.IO.PathCombine(EditorUtil.ApplicationPath(), dir.Dir);
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
                        EditorUtility.DisplayCancelableProgressBar("QuarkAsset", "Building", currentBuildIndex / (float)fileCount);
                    }
                    else
                    {
                        EditorUtility.ClearProgressBar();
                    }
                    var projLength = QuarkConstant.Extensions.Length;
                    for (int i = 0; i < projLength; i++)
                    {
                        var lowerExtension = file.Extension.ToLower();
                        var quarkLowerExt = QuarkConstant.Extensions[i].ToLower();

                        if (quarkLowerExt == lowerExtension)
                        {
                            var assetPath = file.FullName.Remove(0, QuarkAssetWindow.FilterLength);
                            var assetName = file.Name.Replace(file.Extension, string.Empty);
                            var type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                            var assetObj = new QuarkAssetDatabaseObject()
                            {
                                AssetExtension = lowerExtension,
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
            var arr = quarkAssetList.ToArray();

            if (WindowTabData.SortAssetObjectList)
            {
                int arrIdx = 0;
                var arrCount = arr.Length - 1;
                SortByAscend(arr, d => d.AssetName, () => { EditorUtility.DisplayCancelableProgressBar("QuarkAsset", "SortingAssetObjectList", arrIdx++ / (float)arrCount); });
                EditorUtility.ClearProgressBar();
            }

            quarkAssetDataset.QuarkAssetObjectList.Clear();
            quarkAssetDataset.QuarkAssetObjectList.AddRange(arr);
            quarkAssetDataset.QuarkAssetCount = arr.Length;
            quarkAssetDataset.DirHashPairs.Clear();

            if (WindowTabData.SortAssetBundle)
            {
                int dirIdx = 0;
                var dirCount = dirHashPair.Length - 1;
                SortByAscend(dirHashPair, d => d.Dir, () => { EditorUtility.DisplayCancelableProgressBar("QuarkAsset", "SortingAssetBundles", dirIdx++ / (float)dirCount); });
                EditorUtility.ClearProgressBar();
            }
            quarkAssetDataset.DirHashPairs.AddRange(dirHashPair);
        }
        void ADBModeUnderAssetsDirectoryBuild()
        {
            EditorUtility.ClearProgressBar();
            var count = Utility.IO.FolderFileCount(Application.dataPath);
            int currentBuildIndex = 0;
            List<QuarkAssetDatabaseObject> quarkAssetList = new List<QuarkAssetDatabaseObject>();
            var dirHashPair = quarkAssetDataset.DirHashPairs.ToArray();
            quarkAssetList?.Clear();
            string path = Application.dataPath;
            Utility.IO.TraverseFolderFile(path, (file) =>
            {
                currentBuildIndex++;
                if (currentBuildIndex < count)
                {
                    EditorUtility.DisplayCancelableProgressBar("QuarkAsset", "Building", currentBuildIndex / count);
                }
                else
                {
                    EditorUtility.ClearProgressBar();
                }
                var length = QuarkConstant.Extensions.Length;
                for (int i = 0; i < length; i++)
                {
                    var lowerExtension = file.Extension.ToLower();
                    var quarkLowerExt = QuarkConstant.Extensions[i].ToLower();
                    if (quarkLowerExt == lowerExtension)
                    {
                        var assetPath = file.FullName.Remove(0, QuarkAssetWindow.FilterLength);
                        assetPath = assetPath.Replace("\\", "/");
                        var assetName = file.Name.Replace(file.Extension, string.Empty);
                        var type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                        var assetObj = new QuarkAssetDatabaseObject()
                        {
                            AssetExtension = lowerExtension,
                            AssetName = assetName,
                            AssetPath = assetPath,
                            AssetType = type.ToString(),
                            AssetGuid = AssetDatabase.AssetPathToGUID(assetPath)
                        };
                        quarkAssetList.Add(assetObj);
                    }
                }
            });
            var arr = quarkAssetList.ToArray();

            if (WindowTabData.SortAssetObjectList)
            {
                int arrIdx = 0;
                var arrCount = arr.Length - 1;
                SortByAscend(arr, d => d.AssetName, () => { EditorUtility.DisplayCancelableProgressBar("QuarkAsset", "SortingAssetObjectList", arrIdx++ / (float)arrCount); });
                EditorUtility.ClearProgressBar();
            }

            quarkAssetDataset.QuarkAssetObjectList.Clear();
            quarkAssetDataset.QuarkAssetObjectList.AddRange(arr);
            quarkAssetDataset.QuarkAssetCount = arr.Length;
            quarkAssetDataset.DirHashPairs.Clear();

            if (WindowTabData.SortAssetBundle)
            {
                int dirIdx = 0;
                var dirCount = dirHashPair.Length - 1;
                SortByAscend(dirHashPair, d => d.Dir, () => { EditorUtility.DisplayCancelableProgressBar("QuarkAsset", "SortingAssetBundles", dirIdx++ / (float)dirCount); });
                EditorUtility.ClearProgressBar();
            }
            quarkAssetDataset.DirHashPairs.AddRange(dirHashPair);
        }
        void AssetDataBaseModeCreatePathScript()
        {
            var str = "public static class QuarkAssetDefine\n{\n";
            var con = "    public static string ";
            for (int i = 0; i < quarkAssetDataset.QuarkAssetCount; i++)
            {
                var srcName = quarkAssetDataset.QuarkAssetObjectList[i].AssetName;
                srcName = srcName.Trim();
                var fnlName = srcName.Contains(".") == true ? srcName.Replace(".", "_") : srcName;
                fnlName = srcName.Contains(" ") == true ? srcName.Replace(" ", "_") : srcName;
                str = Utility.Text.Append(str, con, fnlName, "= \"", srcName, "\"", " ;\n");
            }
            str += "\n}";
            Utility.IO.OverwriteTextFile(Application.dataPath, "QuarkAssetDefine.cs", str);
            AssetDatabase.Refresh();
        }
        #endregion

        void SortByAscend<T, K>(T[] array, Func<T, K> handler, Action progress)
    where K : IComparable<K>
        {
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    if (handler(array[i]).CompareTo(handler(array[j])) < 0)
                    {
                        T temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                    }
                }
                progress.Invoke();
            }
        }
    }

}
