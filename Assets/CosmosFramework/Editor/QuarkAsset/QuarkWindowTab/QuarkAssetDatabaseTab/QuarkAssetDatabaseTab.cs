using Cosmos.Editor;
using Quark.Asset;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
namespace Quark.Editor
{
    public class QuarkAssetDatabaseTab
    {
        QuarkDirectoriesOperation quarkDirectoriesOperation = new QuarkDirectoriesOperation();
        QuarkAssetDatabaseTabData assetDatabaseTabData { get { return QuarkEditorDataProxy.QuarkAssetDatabaseTabData; } }
        QuarkAssetDataset quarkAssetDataset { get { return QuarkEditorDataProxy.QuarkAssetDataset; } }
        internal const string QuarkAssetDatabaseTabDataFileName = "QuarkAssetDatabaseTabData.json";
        public void OnEnable()
        {
            try
            {
                QuarkEditorDataProxy.QuarkAssetDatabaseTabData = EditorUtil.GetData<QuarkAssetDatabaseTabData>(QuarkAssetDatabaseTabDataFileName);
            }
            catch
            {
                QuarkEditorDataProxy.QuarkAssetDatabaseTabData = new QuarkAssetDatabaseTabData();
            }
            quarkDirectoriesOperation.OnEnable();
            if (!string.IsNullOrEmpty(assetDatabaseTabData.QuarkAssetDatasetPath))
            {
                try
                {
                    QuarkEditorDataProxy.QuarkAssetDataset = AssetDatabase.LoadAssetAtPath<QuarkAssetDataset>(assetDatabaseTabData.QuarkAssetDatasetPath);
                }
                catch { }
            }
        }
        public void OnDisable()
        {
            if (quarkAssetDataset != null)
            {
                try
                {
                    var path = AssetDatabase.GetAssetPath(quarkAssetDataset);
                    assetDatabaseTabData.QuarkAssetDatasetPath = path;
                }
                catch { }
            }
            else
            {
                assetDatabaseTabData.QuarkAssetDatasetPath = string.Empty;
            }
            EditorUtil.SaveData(QuarkAssetDatabaseTabDataFileName, assetDatabaseTabData);
            if (quarkAssetDataset != null)
                EditorUtility.SetDirty(quarkAssetDataset);
        }

        public void OnGUI(Rect rect)
        {
            DrawFastDevelopTab(rect);
        }
        public EditorCoroutine EnumUpdateADBMode()
        {
            return EditorUtil.Coroutine.StartCoroutine(EnumBuildADBMode());
        }
        void DrawFastDevelopTab(Rect rect)
        {
            GUILayout.BeginVertical();
            assetDatabaseTabData.GenerateAssetPathCode = EditorGUILayout.ToggleLeft("GenerateAssetPath", assetDatabaseTabData.GenerateAssetPathCode);
            assetDatabaseTabData.SortAssetObjectList = EditorGUILayout.ToggleLeft("SortAssetObjectList", assetDatabaseTabData.SortAssetObjectList);
            assetDatabaseTabData.SortAssetBundle = EditorGUILayout.ToggleLeft("SortAssetBundles", assetDatabaseTabData.SortAssetBundle);
            GUILayout.EndVertical();

            GUILayout.Space(16);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Build"))
            {
                EditorUtil.Coroutine.StartCoroutine(EnumBuildADBMode());
            }
            if (GUILayout.Button("Clear"))
            {
                ADBModeClear();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(16);
            GUILayout.BeginHorizontal();

            //GUILayout.BeginVertical("box",GUILayout.MaxWidth(rect.width * 0.382f));
            GUILayout.BeginVertical("box");

            if (GUILayout.Button("ClearAssetBundles"))
            {
                QuarkEditorDataProxy.QuarkAssetDataset.DirHashPairs?.Clear();
                quarkDirectoriesOperation.Clear();
            }
            quarkDirectoriesOperation.OnGUI(rect);
            GUILayout.EndVertical();


            //GUILayout.BeginVertical();
            //GUILayout.Label("AssetObject List：");
            //GUILayout.EndVertical();

            GUILayout.EndHorizontal();

        }

        IEnumerator EnumBuildADBMode()
        {
            if (quarkAssetDataset == null)
            {
                EditorUtil.Debug.LogError("QuarkAssetDataset is invalid !");
                yield break;
            }
            try
            {
                AssetDatabaseModeBuild();
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                EditorUtil.Debug.LogError($"Asset is invaild : {e.Message}");
            }
            EditorUtility.SetDirty(quarkAssetDataset);
            EditorUtil.SaveData(QuarkAssetDatabaseTabDataFileName, assetDatabaseTabData);
            if (assetDatabaseTabData.GenerateAssetPathCode)
                AssetDataBaseModeCreatePathScript();
            yield return null;
            QuarkUtility.LogInfo("Quark asset  build done ");
        }
        void ADBModeClear()
        {
            quarkAssetDataset.Dispose();
            quarkDirectoriesOperation.Clear();
            EditorUtility.SetDirty(quarkAssetDataset);
            EditorUtil.ClearData(QuarkAssetDatabaseTabDataFileName);
            EditorUtil.Debug.LogInfo("Quark asset clear done ");
        }
        void AssetDatabaseModeBuild()
        {
            EditorUtility.ClearProgressBar();
            var count = QuarkUtility.FolderFileCount(Application.dataPath);
            int currentBuildIndex = 0;
            List<QuarkAssetObject> quarkAssetList = new List<QuarkAssetObject>();
            quarkAssetList?.Clear();
            int currentDirIndex = 0;
            var dirs = quarkAssetDataset.DirHashPairs;
            var dirHashPair = quarkAssetDataset.DirHashPairs.ToArray();
            //dir===[fileFullName===fileInfo]
            Dictionary<string, Dictionary<string, FileSystemInfo>> dirFileInfoDict = new Dictionary<string, Dictionary<string, FileSystemInfo>>();
            if (dirs != null)
            {
                foreach (var dir in dirs)
                {
                    if (Directory.Exists(dir.Dir))
                    {
                        QuarkUtility.TraverseFolderFile(dir.Dir, (file) =>
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
                            if (!dirFileInfoDict.TryGetValue(dir.Dir, out var fileInfoDict))
                            {
                                fileInfoDict = new Dictionary<string, FileSystemInfo>();
                                dirFileInfoDict.Add(dir.Dir, fileInfoDict);
                            }
                            fileInfoDict[file.FullName] = file;
                        });
                    }
                    else if (File.Exists(dir.Dir))
                    {
                        var fullPath = QuarkUtility.PathCombine(EditorUtil.ApplicationPath(), dir.Dir);

                        if (!dirFileInfoDict.TryGetValue(dir.Dir, out var fileInfoDict))
                        {
                            fileInfoDict = new Dictionary<string, FileSystemInfo>();
                            dirFileInfoDict.Add(dir.Dir, fileInfoDict);
                        }
                        var fileInfo = new FileInfo(fullPath);
                        fileInfoDict[fileInfo.FullName] = fileInfo;
                    }
                }
                var fileCount = dirFileInfoDict.Count;
                foreach (var files in dirFileInfoDict)
                {
                    var fileDict = files.Value;
                    foreach (var srcFile in fileDict)
                    {
                        var file = srcFile.Value;
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
                                var assetPath = file.FullName.Remove(0, QuarkAssetWindow.FilterLength).Replace("\\", "/");
                                var assetName = file.Name.Replace(file.Extension, string.Empty);
                                var type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                                var assetBundleName = QuarkUtility.FormatAssetBundleName(files.Key);
                                var assetObj = new QuarkAssetObject()
                                {
                                    AssetExtension = lowerExtension,
                                    AssetName = assetName,
                                    AssetPath = assetPath,
                                    AssetType = type.ToString(),
                                    AssetBundleName = assetBundleName
                                };
                                quarkAssetList.Add(assetObj);
                            }
                        }
                    }
                }
            }
            var arr = quarkAssetList.ToArray();

            if (assetDatabaseTabData.SortAssetObjectList)
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

            if (assetDatabaseTabData.SortAssetBundle)
            {
                int dirIdx = 0;
                var dirCount = dirHashPair.Length - 1;
                SortByAscend(dirHashPair, d => d.Dir, () => { EditorUtility.DisplayCancelableProgressBar("QuarkAsset", "SortingAssetBundles", dirIdx++ / (float)dirCount); });
                EditorUtility.ClearProgressBar();
            }
            quarkAssetDataset.DirHashPairs.AddRange(dirHashPair);
            var quarkAssetObjectList = quarkAssetDataset.QuarkAssetObjectList;
            foreach (var ao in quarkAssetObjectList)
            {
                if (!quarkAssetDataset.AssetBundleAssetObjectDict.TryGetValue(ao.AssetBundleName, out var objList))
                {
                    objList = new List<QuarkAssetObject>();
                    quarkAssetDataset.AssetBundleAssetObjectDict.Add(ao.AssetBundleName, objList);
                }
                objList.Add(ao);
            }
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
                str = QuarkUtility.Append(str, con, fnlName, "= \"", srcName, "\"", " ;\n");
            }
            str += "\n}";
            QuarkUtility.OverwriteTextFile(Application.dataPath, "QuarkAssetDefine.cs", str);
            AssetDatabase.Refresh();
        }
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
