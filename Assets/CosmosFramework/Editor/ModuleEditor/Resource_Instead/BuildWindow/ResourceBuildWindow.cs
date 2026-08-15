using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    /// <summary>
    /// 新资源构建窗口。
    /// <para>支持分包构建：选择一个ResourcePackageSO独立构建；</para>
    /// <para>支持编辑器模拟模式清单导出：无需构建AB即可在编辑器内模拟加载。</para>
    /// </summary>
    public class ResourceBuildWindow : EditorWindow
    {
        ResourcePackageSO packageSO;
        CFResourceBuildParams buildParams = CFResourceBuildParams.Default;
        Vector2 scrollPos;
        bool advancedFoldout;
        bool isBuilding;

        [MenuItem("Window/Cosmos/Module/Resource/ResourceBuild")]
        public static void OpenWindow()
        {
            var window = GetWindow<ResourceBuildWindow>("ResourceBuild", true);
            window.minSize = new Vector2(420, 520);
        }

        void OnEnable()
        {
            titleContent = new GUIContent("ResourceBuild");
        }

        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            DrawPackageSelection();
            DrawBuildParams();
            DrawBuildButtons();
            EditorGUILayout.EndScrollView();
        }

        void DrawPackageSelection()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("资源包裹 (Package)", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(isBuilding);
            packageSO = (ResourcePackageSO)EditorGUILayout.ObjectField("Package", packageSO, typeof(ResourcePackageSO), false);
            if (packageSO != null)
            {
                EditorGUILayout.LabelField("PackageName", packageSO.GetResolvedPackageName());
                EditorGUILayout.LabelField("PackageVersion", string.IsNullOrEmpty(packageSO.PackageVersion) ? "N/A" : packageSO.PackageVersion);
                EditorGUILayout.LabelField("BundleCount", packageSO.BundleSOList.Count.ToString());
            }
            if (GUILayout.Button("Create New Package"))
            {
                CreateNewPackage();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }
        void DrawBuildParams()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("构建参数 (Build Params)", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(isBuilding);
            buildParams.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build Target", buildParams.BuildTarget);
            buildParams.BuildVersion = EditorGUILayout.TextField("Build Version", buildParams.BuildVersion);
            buildParams.OutputRootPath = EditorGUILayout.TextField("Output Root Path", buildParams.OutputRootPath);
            buildParams.BuildAssetBundleOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumFlagsField("Build Options", buildParams.BuildAssetBundleOptions);
            buildParams.ClearOutputPath = EditorGUILayout.Toggle("Clear Output Path", buildParams.ClearOutputPath);

            advancedFoldout = EditorGUILayout.Foldout(advancedFoldout, "Advanced", true);
            if (advancedFoldout)
            {
                EditorGUI.indentLevel++;
                buildParams.CopyToStreamingAssets = EditorGUILayout.Toggle("Copy To StreamingAssets", buildParams.CopyToStreamingAssets);
                if (buildParams.CopyToStreamingAssets)
                {
                    buildParams.StreamingAssetsRelativePath = EditorGUILayout.TextField("StreamingAssets Path", buildParams.StreamingAssetsRelativePath);
                }
                buildParams.EncryptManifest = EditorGUILayout.Toggle("Encrypt Manifest", buildParams.EncryptManifest);
                if (buildParams.EncryptManifest)
                {
                    buildParams.ManifestEncryptionKey = EditorGUILayout.TextField("Encryption Key", buildParams.ManifestEncryptionKey);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }

        void DrawBuildButtons()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.BeginDisabledGroup(isBuilding);
            if (GUILayout.Button("Build Package (完整构建)", GUILayout.Height(28)))
            {
                StartBuild(false);
            }
            if (GUILayout.Button("Export Manifest Only (仅导出清单)", GUILayout.Height(24)))
            {
                StartBuild(true);
            }
            if (GUILayout.Button("Open ResourceEditor (资源编辑分类)", GUILayout.Height(22)))
            {
                ResourcePackageEditorWindow.OpenWindow();
            }
            EditorGUI.EndDisabledGroup();
            if (isBuilding)
            {
                EditorGUILayout.HelpBox("构建中，请勿关闭窗口...", MessageType.Info);
            }
            EditorGUILayout.EndVertical();
        }

        void StartBuild(bool manifestOnly)
        {
            if (packageSO == null)
            {
                EditorUtil.Debug.LogError("Please select a ResourcePackageSO first !");
                return;
            }
            isBuilding = true;
            try
            {
                EditorUtility.DisplayProgressBar("Cosmos ResourceBuild", "Collecting assets...", 0.1f);
                if (manifestOnly)
                {
                    var outputPath = Path.Combine(buildParams.OutputRootPath, packageSO.GetResolvedPackageName());
                    var filePath = CFResourceBuildPipeline.ExportPackageManifest(packageSO, outputPath);
                    EditorUtil.Debug.LogInfo($"PackageManifest exported : {filePath}");
                }
                else
                {
                    CFResourceBuildPipeline.BuildPackage(packageSO, buildParams);
                }
            }
            catch (Exception e)
            {
                EditorUtil.Debug.LogError($"Build failed : {e}");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                isBuilding = false;
            }
        }

        void CreateNewPackage()
        {
            var package = EditorUtil.CreateScriptableObject<ResourcePackageSO>(ResourceEditorConstants.NEW_PACKAGE_PATH);
            if (package != null)
            {
                package.PackageName = "DefaultPackage";
                package.PackageVersion = "0.0.1";
                EditorUtil.SaveScriptableObject(package);
                packageSO = package;
                EditorUtil.Debug.LogInfo($"ResourcePackageSO created : {AssetDatabase.GetAssetPath(package)}");
            }
        }
    }
}
