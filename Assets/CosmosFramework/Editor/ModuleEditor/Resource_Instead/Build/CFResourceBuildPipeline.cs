using Cosmos.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    /// <summary>
    /// 新资源构建管线。
    /// <para>支持分包构建：每个ResourcePackageSO作为独立单元进行构建；</para>
    /// <para>支持资源整合去重：共享依赖自动归纳到共享依赖包；</para>
    /// <para>构建产物包含AssetBundle文件与PackageManifest清单文件。</para>
    /// </summary>
    public class CFResourceBuildPipeline
    {
        /// <summary>
        /// 完整构建一个资源包裹：收集资产、分配包名、构建AB、生成清单。
        /// </summary>
        public static void BuildPackage(ResourcePackageSO packageSO, CFResourceBuildParams buildParams)
        {
            if (packageSO == null)
                throw new ArgumentNullException("ResourcePackageSO is invalid !");
            var startTime = DateTime.Now;
            var packageName = packageSO.GetResolvedPackageName();
            EditorUtil.Debug.LogInfo($"Start build package : {packageName} , target : {buildParams.BuildTarget}");

            // 1、收集资产（去重 + 整合共享依赖）
            var collectResult = ResourceCollector.Collect(packageSO);
            if (collectResult == null)
                throw new InvalidOperationException("Collect failed !");
            for (int i = 0; i < collectResult.Warnings.Count; i++)
            {
                EditorUtil.Debug.LogWarning(collectResult.Warnings[i]);
            }
            EditorUtil.Debug.LogInfo($"Collect done , assets : {collectResult.Manifest.AssetList.Count} , bundles : {collectResult.Manifest.BundleList.Count} , duplicated removed : {collectResult.DuplicatedAssets.Count}");

            // 2、准备输出目录
            var outputPath = ResolveOutputPath(packageName, buildParams.OutputRootPath);
            if (buildParams.ClearOutputPath && Directory.Exists(outputPath))
                Utility.IO.DeleteFolder(outputPath);
            Utility.IO.CreateFolder(outputPath);

            // 3、分配AssetBundleName并构建
            AssignBundleNames(collectResult);
            try
            {
                var manifest = BuildPipeline.BuildAssetBundles(outputPath, buildParams.BuildAssetBundleOptions, buildParams.BuildTarget);
                if (manifest == null)
                    throw new InvalidOperationException("BuildAssetBundles failed !");
                // 4、回填包体信息（hash、size、依赖）并生成清单
                PostProcessBundles(collectResult, outputPath, manifest);
                WriteManifest(packageSO, collectResult, outputPath, buildParams);
                // 5、可选拷贝到StreamingAssets
                CopyToStreamingAssetsIfNeeded(packageSO, outputPath, buildParams);
            }
            finally
            {
                // 还原AB名，避免污染工程
                ClearBundleNames(collectResult);
            }
            var endTime = DateTime.Now;
            var elapsedTime = endTime - startTime;
            EditorUtil.Debug.LogInfo($"Build package : {packageName} done , elapsed {elapsedTime.Minutes}m {elapsedTime.Seconds}s {elapsedTime.Milliseconds}ms");
        }
        /// <summary>
        /// 仅导出文件清单（编辑器模拟模式使用），不构建AB包。
        /// </summary>
        public static string ExportPackageManifest(ResourcePackageSO packageSO, string outputPath)
        {
            if (packageSO == null)
                return null;
            var collectResult = ResourceCollector.Collect(packageSO);
            if (collectResult == null)
                return null;
            var manifest = collectResult.Manifest;
            var packageName = packageSO.GetResolvedPackageName();
            if (string.IsNullOrEmpty(outputPath))
                outputPath = $"{ResourceEditorConstants.DEFAULT_PACKAGE_OUTPUT_PATH}/{packageName}";
            Utility.IO.CreateFolder(outputPath);
            var filePath = Utility.IO.PathCombine(outputPath, ResourceEditorConstants.PACKAGE_MANIFEST_FILE_NAME);
            var json = ResourceUtility.SerializeManifest(manifest, null);
            Utility.IO.OverwriteTextFile(filePath, json);
            return filePath;
        }

        static string ResolveOutputPath(string packageName, string outputRoot)
        {
            var root = outputRoot;
            if (string.IsNullOrEmpty(root))
                root = ResourceEditorConstants.DEFAULT_PACKAGE_OUTPUT_PATH;
            return Utility.IO.PathCombine(EditorUtil.ProjectPath, root, packageName);
        }

        static void AssignBundleNames(ResourceCollector.CollectResult collectResult)
        {
            foreach (var kv in collectResult.AssetBundleMap)
            {
                var assetPath = kv.Key;
                var bundleName = kv.Value;
                var importer = AssetImporter.GetAtPath(assetPath);
                if (importer == null)
                    continue;
                if (importer.assetBundleName != bundleName)
                    importer.assetBundleName = bundleName;
            }
            AssetDatabase.Refresh();
        }

        static void ClearBundleNames(ResourceCollector.CollectResult collectResult)
        {
            foreach (var assetPath in collectResult.AssetBundleMap.Keys)
            {
                var importer = AssetImporter.GetAtPath(assetPath);
                if (importer == null)
                    continue;
                if (!string.IsNullOrEmpty(importer.assetBundleName))
                    importer.assetBundleName = string.Empty;
            }
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();
        }
        static void PostProcessBundles(ResourceCollector.CollectResult collectResult, string outputPath, AssetBundleManifest abManifest)
        {
            var bundleList = collectResult.Manifest.BundleList;
            var bundleCount = bundleList.Count;
            for (int i = 0; i < bundleCount; i++)
            {
                var bundle = bundleList[i];
                var fileName = $"{ResourceUtility.FilterName(bundle.BundleName)}.{ResourceEditorConstants.DEFAULT_AB_EXTENSION}";
                var filePath = Utility.IO.PathCombine(outputPath, fileName);
                if (File.Exists(filePath))
                {
                    var fileInfo = new FileInfo(filePath);
                    bundle.FileSize = fileInfo.Length;
                    bundle.FileHash = Utility.IO.GenerateFileMD5(filePath);
                }
                var hash = abManifest.GetAssetBundleHash(bundle.BundleName);
                bundle.CRC = Hash128ToCRC(hash);
                bundle.DependBundles = abManifest.GetDirectDependencies(bundle.BundleName);
            }
            // 回填资产所属bundle的hash
            var assetList = collectResult.Manifest.AssetList;
            var assetCount = assetList.Count;
            for (int i = 0; i < assetCount; i++)
            {
                var asset = assetList[i];
                for (int j = 0; j < bundleCount; j++)
                {
                    var bundle = bundleList[j];
                    if (bundle.BundleName == asset.BundleName)
                    {
                        asset.BundleHash = bundle.FileHash;
                        break;
                    }
                }
            }
        }

        static uint Hash128ToCRC(Hash128 hash)
        {
            // 将Hash128转换为可读的CRC数值
            var bytes = new byte[16];
            var hashString = hash.ToString();
            for (int i = 0; i < 8 && i < hashString.Length; i++)
            {
                bytes[i] = (byte)hashString[i];
            }
            return BitConverter.ToUInt32(bytes, 0);
        }

        static void WriteManifest(ResourcePackageSO packageSO, ResourceCollector.CollectResult collectResult, string outputPath, CFResourceBuildParams buildParams)
        {
            var manifest = collectResult.Manifest;
            var filePath = Utility.IO.PathCombine(outputPath, ResourceEditorConstants.PACKAGE_MANIFEST_FILE_NAME);
            var aesKey = buildParams.EncryptManifest ? buildParams.ManifestEncryptionKey : null;
            var json = ResourceUtility.SerializeManifest(manifest, aesKey);
            Utility.IO.OverwriteTextFile(filePath, json);
            EditorUtil.Debug.LogInfo($"PackageManifest generated : {filePath}");
        }

        static void CopyToStreamingAssetsIfNeeded(ResourcePackageSO packageSO, string outputPath, CFResourceBuildParams buildParams)
        {
            if (!buildParams.CopyToStreamingAssets)
                return;
            var relativePath = buildParams.StreamingAssetsRelativePath;
            if (string.IsNullOrEmpty(relativePath))
                relativePath = ResourceEditorConstants.DEFAULT_PACKAGE_STREAMING_ASSETS_PATH;
            var destPath = Utility.IO.PathCombine(Application.streamingAssetsPath, relativePath, packageSO.GetResolvedPackageName());
            if (Directory.Exists(destPath))
                Utility.IO.DeleteFolder(destPath);
            Utility.IO.CreateFolder(destPath);
            var files = Directory.GetFiles(outputPath, "*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; i++)
            {
                var srcFile = files[i];
                var fileName = Path.GetFileName(srcFile);
                var destFile = Utility.IO.PathCombine(destPath, fileName);
                File.Copy(srcFile, destFile, true);
            }
            AssetDatabase.Refresh();
            EditorUtil.Debug.LogInfo($"Copied build output to StreamingAssets : {destPath}");
        }
    }
}
