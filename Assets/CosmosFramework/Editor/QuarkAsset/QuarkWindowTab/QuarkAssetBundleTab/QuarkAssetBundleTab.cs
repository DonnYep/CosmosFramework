using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using Quark.Asset;
using Cosmos.Editor;

namespace Quark.Editor
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
        QuarkAssetDatabaseTab assetDatabaseTab;
        List<string> abPaths = new List<string>();
        public void SetAssetDatabaseTab(QuarkAssetDatabaseTab assetDatabaseTab)
        {
            this.assetDatabaseTab = assetDatabaseTab;
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
        public void OnGUI()
        {
            assetBundleTabData.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("BuildTarget", assetBundleTabData.BuildTarget);
            assetBundleTabData.OutputPath = EditorGUILayout.TextField("OutputPath", assetBundleTabData.OutputPath.Trim());
            assetBundleTabData.AssetBundleBuildPath = Path.Combine(assetBundleTabData.OutputPath, assetBundleTabData.BuildTarget.ToString()).Replace("\\", "/");
            EditorGUILayout.LabelField("AssetBundleBuildPath", assetBundleTabData.AssetBundleBuildPath);

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
                var path = assetBundleTabData.AssetBundleBuildPath;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                EditorUtility.RevealInFinder(path);
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

            DrawAESEncryptionForBuildInfoLable();
            DrawOffsetEncryptionForAssetBundleLable();

            GUILayout.Space(16);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Build"))
            {
                if (quarkAssetDataset != null)
                {
                    if (assetBundleTabData.UseAesEncryptionForBuildInfo)
                    {
                        var aesKeyStr = assetBundleTabData.AesEncryptionKeyForBuildInfo;
                        var aesKeyLength = System.Text.Encoding.UTF8.GetBytes(aesKeyStr).Length;
                        if (aesKeyLength != 16 && aesKeyLength != 24 && aesKeyLength != 32)
                        {
                            QuarkUtility.LogError("QuarkAsset build aes key is invalid , key should be 16,24 or 32 bytes long !");
                        }
                        else
                        {
                            EditorUtil.Coroutine.StartCoroutine(EnumBuildAssetBundle());
                        }
                    }
                    else
                    {
                        EditorUtil.Coroutine.StartCoroutine(EnumBuildAssetBundle());
                    }
                }
                else
                    QuarkUtility.LogError("QuarkAssetDataset is invalid !");
            }
            if (GUILayout.Button("Reset"))
            {
                assetBundleTabData = new QuarkAssetBundleTabData();
                EditorUtil.SaveData(AssetBundleTabDataFileName, assetBundleTabData);
            }
            GUILayout.EndHorizontal();
        }
        void DrawAESEncryptionForBuildInfoLable()
        {
            assetBundleTabData.UseAesEncryptionForBuildInfo = EditorGUILayout.ToggleLeft("Aes encryption for buildInfo and manifest", assetBundleTabData.UseAesEncryptionForBuildInfo);
            if (assetBundleTabData.UseAesEncryptionForBuildInfo)
            {
                EditorGUILayout.LabelField("BuildInfo AES encryption key, key should be 16,24 or 32 bytes long");
                assetBundleTabData.AesEncryptionKeyForBuildInfo = EditorGUILayout.TextField("AESKey", assetBundleTabData.AesEncryptionKeyForBuildInfo);

                var aesKeyStr = assetBundleTabData.AesEncryptionKeyForBuildInfo;
                var aesKeyLength = System.Text.Encoding.UTF8.GetBytes(aesKeyStr).Length;
                EditorGUILayout.LabelField($"Current key length is:{aesKeyLength}");
                if (aesKeyLength != 16 && aesKeyLength != 24 && aesKeyLength != 32 && aesKeyLength != 0)
                {
                    EditorGUILayout.HelpBox("Key should be 16,24 or 32 bytes long", MessageType.Error);
                }
                GUILayout.Space(16);
            }
        }
        void DrawOffsetEncryptionForAssetBundleLable()
        {
            assetBundleTabData.UseOffsetEncryptionForAssetBundle = EditorGUILayout.ToggleLeft("Offset encryption for asserBundle", assetBundleTabData.UseOffsetEncryptionForAssetBundle);
            if (assetBundleTabData.UseOffsetEncryptionForAssetBundle)
            {
                EditorGUILayout.LabelField("AssetBundle encryption offset");
                assetBundleTabData.EncryptionOffsetForAssetBundle = EditorGUILayout.IntField("Encryption offset", assetBundleTabData.EncryptionOffsetForAssetBundle);
                if (assetBundleTabData.EncryptionOffsetForAssetBundle < 0)
                    assetBundleTabData.EncryptionOffsetForAssetBundle = 0;
            }
        }
        void BrowseFolder()
        {
            assetBundleTabData.UseDefaultPath = false;
            var newPath = EditorUtility.OpenFolderPanel("Bundle Folder", assetBundleTabData.OutputPath, string.Empty);
            if (!string.IsNullOrEmpty(newPath))
            {
                assetBundleTabData.OutputPath = newPath.Replace("\\", "/");
            }
        }
        void ResetPath()
        {
            assetBundleTabData.UseDefaultPath = true;
            assetBundleTabData.OutputPath = Path.Combine(Path.GetFullPath("."), "AssetBundles").Replace("\\", "/");
        }
        string GetBuildPath()
        {
            var path = assetBundleTabData.AssetBundleBuildPath;
            return path;
        }
        IEnumerator EnumBuildAssetBundle()
        {
            yield return assetDatabaseTab.EnumUpdateADBMode();
            yield return SetBuildInfo();
            var path = assetBundleTabData.AssetBundleBuildPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            BuildPipeline.BuildAssetBundles(assetBundleTabData.AssetBundleBuildPath, assetBundleTabData.BuildAssetBundleOptions, assetBundleTabData.BuildTarget);
            yield return OperateManifest();
        }
        IEnumerator SetBuildInfo()
        {
            QuarkUtility.LogInfo("Start build asset bundle");
            if (assetBundleTabData.ClearOutputFolders)
            {
                var path = assetBundleTabData.AssetBundleBuildPath;
                if (Directory.Exists(path))
                {
                    QuarkUtility.DeleteFolder(path);
                }
            }
            var dirHashPairs = quarkAssetDataset.DirHashPairs;
            var dirs = dirHashPairs.Select(d => d.Dir).ToArray();
            yield return EditorUtil.Coroutine.StartCoroutine(TraverseTargetDirectories(dirs));
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
                buildInfoCache[map.Value.ABName] = map.Key;
            }
            yield return null;
        }
        void ResetBuildInfo()
        {
            QuarkUtility.LogInfo("Asset bundle build done");
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
                var buildPath = assetBundleTabData.AssetBundleBuildPath;
                if (Directory.Exists(buildPath))
                {
                    var streamingAssetPath = QuarkUtility.WebPathCombine(Application.streamingAssetsPath, assetBundleTabData.StreamingRelativePath);
                    QuarkUtility.Copy(buildPath, streamingAssetPath);
                }
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.RemoveUnusedAssetBundleNames();
            System.GC.Collect();
        }
        IEnumerator OperateManifest()
        {
            var buildPath = GetBuildPath();
            var paths = buildPath.Split('/');
            var pathLength = paths.Length;
            if (pathLength == 0)
            {
                QuarkUtility.LogError("Build path is invalid !");
                yield break;
            }
            var targetFolder = paths[pathLength - 1];
            QuarkUtility.LogInfo(targetFolder);
            var url = Path.Combine(buildPath, assetBundleTabData.BuildTarget.ToString());
            AssetBundleManifest manifest = null;
            AssetBundle mainBundle = null;
            yield return EditorUtil.IO.DownloadAssetBundleAsync(url, (percent) =>
            {
                var per = percent * 100;
                EditorUtility.DisplayProgressBar("LoadManifest", $"current progress : {per} %", percent);
            }, (mainAB) =>
            {
                EditorUtility.ClearProgressBar();
                manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                mainBundle = mainAB;
            });
            var abNames = manifest.GetAllAssetBundles();
            var length = abNames.Length;
            for (int i = 0; i < length; i++)
            {
                var ab = abNames[i];
                var hash = manifest.GetAssetBundleHash(ab);
                foreach (var adm in abBuildInfo.AssetDataMaps)
                {
                    if (adm.Value.ABName == ab)
                    {
                        adm.Value.ABHash = hash.ToString();
                    }
                }
            }
            yield return SetManifestInfo(abNames);
            abPaths.Clear();
            var m_buildPath = GetBuildPath();
            QuarkUtility.TraverseFolderFilePath(m_buildPath, (path) =>
            {
                var fileName = Path.GetFileName(path);
                var fileDir = Path.GetDirectoryName(path);
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
                                    QuarkUtility.RenameFile(path, newFileName);
                                }
                                break;
                            case AssetBundleHashType.HashInstead:
                                {
                                    newFileName = ad.ABHash;
                                    QuarkUtility.RenameFile(path, newFileName);
                                }
                                break;
                        }
                        var editedName = Path.Combine(fileDir, fileName);
                        abPaths.Add(editedName);
                    }
                }
                else if (assetBundleTabData.WithoutManifest)
                {
                    var ext = Path.GetExtension(path);
                    if (ext == ".manifest")
                    {
                        File.Delete(path);
                    }
                }
            });
            WriteBuildInfo();
            ResetBuildInfo();
            yield return OffsetEncryptAB(abPaths.ToArray());
            CopyToStreamingAssets();
            mainBundle.Unload(true);
        }
        IEnumerator SetManifestInfo(string[] abNames)
        {
            var urls = new string[abNames.Length];
            var length = urls.Length;
            for (int i = 0; i < length; i++)
            {
                urls[i] = QuarkUtility.WebPathCombine(GetBuildPath(), abNames[i]);
            }
            //assetPath===assetName；这里统一使用unity的地址格式；
            var assetObjsDict = quarkAssetDataset.QuarkAssetObjectList.ToDictionary((obj) => { return obj.AssetPath.ToLower().Replace("\\", "/"); });
            yield return EditorUtil.IO.DownloadAssetBundlesAsync(urls, percent =>
            {
                EditorUtility.DisplayProgressBar("AssetBundle loading", $"{percent * 100} %", percent);
            }, null, bundles =>
            {
                EditorUtility.ClearProgressBar();
                quarkAssetManifest.ManifestDict.Clear();
                var bundleLength = bundles.Length;
                for (int i = 0; i < bundleLength; i++)
                {
                    var bundleName = bundles[i].name;
                    var manifest = new QuarkManifest.ManifestItem();
                    manifest.Assets = new Dictionary<string, QuarkAssetObject>();
                    var bundleAssets = bundles[i].GetAllAssetNames();
                    {
                        var bundleAssetLength = bundleAssets.Length;
                        for (int j = 0; j < bundleAssetLength; j++)
                        {
                            if (assetObjsDict.TryGetValue(bundleAssets[j], out var assetObj))
                            {
                                manifest.Assets[bundleAssets[j]] = assetObj;
                            }
                        }
                    }
                    if (buildInfoCache.TryGetValue(bundleName, out var abPath))
                    {
                        abBuildInfo.AssetDataMaps.TryGetValue(abPath, out var assetData);
                        manifest.Hash = assetData.ABHash;
                        manifest.ABName = assetData.ABName;
                        quarkAssetManifest.ManifestDict[bundleName] = manifest;
                        manifest.ABFileSize = QuarkUtility.GetFileSize(urls[i]);
                    }
                    bundles[i].Unload(true);
                }
            });
        }
        void WriteBuildInfo()
        {
            abBuildInfo.BuildTime = System.DateTime.Now.ToString();
            var abBuildInfoJson = QuarkUtility.ToJson(abBuildInfo);
            var abBuildInfoPath = QuarkUtility.PathCombine(GetBuildPath(), quarkABBuildInfo);

            var manifestJson = QuarkUtility.ToJson(quarkAssetManifest);
            var manifestPath = QuarkUtility.PathCombine(GetBuildPath(), quarkManifest);
            abBuildInfo.Dispose();
            buildInfoCache.Clear();
            if (assetBundleTabData.UseAesEncryptionForBuildInfo)
            {
                var aesKeyStr = assetBundleTabData.AesEncryptionKeyForBuildInfo;
                byte[] aesKey = QuarkUtility.GenerateBytesAESKey(aesKeyStr);
                var encryptedBuildInfoStr = QuarkUtility.AESEncryptStringToString(abBuildInfoJson, aesKey);
                var encryptedManifestStr = QuarkUtility.AESEncryptStringToString(manifestJson, aesKey);
                QuarkUtility.WriteTextFile(abBuildInfoPath, encryptedBuildInfoStr, false);
                QuarkUtility.WriteTextFile(manifestPath, encryptedManifestStr, false);
            }
            else
            {
                QuarkUtility.WriteTextFile(abBuildInfoPath, abBuildInfoJson, false);
                QuarkUtility.WriteTextFile(manifestPath, manifestJson, false);
            }
        }
        void SetAssetBundleName(string path)
        {
            var abName = QuarkUtility.FormatAssetBundleName(path);
            if (!importerCacheDict.TryGetValue(path, out var importer))
            {
                importer = AssetImporter.GetAtPath(path);
                importerCacheDict[path] = importer;
                importer.assetBundleName = abName;
                //QuarkUtility.LogInfo(importer.assetBundleName);
                //importer.assetBundleVariant = "bytes";
                if (importer == null)
                    QuarkUtility.LogError("AssetImporter is empty : " + path);
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
        IEnumerator OffsetEncryptAB(string[] paths)
        {
            if (assetBundleTabData.UseOffsetEncryptionForAssetBundle)
            {
                var offset = assetBundleTabData.EncryptionOffsetForAssetBundle;
                yield return EditorUtil.IO.DownloadAssetBundlesBytesAsync(paths, percent =>
                {
                    var per = percent * 100;
                    EditorUtility.DisplayProgressBar("AssetBundle loading", $"current progress : {per} %", percent);
                }
                     , null, byteList =>
                     {
                         EditorUtility.ClearProgressBar();
                         var length = byteList.Count;
                         for (int i = 0; i < length; i++)
                         {
                             var percent = i / (float)length;
                             var per = Mathf.RoundToInt(percent * 100);
                             EditorUtility.DisplayProgressBar("AssetBundle encrypting", $"current progress : {per} %", percent);
                             using (MemoryStream stream = new MemoryStream(byteList[i].Length + offset))
                             {
                                 var head = new byte[offset];
                                 stream.Write(head, 0, head.Length);
                                 stream.Write(byteList[i], 0, byteList[i].Length);
                                 File.WriteAllBytes(paths[i], stream.ToArray());
                                 stream.Close();
                             }
                         }
                         EditorUtility.ClearProgressBar();
                     });
            }
        }
    }
}
