using Cosmos.Resource;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    /// <summary>
    /// 资源分析窗口。
    /// <para>支持去重搜索：检测同一GUID被多个包引用的重复资产、不同路径下内容相同的重复资产；</para>
    /// <para>支持资源检索：按名称/路径/标签快速检索包裹内资源。</para>
    /// </summary>
    public class CFResourceAnalyzerWindow : EditorWindow
    {
        enum AnalyzeType
        {
            DuplicateGuid,
            DuplicateContent,
            AssetSearch,
        }

        class AnalyzeItem
        {
            public string Title;
            public string Detail;
            public string AssetPath;
        }

        ResourcePackageSO packageSO;
        AnalyzeType analyzeType = AnalyzeType.DuplicateGuid;
        string searchKeyword = string.Empty;
        Vector2 scrollPos;
        List<AnalyzeItem> items = new List<AnalyzeItem>();
        bool analyzed;
        string analyzeSummary = string.Empty;

        [MenuItem("Window/Cosmos/Module/Resource/CFResourceAnalyzer")]
        public static void OpenWindow()
        {
            var window = GetWindow<CFResourceAnalyzerWindow>("CFResourceAnalyzer", true);
            window.minSize = new Vector2(460, 420);
        }

        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            DrawPackageSelection();
            DrawAnalyzeBar();
            DrawResultList();
            EditorGUILayout.EndScrollView();
        }

        void DrawPackageSelection()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("资源分析 (Analyzer)", EditorStyles.boldLabel);
            packageSO = (ResourcePackageSO)EditorGUILayout.ObjectField("Package", packageSO, typeof(ResourcePackageSO), false);
            EditorGUILayout.EndVertical();
        }

        void DrawAnalyzeBar()
        {
            EditorGUILayout.BeginVertical("box");
            analyzeType = (AnalyzeType)EditorGUILayout.EnumPopup("Analyze Type", analyzeType);
            if (analyzeType == AnalyzeType.AssetSearch)
            {
                searchKeyword = EditorGUILayout.TextField("Keyword", searchKeyword);
            }
            if (GUILayout.Button("Analyze"))
            {
                Analyze();
            }
            if (analyzed && !string.IsNullOrEmpty(analyzeSummary))
            {
                EditorGUILayout.LabelField("Summary", analyzeSummary);
            }
            EditorGUILayout.EndVertical();
        }

        void DrawResultList()
        {
            if (!analyzed || items.Count == 0)
                return;
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Results : {items.Count}", EditorStyles.boldLabel);
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(item.Title, GUILayout.Width(160));
                GUILayout.Label(item.Detail);
                if (!string.IsNullOrEmpty(item.AssetPath) && GUILayout.Button("Ping", GUILayout.Width(48)))
                {
                    EditorUtil.PingAndActiveObject(item.AssetPath);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        void Analyze()
        {
            analyzed = false;
            items.Clear();
            if (packageSO == null)
            {
                EditorUtil.Debug.LogError("Please select a ResourcePackageSO first !");
                return;
            }
            switch (analyzeType)
            {
                case AnalyzeType.DuplicateGuid:
                    AnalyzeDuplicateGuid();
                    break;
                case AnalyzeType.DuplicateContent:
                    AnalyzeDuplicateContent();
                    break;
                case AnalyzeType.AssetSearch:
                    AnalyzeAssetSearch();
                    break;
            }
            analyzed = true;
        }
        void AnalyzeDuplicateGuid()
        {
            var guidToBundles = new Dictionary<string, List<string>>();
            var guidToPath = new Dictionary<string, string>();
            foreach (var bundleSO in packageSO.BundleSOList)
            {
                if (bundleSO == null)
                    continue;
                var bundleName = bundleSO.GetResolvedBundleName();
                foreach (var entry in bundleSO.GetAssetEntries())
                {
                    if (string.IsNullOrEmpty(entry.Guid))
                        continue;
                    if (!guidToBundles.TryGetValue(entry.Guid, out var list))
                    {
                        list = new List<string>();
                        guidToBundles.Add(entry.Guid, list);
                    }
                    if (!list.Contains(bundleName))
                        list.Add(bundleName);
                    guidToPath[entry.Guid] = AssetDatabase.GUIDToAssetPath(entry.Guid);
                }
            }
            foreach (var kv in guidToBundles)
            {
                if (kv.Value.Count > 1)
                {
                    items.Add(new AnalyzeItem()
                    {
                        Title = kv.Key,
                        Detail = $"Referenced by {kv.Value.Count} bundles : {string.Join(" , ", kv.Value.ToArray())}",
                        AssetPath = guidToPath[kv.Key],
                    });
                }
            }
            analyzeSummary = $"重复GUID数量 : {items.Count}";
        }

        void AnalyzeDuplicateContent()
        {
            var md5ToPaths = new Dictionary<string, List<string>>();
            foreach (var bundleSO in packageSO.BundleSOList)
            {
                if (bundleSO == null)
                    continue;
                foreach (var entry in bundleSO.GetAssetEntries())
                {
                    if (string.IsNullOrEmpty(entry.Guid))
                        continue;
                    var assetPath = AssetDatabase.GUIDToAssetPath(entry.Guid);
                    if (string.IsNullOrEmpty(assetPath))
                        continue;
                    var fullPath = Utility.IO.PathCombine(EditorUtil.ProjectPath, assetPath);
                    if (!File.Exists(fullPath))
                        continue;
                    var md5 = ComputeFileMD5(fullPath);
                    if (!md5ToPaths.TryGetValue(md5, out var list))
                    {
                        list = new List<string>();
                        md5ToPaths.Add(md5, list);
                    }
                    if (!list.Contains(assetPath))
                        list.Add(assetPath);
                }
            }
            foreach (var kv in md5ToPaths)
            {
                if (kv.Value.Count > 1)
                {
                    items.Add(new AnalyzeItem()
                    {
                        Title = kv.Key,
                        Detail = $"Duplicate content paths : {string.Join(" , ", kv.Value.ToArray())}",
                        AssetPath = kv.Value[0],
                    });
                }
            }
            analyzeSummary = $"内容重复的资产组数量 : {items.Count}";
        }

        void AnalyzeAssetSearch()
        {
            foreach (var bundleSO in packageSO.BundleSOList)
            {
                if (bundleSO == null)
                    continue;
                var bundleName = bundleSO.GetResolvedBundleName();
                foreach (var entry in bundleSO.GetAssetEntries())
                {
                    if (string.IsNullOrEmpty(entry.Guid))
                        continue;
                    var assetPath = AssetDatabase.GUIDToAssetPath(entry.Guid);
                    var name = string.IsNullOrEmpty(entry.Name) ? Path.GetFileNameWithoutExtension(assetPath) : entry.Name;
                    var tags = entry.AssetTags == null ? string.Empty : string.Join(",", entry.AssetTags);
                    if (string.IsNullOrEmpty(searchKeyword) || name.ToLower().Contains(searchKeyword.ToLower()) || assetPath.ToLower().Contains(searchKeyword.ToLower()) || tags.ToLower().Contains(searchKeyword.ToLower()))
                    {
                        items.Add(new AnalyzeItem()
                        {
                            Title = name,
                            Detail = $"{assetPath} | Bundle : {bundleName} | Tags : {tags}",
                            AssetPath = assetPath,
                        });
                    }
                }
            }
            analyzeSummary = $"匹配资源数量 : {items.Count}";
        }

        static string ComputeFileMD5(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = md5.ComputeHash(stream);
                    return System.BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
                }
            }
        }
    }
}
