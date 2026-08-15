using Cosmos.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    /// <summary>
    /// 资源编辑分类窗口（Addressables Groups 风格）。
    /// <para>功能：</para>
    /// <para>1、创建/删除资源包裹（ResourcePackageSO）与资源包（PackageBundleSO）；</para>
    /// <para>2、向资源包中添加/移除资产（单个拖放或整个文件夹收集）；</para>
    /// <para>3、编辑资产的寻址名称（Address）与分类标签（Tags），完成资源分类；</para>
    /// <para>4、一键分配/清除AssetBundleName，配合构建窗口使用。</para>
    /// </summary>
    public class ResourcePackageEditorWindow : EditorWindow
    {
        ResourcePackageSO packageSO;
        PackageBundleSO selectedBundle;
        Vector2 bundleScrollPos;
        Vector2 entryScrollPos;
        string newBundleName = "NewBundle";
        string addFolderPath = "Assets";
        string statusMessage = string.Empty;
        double statusEndTime;

        [MenuItem("Window/Cosmos/Module/Resource/ResourceEditor")]
        public static void OpenWindow()
        {
            var window = GetWindow<ResourcePackageEditorWindow>("ResourceEditor", true);
            window.minSize = new Vector2(640, 480);
        }

        void OnEnable()
        {
            titleContent = new GUIContent("ResourceEditor");
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            DrawPackageBar();
            if (packageSO != null)
            {
                EditorGUILayout.BeginHorizontal();
                DrawBundleList();
                DrawEntryList();
                EditorGUILayout.EndHorizontal();
                DrawOperationBar();
            }
            DrawStatusBar();
            EditorGUILayout.EndVertical();
        }

        #region 包裹栏
        void DrawPackageBar()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("资源包裹 (Package)", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            packageSO = (ResourcePackageSO)EditorGUILayout.ObjectField("Package", packageSO, typeof(ResourcePackageSO), false);
            if (GUILayout.Button("New", GUILayout.Width(48)))
                CreateNewPackage();
            if (GUILayout.Button("Save", GUILayout.Width(48)))
                SavePackage();
            EditorGUILayout.EndHorizontal();
            if (packageSO != null)
            {
                EditorGUILayout.BeginHorizontal();
                packageSO.PackageName = EditorGUILayout.TextField("PackageName", packageSO.PackageName);
                packageSO.PackageVersion = EditorGUILayout.TextField("Version", packageSO.PackageVersion);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region 包体列表（左栏）
        void DrawBundleList()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Width(220));
            EditorGUILayout.LabelField($"资源包 (Bundles : {packageSO.BundleSOList.Count})", EditorStyles.boldLabel);
            bundleScrollPos = EditorGUILayout.BeginScrollView(bundleScrollPos);
            for (int i = 0; i < packageSO.BundleSOList.Count; i++)
            {
                var bundle = packageSO.BundleSOList[i];
                if (bundle == null)
                    continue;
                var isSelected = bundle == selectedBundle;
                var style = isSelected ? EditorStyles.boldLabel : EditorStyles.label;
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(bundle.GetResolvedBundleName(), style, GUILayout.Height(20)))
                {
                    selectedBundle = bundle;
                }
                if (GUILayout.Button("-", GUILayout.Width(24)))
                {
                    DeleteBundle(bundle);
                    i--;
                    continue;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            newBundleName = EditorGUILayout.TextField(newBundleName);
            if (GUILayout.Button("Add", GUILayout.Width(44)))
                CreateBundle(newBundleName);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region 资产条目列表（右栏）
        void DrawEntryList()
        {
            EditorGUILayout.BeginVertical("box");
            if (selectedBundle == null)
            {
                EditorGUILayout.HelpBox("请在左侧选择一个资源包 (Bundle) 进行资产编辑。", MessageType.Info);
                EditorGUILayout.EndVertical();
                return;
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"资产条目 (Bundle : {selectedBundle.GetResolvedBundleName()} , Count : {selectedBundle.AssetEntries.Count})", EditorStyles.boldLabel);
            selectedBundle.PackSeparately = EditorGUILayout.ToggleLeft("PackSeparately", selectedBundle.PackSeparately, GUILayout.Width(120));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            selectedBundle.BundleName = EditorGUILayout.TextField("BundleName", selectedBundle.BundleName);
            if (GUILayout.Button("Assign AB Names", GUILayout.Width(120)))
            {
                selectedBundle.AssignBundleNameToAssets();
                AssetDatabase.SaveAssets();
                ShowStatus("AssetBundleName assigned !");
            }
            EditorGUILayout.EndHorizontal();

            DrawAddAssetBar();

            entryScrollPos = EditorGUILayout.BeginScrollView(entryScrollPos);
            var entries = selectedBundle.AssetEntries;
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                if (entry == null)
                    continue;
                var assetPath = string.IsNullOrEmpty(entry.AssetPath) ? AssetDatabase.GUIDToAssetPath(entry.Guid) : entry.AssetPath;
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                entry.Name = EditorGUILayout.TextField("Address", entry.Name);
                if (GUILayout.Button("Ping", GUILayout.Width(44)))
                {
                    if (!string.IsNullOrEmpty(assetPath))
                        EditorUtil.PingAndActiveObject(assetPath);
                }
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    selectedBundle.RemoveAssetEntry(entry);
                    SavePackage();
                    i--;
                    continue;
                }
                EditorGUILayout.EndHorizontal();
                var tags = entry.AssetTags == null ? string.Empty : string.Join(",", entry.AssetTags);
                var newTags = EditorGUILayout.TextField("Tags", tags);
                if (newTags != tags)
                    entry.AssetTags = ParseTags(newTags);
                EditorGUILayout.LabelField("Path", assetPath);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        void DrawAddAssetBar()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            var dropped = (UnityEngine.Object)EditorGUILayout.ObjectField("Add Asset", null, typeof(UnityEngine.Object), false);
            if (dropped != null)
            {
                AddAsset(AssetDatabase.GetAssetPath(dropped));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            addFolderPath = EditorGUILayout.TextField("Add Folder", addFolderPath);
            if (GUILayout.Button("Collect", GUILayout.Width(64)))
                AddFolder(addFolderPath);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region 底部操作栏
        void DrawOperationBar()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Assign All BundleNames"))
            {
                packageSO.AssignBundleNames();
                SavePackage();
                ShowStatus("All AssetBundleNames assigned !");
            }
            if (GUILayout.Button("Clear All BundleNames"))
            {
                packageSO.ClearBundleNames();
                SavePackage();
                ShowStatus("All AssetBundleNames cleared !");
            }
            if (GUILayout.Button("Open Build Window"))
            {
                ResourceBuildWindow.OpenWindow();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        void DrawStatusBar()
        {
            if (string.IsNullOrEmpty(statusMessage))
                return;
            if (EditorApplication.timeSinceStartup > statusEndTime)
            {
                statusMessage = string.Empty;
                return;
            }
            EditorGUILayout.HelpBox(statusMessage, MessageType.Info);
        }
        #endregion

        #region 操作实现
        void CreateNewPackage()
        {
            var package = EditorUtil.CreateScriptableObject<ResourcePackageSO>(ResourceEditorConstants.NEW_PACKAGE_PATH);
            if (package != null)
            {
                package.PackageName = "DefaultPackage";
                package.PackageVersion = "0.0.1";
                EditorUtil.SaveScriptableObject(package);
                packageSO = package;
                selectedBundle = null;
                ShowStatus($"ResourcePackageSO created : {AssetDatabase.GetAssetPath(package)}");
            }
        }

        void CreateBundle(string bundleName)
        {
            if (packageSO == null)
                return;
            if (string.IsNullOrEmpty(bundleName))
            {
                ShowStatus("Bundle name is invalid !");
                return;
            }
            var packagePath = AssetDatabase.GetAssetPath(packageSO);
            if (string.IsNullOrEmpty(packagePath))
            {
                ShowStatus("Package asset path is invalid, please save the package first !");
                return;
            }
            var directory = Path.GetDirectoryName(packagePath);
            var assetPath = Utility.IO.PathCombine(directory, $"{packageSO.GetResolvedPackageName()}_{ResourceUtility.FilterName(bundleName)}.asset");
            // 生成不冲突的唯一资产路径（同名文件存在时自动追加后缀）
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            var bundle = ScriptableObject.CreateInstance<PackageBundleSO>();
            bundle.BundleName = bundleName;
            bundle.ParentPackageHash = AssetDatabase.AssetPathToGUID(packagePath);
            AssetDatabase.CreateAsset(bundle, assetPath);
            packageSO.BundleSOList.Add(bundle);
            packageSO.OnAfterDeserialize();
            selectedBundle = bundle;
            SavePackage();
            ShowStatus($"Bundle created : {bundleName}");
        }

        void DeleteBundle(PackageBundleSO bundle)
        {
            if (packageSO == null || bundle == null)
                return;
            packageSO.BundleSOList.Remove(bundle);
            var assetPath = AssetDatabase.GetAssetPath(bundle);
            if (!string.IsNullOrEmpty(assetPath))
                AssetDatabase.DeleteAsset(assetPath);
            if (selectedBundle == bundle)
                selectedBundle = null;
            SavePackage();
            ShowStatus($"Bundle deleted : {bundle.GetResolvedBundleName()}");
        }

        void AddAsset(string assetPath)
        {
            if (selectedBundle == null)
            {
                ShowStatus("Please select a bundle first !");
                return;
            }
            if (string.IsNullOrEmpty(assetPath) || !assetPath.StartsWith("Assets/"))
            {
                ShowStatus($"Invalid asset path : {assetPath}");
                return;
            }
            if (!IsCollectableAsset(assetPath))
            {
                ShowStatus($"Asset is not collectable : {assetPath}");
                return;
            }
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(guid))
                return;
            if (IsGuidExists(guid))
            {
                ShowStatus($"Asset already exists in package : {assetPath}");
                return;
            }
            var entry = new AssetEntry()
            {
                Guid = guid,
                AssetPath = assetPath,
                Extension = Path.GetExtension(assetPath).ToLower(),
                Name = Path.GetFileNameWithoutExtension(assetPath),
                AssetTags = new string[0],
                BundleHash = selectedBundle.ParentPackageHash,
            };
            selectedBundle.AddAssetEntry(entry);
            SavePackage();
            ShowStatus($"Asset added : {assetPath}");
        }

        void AddFolder(string folderPath)
        {
            if (selectedBundle == null)
            {
                ShowStatus("Please select a bundle first !");
                return;
            }
            if (string.IsNullOrEmpty(folderPath) || !folderPath.StartsWith("Assets/"))
            {
                ShowStatus($"Invalid folder path : {folderPath}");
                return;
            }
            var guids = AssetDatabase.FindAssets("", new string[] { folderPath });
            int addedCount = 0;
            for (int i = 0; i < guids.Length; i++)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (string.IsNullOrEmpty(assetPath) || !assetPath.StartsWith("Assets/"))
                    continue;
                if (!IsCollectableAsset(assetPath))
                    continue;
                if (IsGuidExists(guids[i]))
                    continue;
                var entry = new AssetEntry()
                {
                    Guid = guids[i],
                    AssetPath = assetPath,
                    Extension = Path.GetExtension(assetPath).ToLower(),
                    Name = Path.GetFileNameWithoutExtension(assetPath),
                    AssetTags = new string[0],
                    BundleHash = selectedBundle.ParentPackageHash,
                };
                selectedBundle.AddAssetEntry(entry);
                addedCount++;
            }
            if (addedCount > 0)
                SavePackage();
            ShowStatus($"Folder collected : {folderPath} , added {addedCount} assets");
        }

        bool IsGuidExists(string guid)
        {
            foreach (var bundle in packageSO.BundleSOList)
            {
                if (bundle == null)
                    continue;
                foreach (var entry in bundle.AssetEntries)
                {
                    if (entry != null && entry.Guid == guid)
                        return true;
                }
            }
            return false;
        }

        static bool IsCollectableAsset(string assetPath)
        {
            var extension = Path.GetExtension(assetPath).ToLower();
            var extensions = ResourceEditorConstants.Extensions;
            for (int i = 0; i < extensions.Length; i++)
            {
                if (extensions[i] == extension)
                    return true;
            }
            return false;
        }

        static string[] ParseTags(string tagsContext)
        {
            if (string.IsNullOrEmpty(tagsContext))
                return new string[0];
            var tags = tagsContext.Split(new char[] { ',', '，', ';', '；' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new List<string>();
            for (int i = 0; i < tags.Length; i++)
            {
                var tag = tags[i].Trim();
                if (string.IsNullOrEmpty(tag))
                    continue;
                if (!result.Contains(tag))
                    result.Add(tag);
            }
            return result.ToArray();
        }

        void SavePackage()
        {
            if (packageSO == null)
                return;
            EditorUtility.SetDirty(packageSO);
            foreach (var bundle in packageSO.BundleSOList)
            {
                if (bundle != null)
                    EditorUtility.SetDirty(bundle);
            }
            AssetDatabase.SaveAssets();
        }

        void ShowStatus(string message)
        {
            statusMessage = message;
            statusEndTime = EditorApplication.timeSinceStartup + 4;
        }
        #endregion
    }
}
