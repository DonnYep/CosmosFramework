using Cosmos.Resource;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections;
using System.IO;
using Cosmos.Unity.EditorCoroutines.Editor;
using System.Linq;

namespace Cosmos.Editor.Resource
{
    public class AssetDatabaseTab : ResourceWindowTabBase
    {
        ResourceBundleLabel resourceBundleLabel = new ResourceBundleLabel();
        ResourceObjectLabel resourceObjectLabel = new ResourceObjectLabel();
        public const string AssetDatabaseTabDataName = "ResourceEditor_AssetDatabaseTabData.json";
        AssetDatabaseTabData tabData;
        bool hasChanged = false;
        bool loadingMultiSelection = false;
        /// <summary>
        /// 对象的加载进度；
        /// </summary>
        int loadingObjectInfoProgress;
        /// <summary>
        /// 加载的ab包数量；
        /// </summary>
        int loadingBundleInfoLength;
        /// <summary>
        /// 当前加载的ab包序号；
        /// </summary>
        int currentLoadingBundleInfoIndex;
        /// <summary>
        /// 选中的协程；
        /// </summary>
        EditorCoroutine selectionCoroutine;
        /// <summary>
        /// 构建dataset协程
        /// </summary>
        EditorCoroutine buildDatasetCoroutine;
        public override void OnEnable()
        {
            resourceBundleLabel.OnEnable();
            resourceObjectLabel.OnEnable();
            resourceBundleLabel.OnAllDelete += OnAllBundleDelete;
            resourceBundleLabel.OnDelete += OnBundleDelete;
            resourceBundleLabel.OnSelectionChanged += OnSelectionChanged;
            resourceBundleLabel.OnRenameBundle += OnRenameBundle;
            resourceBundleLabel.OnSort += OnBundleSort; ;
            GetTabData();
            if (ResourceWindowDataProxy.ResourceDataset != null)
            {
                resourceBundleLabel.Clear();
                resourceObjectLabel.Clear();
                var bundleInfoList = ResourceWindowDataProxy.ResourceDataset.ResourceBundleInfoList;
                var bundleInfoLength = bundleInfoList.Count;
                for (int i = 0; i < bundleInfoLength; i++)
                {
                    var bundleInfo = bundleInfoList[i];
                    resourceBundleLabel.AddBundle(bundleInfo);
                }
                resourceBundleLabel.Reload();
                hasChanged = ResourceWindowDataProxy.ResourceDataset.IsChanged;
                DisplaySelectedBundle();
            }
        }
        public override void OnDisable()
        {
            SaveTabData();
            if (selectionCoroutine != null)
                EditorUtil.Coroutine.StopCoroutine(selectionCoroutine);
            if (buildDatasetCoroutine != null)
                EditorUtil.Coroutine.StopCoroutine(buildDatasetCoroutine);
        }
        public override void OnGUI(Rect rect)
        {
            EditorGUILayout.BeginVertical();
            {
                if (hasChanged)
                    EditorGUILayout.HelpBox("Dataset has been changed, please \"Build Dataset\" !", MessageType.Warning);
                EditorGUILayout.BeginHorizontal();
                {
                    DrawDragRect();
                    resourceBundleLabel.OnGUI(rect);
                    resourceObjectLabel.OnGUI(rect);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    if (loadingMultiSelection)
                    {
                        EditorGUILayout.LabelField($"Loading Progress . . .  {currentLoadingBundleInfoIndex}/{loadingBundleInfoLength}");

                        EditorGUILayout.LabelField($"Object loading . . .  {loadingObjectInfoProgress}%");
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Resource Editor");
                    }
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Build Dataset", GUILayout.MinWidth(128)))
                    {
                        resourceObjectLabel.Clear();
                        BuildDataset();
                    }
                    if (GUILayout.Button("Clear Dataset", GUILayout.MinWidth(128)))
                    {
                        if (ResourceWindowDataProxy.ResourceDataset == null)
                            return;
                        ResourceWindowDataProxy.ResourceDataset.Clear();
                        resourceBundleLabel.Clear();
                        resourceObjectLabel.Clear();
                        resourceBundleLabel.Reload();
                        resourceObjectLabel.Reload();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
        }
        public override void OnDatasetAssign()
        {
            if (ResourceWindowDataProxy.ResourceDataset != null)
            {
                resourceBundleLabel.Clear();
                var bundleInfoList = ResourceWindowDataProxy.ResourceDataset.ResourceBundleInfoList;
                var bundleLength = bundleInfoList.Count;
                for (int i = 0; i < bundleLength; i++)
                {
                    var bundleInfo = bundleInfoList[i];
                    resourceBundleLabel.AddBundle(bundleInfo);
                }
                resourceBundleLabel.Reload();
                resourceObjectLabel.Clear();
                hasChanged = ResourceWindowDataProxy.ResourceDataset.IsChanged;
                DisplaySelectedBundle();
            }
        }
        public override void OnDatasetRefresh()
        {
            OnDatasetAssign();
        }
        public override void OnDatasetUnassign()
        {
            resourceBundleLabel.Clear();
            resourceObjectLabel.Clear();
            tabData.SelectedBundleIds.Clear();
            hasChanged = false;
        }
        public EditorCoroutine BuildDataset()
        {
            if (buildDatasetCoroutine != null)
                EditorUtil.Coroutine.StopCoroutine(buildDatasetCoroutine);
            buildDatasetCoroutine = EditorUtil.Coroutine.StartCoroutine(EnumBuildDataset());
            return buildDatasetCoroutine;
        }
        void DrawDragRect()
        {
            if (ResourceWindowDataProxy.ResourceDataset == null)
                return;
            if (UnityEngine.Event.current.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                UnityEngine.Event.current.Use();
            }
            else if (UnityEngine.Event.current.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                if (DragAndDrop.paths.Length == 0 && DragAndDrop.objectReferences.Length > 0)
                {
                    foreach (Object obj in DragAndDrop.objectReferences)
                    {
                        EditorUtil.Debug.LogInfo("- " + obj);
                    }
                }
                else if (DragAndDrop.paths.Length > 0 && DragAndDrop.objectReferences.Length == 0)
                {
                    foreach (string path in DragAndDrop.paths)
                    {
                        EditorUtil.Debug.LogInfo("- " + path);
                    }
                }
                else if (DragAndDrop.paths.Length == DragAndDrop.objectReferences.Length)
                {
                    for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                    {
                        Object obj = DragAndDrop.objectReferences[i];
                        string path = DragAndDrop.paths[i];
                        if (!(obj is MonoScript) && (obj is DefaultAsset))
                        {
                            var bundleInfoList = ResourceWindowDataProxy.ResourceDataset.ResourceBundleInfoList;
                            var isInSameBundle = ResourceWindowUtility.CheckAssetsAndScenesInOneAssetBundle(path);
                            if (isInSameBundle)
                            {
                                var invalidBundleName = ResourceUtility.FilterName(path);
                                EditorUtil.Debug.LogError($"Cannot mark assets and scenes in one AssetBundle. AssetBundle name is {invalidBundleName}");
                                continue;
                            }
                            var bundleInfo = new ResourceBundleInfo()
                            {
                                BundleName = path,
                                BundlePath = path
                            };
                            if (!bundleInfoList.Contains(bundleInfo))
                            {
                                bundleInfoList.Add(bundleInfo);
                                bundleInfo.BundleKey = bundleInfo.BundleName;
                                resourceBundleLabel.AddBundle(bundleInfo);
                                ResourceWindowDataProxy.ResourceDataset.IsChanged = true;
                                hasChanged = true;
                            }
                        }
                    }
                    resourceBundleLabel.Reload();
                }
            }
        }
        void OnAllBundleDelete()
        {
            ResourceWindowDataProxy.ResourceDataset.ResourceBundleInfoList.Clear();
            resourceObjectLabel.Clear();
            tabData.SelectedBundleIds.Clear();
            ResourceWindowDataProxy.ResourceDataset.IsChanged = true;
        }
        void OnBundleDelete(IList<int> bundleIds)
        {
            if (ResourceWindowDataProxy.ResourceDataset == null)
                return;
            if (selectionCoroutine != null)
                EditorUtil.Coroutine.StopCoroutine(selectionCoroutine);
            var bundleInfos = ResourceWindowDataProxy.ResourceDataset.ResourceBundleInfoList;
            var rmlen = bundleIds.Count;
            var rmbundleInfos = new ResourceBundleInfo[rmlen];
            for (int i = 0; i < rmlen; i++)
            {
                var rmid = bundleIds[i];
                rmbundleInfos[i] = bundleInfos[rmid];
                tabData.SelectedBundleIds.Remove(rmid);
            }
            for (int i = 0; i < rmlen; i++)
            {
                bundleInfos.Remove(rmbundleInfos[i]);
            }
            ResourceWindowDataProxy.ResourceDataset.IsChanged = true;
            hasChanged = true;
            resourceObjectLabel.Clear();
        }
        void OnSelectionChanged(IList<int> selectedIds)
        {
            if (selectionCoroutine != null)
                EditorUtil.Coroutine.StopCoroutine(selectionCoroutine);
            selectionCoroutine = EditorUtil.Coroutine.StartCoroutine(EnumSelectionChanged(selectedIds));
        }
        void OnRenameBundle(int id, string newName)
        {
            if (ResourceWindowDataProxy.ResourceDataset == null)
                return;
            var bundles = ResourceWindowDataProxy.ResourceDataset.ResourceBundleInfoList;
            var dstBundle = bundles[id];
            dstBundle.BundleName = newName;
            dstBundle.BundleKey = newName;
            EditorUtility.SetDirty(ResourceWindowDataProxy.ResourceDataset);
            ResourceWindowDataProxy.ResourceDataset.IsChanged = true;
            hasChanged = true;
        }
        void OnBundleSort(IList<string> sortedNames)
        {
            if (ResourceWindowDataProxy.ResourceDataset == null)
                return;
            var bundleArray = ResourceWindowDataProxy.ResourceDataset.ResourceBundleInfoList.ToArray();
            ResourceWindowDataProxy.ResourceDataset.ResourceBundleInfoList.Clear();
            var length = sortedNames.Count;
            var bundleLength = bundleArray.Length;
            for (int i = 0; i < length; i++)
            {
                var name = sortedNames[i];
                for (int j = 0; j < bundleLength; j++)
                {
                    var bundle = bundleArray[j];
                    if (bundle.BundleName == name)
                    {
                        ResourceWindowDataProxy.ResourceDataset.ResourceBundleInfoList.Add(bundle);
                        continue;
                    }
                }
            }
            EditorUtility.SetDirty(ResourceWindowDataProxy.ResourceDataset);
#if UNITY_2021_1_OR_NEWER
            AssetDatabase.SaveAssetIfDirty(ResourceWindowDataProxy.ResourceDataset);
#elif UNITY_2019_1_OR_NEWER
            AssetDatabase.SaveAssets();
#endif
        }
        void GetTabData()
        {
            try
            {
                tabData = EditorUtil.GetData<AssetDatabaseTabData>(AssetDatabaseTabDataName);
            }
            catch
            {
                tabData = new AssetDatabaseTabData();
                EditorUtil.SaveData(AssetDatabaseTabDataName, tabData);
            }
        }
        void SaveTabData()
        {
            EditorUtil.SaveData(AssetDatabaseTabDataName, tabData);
        }
        IEnumerator EnumBuildDataset()
        {
            if (ResourceWindowDataProxy.ResourceDataset == null)
                yield break;
            resourceBundleLabel.Clear();
            var bundleInfos = ResourceWindowDataProxy.ResourceDataset.ResourceBundleInfoList;
            var extensions = ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList;
            var lowerExtensions = extensions.Select(s => s.ToLower()).ToArray();
            extensions.Clear();
            extensions.AddRange(lowerExtensions);
            var bundleLength = bundleInfos.Count;

            List<ResourceBundleInfo> invalidBundleInfos = new List<ResourceBundleInfo>();

            for (int i = 0; i < bundleLength; i++)
            {
                var bundleInfo = bundleInfos[i];
                var bundlePath = bundleInfo.BundlePath;
                if (!AssetDatabase.IsValidFolder(bundlePath))
                {
                    invalidBundleInfos.Add(bundleInfo);
                    continue;
                }
                var importer = AssetImporter.GetAtPath(bundleInfo.BundlePath);
                importer.assetBundleName = bundleInfo.BundleName;

                var files = Utility.IO.GetAllFiles(bundlePath);
                var fileLength = files.Length;
                bundleInfo.ResourceObjectInfoList.Clear();
                for (int j = 0; j < fileLength; j++)
                {
                    var srcFilePath = files[j].Replace("\\", "/");
                    var srcFileExt = Path.GetExtension(srcFilePath);
                    var lowerFileExt = srcFileExt.ToLower();
                    if (extensions.Contains(lowerFileExt))
                    {
                        //统一使用小写的文件后缀名
                        var lowerExtFilePath = srcFilePath.Replace(srcFileExt, lowerFileExt);

                        var resourceObjectInfo = new ResourceObjectInfo()
                        {
                            BundleName = bundleInfo.BundleName,
                            Extension = lowerFileExt,
                            ObjectName = Path.GetFileNameWithoutExtension(lowerExtFilePath),
                            ObjectPath = lowerExtFilePath,
                            ObjectSize = EditorUtil.GetAssetFileSizeLength(lowerExtFilePath),
                            ObjectFormatBytes = EditorUtil.GetAssetFileSize(lowerExtFilePath),
                        };
                        var asset = AssetDatabase.LoadMainAssetAtPath(resourceObjectInfo.ObjectPath);
                        if (asset != null)
                        {
                            resourceObjectInfo.ObjectIcon = AssetDatabase.GetCachedIcon(lowerExtFilePath) as Texture2D;
                            resourceObjectInfo.ObjectState = "VALID";
                            resourceObjectInfo.ObjectStateIcon = ResourceWindowUtility.GetAssetValidIcon();
                        }
                        else
                        {
                            resourceObjectInfo.ObjectIcon = EditorGUIUtility.FindTexture("console.erroricon");
                            resourceObjectInfo.ObjectState = "INVALID";
                            resourceObjectInfo.ObjectStateIcon = ResourceWindowUtility.GetAssetInvalidIcon();
                        }
                        bundleInfo.ResourceObjectInfoList.Add(resourceObjectInfo);
                    }
                }
                long bundleSize = EditorUtil.GetUnityDirectorySize(bundlePath, ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList);
                bundleInfo.BundleSize = bundleSize;
                bundleInfo.BundleKey = bundleInfo.BundleName;
                bundleInfo.BundleFormatBytes = EditorUtility.FormatBytes(bundleSize);

                resourceBundleLabel.AddBundle(bundleInfo);

                var bundlePercent = i / (float)bundleLength;
                EditorUtility.DisplayProgressBar("BuildDataset building", $"building bundle : {Mathf.RoundToInt(bundlePercent * 100)}%", bundlePercent);
                yield return null;
            }
            EditorUtility.DisplayProgressBar("BuildDataset building", $"building bundle : {100}%", 1);
            //yield return null;
            for (int i = 0; i < invalidBundleInfos.Count; i++)
            {
                bundleInfos.Remove(invalidBundleInfos[i]);
            }
            for (int i = 0; i < bundleInfos.Count; i++)
            {
                var bundleInfo = bundleInfos[i];
                var importer = AssetImporter.GetAtPath(bundleInfo.BundlePath);
                bundleInfo.DependentBundleKeyList.Clear();
                bundleInfo.DependentBundleKeyList.AddRange(AssetDatabase.GetAssetBundleDependencies(importer.assetBundleName, true));
            }
            for (int i = 0; i < bundleInfos.Count; i++)
            {
                var bundleInfo = bundleInfos[i];
                var importer = AssetImporter.GetAtPath(bundleInfo.BundlePath);
                importer.assetBundleName = string.Empty;
            }
            yield return null;
            EditorUtility.ClearProgressBar();
            EditorUtility.SetDirty(ResourceWindowDataProxy.ResourceDataset);
#if UNITY_2021_1_OR_NEWER
            AssetDatabase.SaveAssetIfDirty(ResourceWindowDataProxy.ResourceDataset);
#elif UNITY_2019_1_OR_NEWER
            AssetDatabase.SaveAssets();
#endif
            resourceBundleLabel.Reload();

            ResourceWindowDataProxy.ResourceDataset.IsChanged = false;
            hasChanged = false;
            yield return null;
            SaveTabData();
            DisplaySelectedBundle();
        }
        IEnumerator EnumSelectionChanged(IList<int> selectedIds)
        {
            if (ResourceWindowDataProxy.ResourceDataset == null)
                yield break;
            loadingMultiSelection = true;
            var bundleInfos = ResourceWindowDataProxy.ResourceDataset.ResourceBundleInfoList;
            var idlen = selectedIds.Count;
            loadingBundleInfoLength = idlen;

            resourceObjectLabel.Clear();
            resourceObjectLabel.Reload();

            loadingObjectInfoProgress = 0;
            currentLoadingBundleInfoIndex = 0;
            for (int i = 0; i < idlen; i++)
            {
                currentLoadingBundleInfoIndex++;

                var id = selectedIds[i];
                if (id >= bundleInfos.Count)
                    continue;
                var objectInfos = bundleInfos[id].ResourceObjectInfoList;
                var objectInfoLength = objectInfos.Count;
                for (int j = 0; j < objectInfoLength; j++)
                {
                    var objectInfo = objectInfos[j];
                    resourceObjectLabel.AddObject(objectInfo);
                    var progress = Mathf.RoundToInt((float)j / (objectInfoLength - 1) * 100);
                    loadingObjectInfoProgress = progress > 0 ? progress : 0;
                }
                yield return null;
                resourceObjectLabel.Reload();
            }
            yield return null;

            loadingMultiSelection = false;
            tabData.SelectedBundleIds.Clear();
            tabData.SelectedBundleIds.AddRange(selectedIds);
            SaveTabData();
        }
        void DisplaySelectedBundle()
        {
            var bundleIds = tabData.SelectedBundleIds;
            resourceBundleLabel.SetSelection(bundleIds);
            OnSelectionChanged(bundleIds);
        }
    }
}
