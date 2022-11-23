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
        int loadingObjectProgress;
        /// <summary>
        /// 加载的ab包数量；
        /// </summary>
        int loadingBundleLength;
        /// <summary>
        /// 当前加载的ab包序号；
        /// </summary>
        int currentLoadingBundleIndex;
        /// <summary>
        /// 选中的协程；
        /// </summary>
        EditorCoroutine selectionCoroutine;
        public override void OnEnable()
        {
            resourceBundleLabel.OnEnable();
            resourceObjectLabel.OnEnable();
            resourceBundleLabel.OnAllDelete += OnAllBundleDelete;
            resourceBundleLabel.OnDelete += OnBundleDelete;
            resourceBundleLabel.OnSelectionChanged += OnSelectionChanged;
            resourceBundleLabel.OnRenameBundle += OnRenameBundle;
            GetTabData();
            if (ResourceWindowDataProxy.ResourceDataset != null)
            {
                resourceBundleLabel.Clear();
                resourceObjectLabel.Clear();
                var bundleList = ResourceWindowDataProxy.ResourceDataset.ResourceBundleList;
                var bundleLen = bundleList.Count;
                for (int i = 0; i < bundleLen; i++)
                {
                    var bundle = bundleList[i];
                    long bundleSize = EditorUtil.GetUnityDirectorySize(bundle.BundlePath, ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList);
                    resourceBundleLabel.AddBundle(new ResourceBundleInfo(bundle.BundleName, bundle.BundlePath, bundleSize, bundle.ResourceObjectList.Count));
                }
                hasChanged = ResourceWindowDataProxy.ResourceDataset.IsChanged;
                DisplaySelectedBundle();
            }
        }
        public override void OnDisable()
        {
            SaveTabData();
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
                        EditorGUILayout.LabelField($"Loading Progress . . .  {currentLoadingBundleIndex}/{loadingBundleLength}");

                        EditorGUILayout.LabelField($"Object loading . . .  {loadingObjectProgress}%");
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
                var bundleList = ResourceWindowDataProxy.ResourceDataset.ResourceBundleList;
                var bundleLen = bundleList.Count;
                for (int i = 0; i < bundleLen; i++)
                {
                    var bundle = bundleList[i];
                    long bundleSize = EditorUtil.GetUnityDirectorySize(bundle.BundlePath, ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList);
                    resourceBundleLabel.AddBundle(new ResourceBundleInfo(bundle.BundleName, bundle.BundlePath, bundleSize, bundle.ResourceObjectList.Count));
                }
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
            return EditorUtil.Coroutine.StartCoroutine(EnumBuildDataset());
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
                            var bundleList = ResourceWindowDataProxy.ResourceDataset.ResourceBundleList;
                            var isInSameBundle = ResourceWindowUtility.CheckAssetsAndScenesInOneAssetBundle(path);
                            if (isInSameBundle)
                            {
                                var invalidBundleName = ResourceUtility.BundleNameFilter(path);
                                EditorUtil.Debug.LogError($"Cannot mark assets and scenes in one AssetBundle. AssetBundle name is {invalidBundleName}");
                                continue;
                            }
                            var bundle = new ResourceBundle()
                            {
                                BundleName = path,
                                BundlePath = path
                            };
                            if (!bundleList.Contains(bundle))
                            {
                                bundleList.Add(bundle);
                                long bundleSize = EditorUtil.GetUnityDirectorySize(path, ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList);
                                var bundleInfo = new ResourceBundleInfo(bundle.BundleName, bundle.BundlePath, bundleSize, bundle.ResourceObjectList.Count);
                                resourceBundleLabel.AddBundle(bundleInfo);
                                ResourceWindowDataProxy.ResourceDataset.IsChanged = true;
                                hasChanged = true;
                            }
                        }
                    }
                }
            }
        }
        void OnAllBundleDelete()
        {
            ResourceWindowDataProxy.ResourceDataset.ResourceBundleList.Clear();
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
            var bundles = ResourceWindowDataProxy.ResourceDataset.ResourceBundleList;
            var rmlen = bundleIds.Count;
            var rmbundles = new ResourceBundle[rmlen];
            for (int i = 0; i < rmlen; i++)
            {
                var rmid = bundleIds[i];
                rmbundles[i] = bundles[rmid];
                tabData.SelectedBundleIds.Remove(rmid);
            }
            for (int i = 0; i < rmlen; i++)
            {
                bundles.Remove(rmbundles[i]);
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
            var bundles = ResourceWindowDataProxy.ResourceDataset.ResourceBundleList;
            var dstBundle = bundles[id];
            dstBundle.BundleName = newName;
            EditorUtility.SetDirty(ResourceWindowDataProxy.ResourceDataset);
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
            var bundles = ResourceWindowDataProxy.ResourceDataset.ResourceBundleList;
            var objects = ResourceWindowDataProxy.ResourceDataset.ResourceObjectList;
            var extensions = ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList;
            var scenes = ResourceWindowDataProxy.ResourceDataset.ResourceSceneList;
            var lowerExtensions = extensions.Select(s => s.ToLower()).ToArray();
            extensions.Clear();
            extensions.AddRange(lowerExtensions);
            objects.Clear();
            scenes.Clear();
            var bundleLength = bundles.Count;

            List<ResourceBundleInfo> validBundleInfo = new List<ResourceBundleInfo>();
            List<ResourceBundle> invalidBundles = new List<ResourceBundle>();

            for (int i = 0; i < bundleLength; i++)
            {
                var bundle = bundles[i];
                var bundlePath = bundle.BundlePath;
                if (!AssetDatabase.IsValidFolder(bundlePath))
                {
                    invalidBundles.Add(bundle);
                    continue;
                }
                var importer = AssetImporter.GetAtPath(bundle.BundlePath);
                importer.assetBundleName = bundle.BundleName;

                var files = Utility.IO.GetAllFiles(bundlePath);
                var fileLength = files.Length;
                bundle.ResourceObjectList.Clear();
                for (int j = 0; j < fileLength; j++)
                {
                    var srcFilePath = files[j].Replace("\\", "/");
                    var srcFileExt = Path.GetExtension(srcFilePath);
                    var lowerFileExt = srcFileExt.ToLower();
                    if (extensions.Contains(lowerFileExt))
                    {
                        //统一使用小写的文件后缀名
                        var lowerExtFilePath = srcFilePath.Replace(srcFileExt, lowerFileExt);
                        var resourceObject = new ResourceObject(Path.GetFileNameWithoutExtension(lowerExtFilePath), lowerExtFilePath, bundle.BundleName, lowerFileExt);
                        objects.Add(resourceObject);
                        bundle.ResourceObjectList.Add(resourceObject);
                        if (lowerFileExt == ResourceConstants.UNITY_SCENE_EXTENSION)//表示为场景资源
                        {
                            scenes.Add(resourceObject);
                        }
                    }
                }
                long bundleSize = EditorUtil.GetUnityDirectorySize(bundlePath, ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList);
                var bundleInfo = new ResourceBundleInfo(bundle.BundleName, bundle.BundlePath, bundleSize, bundle.ResourceObjectList.Count);
                validBundleInfo.Add(bundleInfo);

                var bundlePercent = i / (float)bundleLength;
                EditorUtility.DisplayProgressBar("BuildDataset building", $"building bundle : {Mathf.RoundToInt(bundlePercent * 100)}%", bundlePercent);
                yield return null;
            }
            EditorUtility.DisplayProgressBar("BuildDataset building", $"building bundle : {100}%", 1);
            //yield return null;
            for (int i = 0; i < invalidBundles.Count; i++)
            {
                bundles.Remove(invalidBundles[i]);
            }
            for (int i = 0; i < bundles.Count; i++)
            {
                var bundle = bundles[i];
                var importer = AssetImporter.GetAtPath(bundle.BundlePath);
                bundle.DependentList.Clear();
                bundle.DependentList.AddRange(AssetDatabase.GetAssetBundleDependencies(importer.assetBundleName, true));
            }
            for (int i = 0; i < bundles.Count; i++)
            {
                var bundle = bundles[i];
                var importer = AssetImporter.GetAtPath(bundle.BundlePath);
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
            {
                //这么处理是为了bundleLable能够在编辑器页面一下刷新，放在协程里逐步更新，使用体验并不是很好。
                resourceBundleLabel.Clear();
                for (int i = 0; i < validBundleInfo.Count; i++)
                {
                    resourceBundleLabel.AddBundle(validBundleInfo[i]);
                }
            }
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
            var bundles = ResourceWindowDataProxy.ResourceDataset.ResourceBundleList;
            var idlen = selectedIds.Count;
            loadingBundleLength = idlen;

            resourceObjectLabel.Clear();
            loadingObjectProgress = 0;
            currentLoadingBundleIndex = 0;
            for (int i = 0; i < idlen; i++)
            {
                currentLoadingBundleIndex++;

                var id = selectedIds[i];
                if (id >= bundles.Count)
                    continue;
                var objects = bundles[id].ResourceObjectList;
                var objectLength = objects.Count;
                for (int j = 0; j < objectLength; j++)
                {
                    var obj = objects[j];
                    var assetPath = obj.AssetPath;
                    var valid = AssetDatabase.LoadMainAssetAtPath(obj.AssetPath) != null;
                    var objInfo = new ResourceObjectInfo(obj.AssetName, assetPath, obj.BundleName, EditorUtil.GetAssetFileSize(assetPath), obj.Extension, valid);
                    resourceObjectLabel.AddObject(objInfo);

                    var progress = Mathf.RoundToInt((float)j / (objectLength - 1) * 100);
                    loadingObjectProgress = progress > 0 ? progress : 0;
                    yield return null;
                }
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
