﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using Quark.Asset;
using Quark;
using Cosmos;
namespace CosmosEditor.Quark
{
    public class QuarkAssetBundleTab
    {
        QuarkAssetBundleTabData assetBundleTabData;
        const string AssetBundleTabDataFileName = "AssetBundleTabData.json";
        const string quarkABBuildInfo = "BuildInfo.json";
        const string quarkManifest = "Manifest.json";
        Dictionary<string, AssetImporter> importerCacheDict = new Dictionary<string, AssetImporter>();
        QuarkBuildInfo abBuildInfo = new QuarkBuildInfo();
        QuarkManifest quarkAssetManifest = new QuarkManifest();
        /// <summary>
        /// Key:ABName ; Value: ABPath
        /// </summary>
        Dictionary<string, string> buildInfoCache = new Dictionary<string, string>();
        QuarkAssetDataset quarkAssetDataset { get { return QuarkEditorDataProxy.QuarkAssetDataset; } }
        QuarkDatasetTab assetDatasetTab;
        List<string> abPaths = new List<string>();
        public void SetAssetDatasetTab(QuarkDatasetTab assetDatasetTab)
        {
            this.assetDatasetTab = assetDatasetTab;
        }
        public void OnDisable()
        {
            EditorUtil.SaveData(AssetBundleTabDataFileName, assetBundleTabData);
        }
        public void OnEnable()
        {
            try
            {
                assetBundleTabData = EditorUtil.GetData<QuarkAssetBundleTabData>(AssetBundleTabDataFileName);
            }
            catch
            {
                assetBundleTabData = new QuarkAssetBundleTabData();
                EditorUtil.SaveData(AssetBundleTabDataFileName, assetBundleTabData);
            }
        }
        public void OnGUI(Rect rect)
        {
            assetBundleTabData.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("BuildTarget", assetBundleTabData.BuildTarget);
            assetBundleTabData.OutputPath = EditorGUILayout.TextField("OutputPath", assetBundleTabData.OutputPath.Trim());

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Browse", GUILayout.MaxWidth(128f)))
            {
                BrowseFolder();
            }
            if (GUILayout.Button("Reset", GUILayout.MaxWidth(128f)))
            {
                ResetPath();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(16);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OpenOutputPath"))
            {
                EditorUtility.RevealInFinder(GetBuildFolder());
            }
            if (GUILayout.Button("OpenPersistentPath"))
            {
                EditorUtility.RevealInFinder(Application.persistentDataPath);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(16);


            GUILayout.BeginVertical();
            assetBundleTabData.WithoutManifest = EditorGUILayout.ToggleLeft("WithoutManifest", assetBundleTabData.WithoutManifest);

            assetBundleTabData.ClearOutputFolders = EditorGUILayout.ToggleLeft("ClearOutputFolders", assetBundleTabData.ClearOutputFolders);
            assetBundleTabData.CopyToStreamingAssets = EditorGUILayout.ToggleLeft("CopyToStreamingAssets", assetBundleTabData.CopyToStreamingAssets);
            if (assetBundleTabData.CopyToStreamingAssets)
            {
                GUILayout.Space(16);
                GUILayout.Label("Assets/StreamingAssets/ 下的相对路径地址，可选填 ");
                assetBundleTabData.StreamingRelativePath = EditorGUILayout.TextField("StreamingRelativePath", assetBundleTabData.StreamingRelativePath.Trim());
            }
            GUILayout.EndVertical();


            GUILayout.Space(16);
            GUILayout.Label("CompressedFormat  建议使用默认模式，并且请勿与NameHashType的其他类型混用，会导致AB包名混乱！");
            assetBundleTabData.BuildAssetBundleOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup("CompressedFormat:", assetBundleTabData.BuildAssetBundleOptions);
            assetBundleTabData.NameHashType = (AssetBundleHashType)EditorGUILayout.EnumPopup("NameHashType", assetBundleTabData.NameHashType);

            GUILayout.Space(16);

            assetBundleTabData.UseAESEncryption = EditorGUILayout.ToggleLeft("UseAESEncryption", assetBundleTabData.UseAESEncryption);
            if (assetBundleTabData.UseAESEncryption)
            {
                GUILayout.Space(16);
                var aesKeyLength = Encoding.UTF8.GetBytes(assetBundleTabData.AESEncryptionKey).Length;
                EditorGUILayout.LabelField($"密钥长度须为16，24，32，当前密钥长度：{aesKeyLength}");
                assetBundleTabData.AESEncryptionKey = EditorGUILayout.TextField("AESEncryptionKey", assetBundleTabData.AESEncryptionKey);
            }
            GUILayout.Space(16);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Build"))
            {
                if (quarkAssetDataset != null)
                    EditorUtil.Coroutine.StartCoroutine(EnumBuildAssetBundle());
                else
                    EditorUtil.Debug.LogError("QuarkAssetDataset is invalid !");
            }
            if (GUILayout.Button("Reset"))
            {
                assetBundleTabData = new QuarkAssetBundleTabData();
                EditorUtil.SaveData(AssetBundleTabDataFileName, assetBundleTabData);
            }
            GUILayout.EndHorizontal();
        }
        void BrowseFolder()
        {
            assetBundleTabData.UseDefaultPath = false;
            var newPath = EditorUtility.OpenFolderPanel("Bundle Folder", assetBundleTabData.OutputPath, string.Empty);
            if (!string.IsNullOrEmpty(newPath))
            {
                var gamePath = EditorUtil.ApplicationPath();
                gamePath = gamePath.Replace("\\", "/");
                if (newPath.StartsWith(gamePath) && newPath.Length > gamePath.Length)
                    newPath = newPath.Remove(0, gamePath.Length + 1);
                assetBundleTabData.OutputPath = newPath;
            }
        }
        void ResetPath()
        {
            assetBundleTabData.UseDefaultPath = true;
            assetBundleTabData.OutputPath = Path.Combine("AssetBundles", assetBundleTabData.BuildTarget.ToString());
        }
        string GetBuildFolder()
        {
            var path = Utility.IO.WebPathCombine(EditorUtil.ApplicationPath(), assetBundleTabData.OutputPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        string GetBuildPath()
        {
            assetBundleTabData.OutputPath = Utility.IO.WebPathCombine(assetBundleTabData.OutputPath);
            var path = Utility.IO.WebPathCombine(EditorUtil.ApplicationPath(), assetBundleTabData.OutputPath);
            return path;
        }
        IEnumerator EnumBuildAssetBundle()
        {
            yield return assetDatasetTab.EnumUpdateADBMode();
            yield return SetBuildInfo();
            BuildPipeline.BuildAssetBundles(GetBuildFolder(), assetBundleTabData.BuildAssetBundleOptions, assetBundleTabData.BuildTarget);
            OperateManifest();
        }
        IEnumerator SetBuildInfo()
        {
            EditorUtil.Debug.LogInfo("Start build asset bundle");
            if (assetBundleTabData.ClearOutputFolders)
            {
                var path = Utility.IO.PathCombine(EditorUtil.ApplicationPath(), assetBundleTabData.OutputPath);
                if (Directory.Exists(path))
                {
                    Utility.IO.DeleteFolder(path);
                }
            }
            if (QuarkAssetWindow.WindowTabData.UnderAssetsDirectory)
            {
                var dirs = EditorUtil.IO.GetAllBundleableFilePath();
                yield return EditorUtil.Coroutine.StartCoroutine(TraverseTargetDirectories(dirs));
            }
            else
            {
                var dirHashPairs = quarkAssetDataset.DirHashPairs;
                var dirs = dirHashPairs.Select(d => d.Dir).ToArray();
                yield return EditorUtil.Coroutine.StartCoroutine(TraverseTargetDirectories(dirs));
            }
        }
        IEnumerator TraverseTargetDirectories(string[] dirs)
        {
            foreach (var dir in dirs)
            {
                SetAssetBundleName(dir);
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            foreach (var map in abBuildInfo.AssetDataMaps)
            {
                map.Value.DependList = AssetDatabase.GetAssetBundleDependencies(map.Value.ABName, true).ToList();
                buildInfoCache.TryAdd(map.Value.ABName, map.Key);
            }
            yield return null;
        }
        void ResetBuildInfo()
        {
            EditorUtil.Debug.LogInfo("Asset bundle built done");
            foreach (var imp in importerCacheDict)
            {
                imp.Value.assetBundleName = null; ;
            }
            importerCacheDict.Clear();
        }
        void CopyToStreamingAssets()
        {
            if (assetBundleTabData.CopyToStreamingAssets)
            {
                var buildPath = Utility.IO.WebPathCombine(EditorUtil.ApplicationPath(), assetBundleTabData.OutputPath);
                if (Directory.Exists(buildPath))
                {
                    var streamingAssetPath = Utility.IO.WebPathCombine(Application.streamingAssetsPath, assetBundleTabData.StreamingRelativePath);
                    Utility.IO.Copy(buildPath, streamingAssetPath);
                }
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.RemoveUnusedAssetBundleNames();
            System.GC.Collect();
        }
        void OperateManifest()
        {
            var buildPath = GetBuildPath();
            var paths= buildPath.Split('/');
            var pathLength = paths.Length;
            if (pathLength==0)
            {
                EditorUtil.Debug.LogError("Build path is invalid !");
                return;
            }
            var targetFolder = paths[pathLength - 1];
            EditorUtil.Debug.LogInfo(targetFolder);
            var url = Utility.IO.WebPathCombine(buildPath, targetFolder);
            EditorUtil.IO.DownloadAssetBundleAsync(url, (percent) =>
            {
                var per = percent * 100;
                EditorUtility.DisplayProgressBar("LoadManifest", $"当前进度 ：{per} %", percent);
            }, (mainBundle) =>
            {
                EditorUtility.ClearProgressBar();
                var manifest = mainBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                var abNames = manifest.GetAllAssetBundles();
                Utility.Assert.Traverse(abNames, (ab) =>
                {
                    var hash = manifest.GetAssetBundleHash(ab);
                    foreach (var adm in abBuildInfo.AssetDataMaps)
                    {
                        if (adm.Value.ABName == ab)
                        {
                            adm.Value.ABHash = hash.ToString();
                        }
                    }
                });
                SetManifestInfo(abNames);
                abPaths.Clear();
                var m_buildPath = GetBuildPath();
                Utility.IO.TraverseFolderFilePath(m_buildPath, (path) =>
                {
                    var fileName = Path.GetFileName(path);
                    var fileDir = Path.GetDirectoryName(path);
                    //EditorUtil.Debug.LogInfo(fileDir);
                    if (!fileName.Contains(".manifest"))
                    {
                        if (buildInfoCache.TryGetValue(fileName, out var abPath))
                        {
                            abBuildInfo.AssetDataMaps.TryGetValue(abPath, out var ad);
                            string newFileName = string.Empty;
                            switch (assetBundleTabData.NameHashType)
                            {
                                case AssetBundleHashType.DefaultName:
                                    {
                                        newFileName = fileName;
                                    }
                                    break;
                                case AssetBundleHashType.AppendHash:
                                    {
                                        newFileName = fileName + "_" + ad.ABHash;
                                        Utility.IO.RenameFile(path, newFileName);
                                    }
                                    break;
                                case AssetBundleHashType.HashInstead:
                                    {
                                        newFileName = ad.ABHash;
                                        Utility.IO.RenameFile(path, newFileName);
                                    }
                                    break;
                            }
                            var editedName = Path.Combine(fileDir, fileName);
                            abPaths.Add(editedName);
                        }
                    }
                });
                if (assetBundleTabData.WithoutManifest)
                {
                    var abBuildPath = GetBuildPath();
                    Utility.IO.TraverseFolderFilePath(abBuildPath, (path) =>
                    {
                        var ext = Path.GetExtension(path);
                        if (ext == ".manifest")
                        {
                            File.Delete(path);
                        }
                    });
                }
                EncryptAB(abPaths.ToArray());
                mainBundle.Unload(true);
            });
        }
        void SetManifestInfo(string[] abNames)
        {
            var urls = new string[abNames.Length];
            var length = urls.Length;
            for (int i = 0; i < length; i++)
            {
                urls[i] = Utility.IO.WebPathCombine(GetBuildPath(), abNames[i]);
            }
            //assetPath===assetName；这里统一使用unity的地址格式；
            var assetObjsDict = quarkAssetDataset.QuarkAssetObjectList.ToDictionary((obj) => { return obj.AssetPath.ToLower().Replace("\\", "/"); });
            EditorUtil.IO.DownloadAssetBundlesAsync(urls, percent =>
            {
                EditorUtility.DisplayProgressBar("AssetBundleLoading", $"{percent * 100} %", percent);
            }, null, bundles =>
            {
                EditorUtility.ClearProgressBar();
                quarkAssetManifest.ManifestDict.Clear();
                var bundleLength = bundles.Length;
                for (int i = 0; i < bundleLength; i++)
                {
                    var bundleName = bundles[i].name;
                    var manifest = new QuarkManifest.ManifestItem();
                    manifest.Assets = new Dictionary<string, string>();
                    var bundleAssets = bundles[i].GetAllAssetNames();
                    {
                        var bundleAssetLength = bundleAssets.Length;
                        for (int j = 0; j < bundleAssetLength; j++)
                        {
                            if (assetObjsDict.TryGetValue(bundleAssets[j], out var assetObj))
                            {
                                manifest.Assets.TryAdd(bundleAssets[j], assetObj.AssetName);
                            }
                        }
                    }
                    //EditorUtil.Debug.LogInfo(bundleName, MessageColor.YELLOW);
                    if (buildInfoCache.TryGetValue(bundleName, out var abPath))
                    {
                        abBuildInfo.AssetDataMaps.TryGetValue(abPath, out var assetData);
                        manifest.Hash = assetData.ABHash;
                        manifest.ABName = assetData.ABName;
                        quarkAssetManifest.ManifestDict.TryAdd(bundleName, manifest);
                        manifest.ABFileSize = Utility.IO.GetFileSize(urls[i]);
                    }
                    bundles[i].Unload(true);
                }
                WriteBuildInfo();
                ResetBuildInfo();
                CopyToStreamingAssets();
            });

        }
        void WriteBuildInfo()
        {
            abBuildInfo.BuildTime = System.DateTime.Now.ToString();
            var json = QuarkUtility.Json.ToJson(abBuildInfo, true);
            var fullPath = Utility.IO.PathCombine(GetBuildPath(), quarkABBuildInfo);
            Utility.IO.WriteTextFile(fullPath, json, false);
            abBuildInfo.Dispose();
            buildInfoCache.Clear();
            var manifestJson = QuarkUtility.Json.ToJson(quarkAssetManifest);
            var manifestPath = Utility.IO.PathCombine(GetBuildPath(), quarkManifest);
            Utility.IO.WriteTextFile(manifestPath, manifestJson, false);
        }
        void SetAssetBundleName(string path)
        {
            string abName = string.Empty;
            abName = Utility.Text.Replace(path, new string[] { "\\", "/", ".", " " }, "_").ToLower();
            AssetImporter importer = null;
            if (!importerCacheDict.TryGetValue(path, out importer))
            {
                importer = AssetImporter.GetAtPath(path);
                importerCacheDict[path] = importer;
                importer.assetBundleName = abName;
                //EditorUtil.Debug.LogInfo(importer.assetBundleName);
                //importer.assetBundleVariant = "bytes";
                if (importer == null)
                    EditorUtil.Debug.LogError("AssetImporter is empty : " + path);
                else
                if (!abBuildInfo.AssetDataMaps.TryGetValue(importer.assetPath, out var assetData))
                {
                    assetData = new QuarkBuildInfo.AssetData()
                    {
                        DependList = AssetDatabase.GetAssetBundleDependencies(abName, true).ToList(),
                        Id = abBuildInfo.AssetDataMaps.Count,
                        ABName = abName,
                        //ABName = abName+".bytes",
                    };

                    abBuildInfo.AssetDataMaps[importer.assetPath] = assetData;
                }
            }
        }
        /// <summary>
        /// 对AB进行加密；
        /// </summary>
        /// <param name="paths">AB的地址</param>
        void EncryptAB(string[] paths)
        {
            if (assetBundleTabData.UseAESEncryption)
            {
                var aseByteKey = Encoding.UTF8.GetBytes(assetBundleTabData.AESEncryptionKey);
                EditorUtil.IO.DownloadAssetBundlesBytesAsync(paths, percent =>
                {
                    var per = percent * 100;
                    EditorUtility.DisplayProgressBar("AB加密加载", $"当前进度 ：{per} %", percent);
                }
                , null, byteList =>
                {
                    EditorUtility.ClearProgressBar();
                    var length = byteList.Count;
                    for (int i = 0; i < length; i++)
                    {
                        var encryptedBytes = Utility.Encryption.AESEncryptByteToByte(byteList[i], aseByteKey);
                        File.WriteAllBytes(paths[i], encryptedBytes);
                    }
                    //GUIUtility.ExitGUI();
                });
            }
        }
    }
}
