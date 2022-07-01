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
        QuarkAssetDatabaseTabData tabData;
        internal const string QuarkAssetDatabaseTabDataFileName = "QuarkAsset_DatabaseTabData.json";

        string[] selectBarArr = new string[] { "Asset bundle lable", "Asset object lable" };

        QuarkAssetBundleSearchLable assetBundleSearchLable = new QuarkAssetBundleSearchLable();
        QuarkAssetObjectSearchLable assetObjectSearchLable = new QuarkAssetObjectSearchLable();

        QuarkAssetDataset dataset;
        EditorCoroutine editorCoroutine;
        public void OnEnable()
        {
            try
            {
                tabData = EditorUtil.GetData<QuarkAssetDatabaseTabData>(QuarkAssetDatabaseTabDataFileName);
            }
            catch
            {
                tabData = new QuarkAssetDatabaseTabData();
                EditorUtil.SaveData(QuarkAssetDatabaseTabDataFileName, tabData);
            }
            assetBundleSearchLable.OnEnable();
            assetObjectSearchLable.OnEnable();
        }
        public void OnDisable()
        {
            EditorUtil.SaveData(QuarkAssetDatabaseTabDataFileName, tabData);
        }
        public void OnDatasetAssign(QuarkAssetDataset dataset)
        {
            this.dataset = dataset;
            editorCoroutine=EditorUtil.Coroutine.StartCoroutine(EnumOnDatasetAssign(dataset));
        }
        public void OnDatasetUnassign()
        {
            if (editorCoroutine != null)
                EditorUtil.Coroutine.StopCoroutine(editorCoroutine);
            assetObjectSearchLable.TreeView.Clear();
            assetBundleSearchLable.TreeView.Clear();
        }
        public void OnGUI()
        {
            DrawFastDevelopTab();
        }
        public EditorCoroutine EnumUpdateADBMode()
        {
            return EditorUtil.Coroutine.StartCoroutine(EnumBuildADBMode());
        }
        void DrawFastDevelopTab()
        {
            GUILayout.BeginVertical();
            tabData.GenerateAssetPathCode = EditorGUILayout.ToggleLeft("GenerateAssetPath", tabData.GenerateAssetPathCode);
            tabData.SortAssetObjectList = EditorGUILayout.ToggleLeft("SortAssetObjectList", tabData.SortAssetObjectList);
            tabData.SortAssetBundle = EditorGUILayout.ToggleLeft("SortAssetBundles", tabData.SortAssetBundle);
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

            GUILayout.BeginVertical(GUILayout.MinWidth(128));
            tabData.SelectedBarIndex = GUILayout.SelectionGrid(tabData.SelectedBarIndex, selectBarArr, 1);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            switch (tabData.SelectedBarIndex)
            {
                case 0:
                    assetBundleSearchLable.OnGUI();
                    break;
                case 1:
                    assetObjectSearchLable.OnGUI();
                    break;
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        IEnumerator EnumBuildADBMode()
        {
            if (dataset == null)
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
            EditorUtility.SetDirty(dataset);
            EditorUtil.SaveData(QuarkAssetDatabaseTabDataFileName, tabData);
            if (tabData.GenerateAssetPathCode)
                AssetDataBaseModeCreatePathScript();
            yield return null;
            OnDatasetAssign(dataset);
            QuarkUtility.LogInfo("Quark asset  build done ");
        }
        void ADBModeClear()
        {
            dataset.Dispose();
            assetBundleSearchLable.TreeView.Clear();
            assetObjectSearchLable.TreeView.Clear();
            EditorUtility.SetDirty(dataset);
            EditorUtil.ClearData(QuarkAssetDatabaseTabDataFileName);
            EditorUtil.Debug.LogInfo("Quark asset clear done ");
        }
        void AssetDatabaseModeBuild()
        {
            EditorUtility.ClearProgressBar();
            var count = QuarkUtility.FolderFileCount(Application.dataPath);
            int currentBuildIndex = 0;
            List<QuarkAssetObject> quarkAssetList = new List<QuarkAssetObject>();
            List<QuarkBundleInfo> validBundleList = new List<QuarkBundleInfo>();
            quarkAssetList?.Clear();
            int currentDirIndex = 0;
            var bundles = dataset.QuarkBundleInfoList;
            QuarkBundleInfo[] bundleArray = new QuarkBundleInfo[0];
            //bundle===[fileFullName===fileInfo]
            Dictionary<QuarkBundleInfo, Dictionary<string, FileSystemInfo>> bundleFileInfoDict = new Dictionary<QuarkBundleInfo, Dictionary<string, FileSystemInfo>>();
            if (bundles != null)
            {
                foreach (var bundle in bundles)
                {
                    if (Directory.Exists(bundle.AssetBundlePath))
                    {
                        QuarkUtility.TraverseFolderFile(bundle.AssetBundlePath, (file) =>
                        {
                            currentDirIndex++;
                            if (currentDirIndex < bundles.Count)
                            {
                                EditorUtility.DisplayCancelableProgressBar("QuarkAsset", "TraversingFolder", currentDirIndex / (float)bundles.Count);
                            }
                            else
                            {
                                EditorUtility.ClearProgressBar();
                            }
                            if (!bundleFileInfoDict.TryGetValue(bundle, out var fileInfoDict))
                            {
                                fileInfoDict = new Dictionary<string, FileSystemInfo>();
                                bundleFileInfoDict.Add(bundle, fileInfoDict);
                            }
                            fileInfoDict[file.FullName] = file;
                        });
                    }
                    else if (File.Exists(bundle.AssetBundlePath))
                    {
                        var fullPath = QuarkUtility.PathCombine(EditorUtil.ApplicationPath(), bundle.AssetBundlePath);

                        if (!bundleFileInfoDict.TryGetValue(bundle, out var fileInfoDict))
                        {
                            fileInfoDict = new Dictionary<string, FileSystemInfo>();
                            bundleFileInfoDict.Add(bundle, fileInfoDict);
                        }
                        var fileInfo = new FileInfo(fullPath);
                        fileInfoDict[fileInfo.FullName] = fileInfo;
                    }
                    else
                    {
                        //若地址无效，则添加到移除队列
                        validBundleList.Add(bundle);
                    }
                }
                for (int i = 0; i < validBundleList.Count; i++)
                {
                    bundles.Remove(validBundleList[i]);
                }
                bundleArray = bundles.ToArray();
                var fileCount = bundleFileInfoDict.Count;
                foreach (var files in bundleFileInfoDict)
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
                                var assetBundleName = QuarkUtility.FormatAssetBundleName(files.Key.AssetBundleName);
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

            if (tabData.SortAssetObjectList)
            {
                int arrIdx = 0;
                var arrCount = arr.Length - 1;
                SortByAscend(arr, d => d.AssetName, () => { EditorUtility.DisplayCancelableProgressBar("QuarkAsset", "SortingAssetObjectList", arrIdx++ / (float)arrCount); });
                EditorUtility.ClearProgressBar();
            }

            dataset.QuarkAssetObjectList.Clear();
            dataset.QuarkAssetObjectList.AddRange(arr);
            dataset.QuarkAssetCount = arr.Length;
            dataset.QuarkBundleInfoList.Clear();

            if (tabData.SortAssetBundle)
            {
                int dirIdx = 0;
                var dirCount = bundleArray.Length - 1;
                SortByAscend(bundleArray, d => d.AssetBundlePath, () => { EditorUtility.DisplayCancelableProgressBar("QuarkAsset", "SortingAssetBundles", dirIdx++ / (float)dirCount); });
                EditorUtility.ClearProgressBar();
            }
            dataset.QuarkBundleInfoList.AddRange(bundleArray);
        }
        void AssetDataBaseModeCreatePathScript()
        {
            var str = "public static class QuarkAssetDefine\n{\n";
            var con = "    public static string ";
            for (int i = 0; i < dataset.QuarkAssetCount; i++)
            {
                var srcName = dataset.QuarkAssetObjectList[i].AssetName;
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

        IEnumerator EnumOnDatasetAssign(QuarkAssetDataset dataset)
        {
            var bundles = dataset.QuarkBundleInfoList;
            var bundleLen = bundles.Count;
            assetBundleSearchLable.TreeView.Clear();
            for (int i = 0; i < bundleLen; i++)
            {
                var bundle = bundles[i];
                assetBundleSearchLable.TreeView.AddPath(bundle.AssetBundlePath);
            }
            assetObjectSearchLable.TreeView.Clear();
            var objects = dataset.QuarkAssetObjectList;
            for (int i = 0; i < objects.Count; i++)
            {
                assetObjectSearchLable.TreeView.AddPath(objects[i].AssetPath);
                yield return null;
            }
        }
    }

}
