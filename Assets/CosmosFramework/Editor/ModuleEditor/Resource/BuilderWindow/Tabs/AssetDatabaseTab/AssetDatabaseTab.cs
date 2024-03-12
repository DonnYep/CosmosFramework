using Cosmos.Resource;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections;
using System.IO;
using System.Linq;

namespace Cosmos.Editor.Resource
{
    public class AssetDatabaseTab : ResourceBuilderWindowTabBase
    {
        AssetDatabaseBundleDetailLabel bundleDetailLabel = new AssetDatabaseBundleDetailLabel();
        AssetDatabaseBundleLabel bundleLabel = new AssetDatabaseBundleLabel();
        AssetDatabaseObjectLabel objectLabel = new AssetDatabaseObjectLabel();
        public const string AssetDatabaseTabDataName = "ResourceBuilderWindow_AssetDatabaseTabData.json";
        string[] tabArray = new string[] { "Resource objects", "Dependencies" };
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
        Cosmos.Unity.EditorCoroutines.Editor.EditorCoroutine selectionCoroutine;
        /// <summary>
        /// 构建dataset协程
        /// </summary>
        Cosmos.Unity.EditorCoroutines.Editor.EditorCoroutine buildDatasetCoroutine;

        long selectedObjectSize;

        int selectedBundleCount;
        long selectedBundleSize;

        long totalObjectSize;
        long totoalBundleSize;

        Rect horizontalSplitterRect;
        Rect rightRect;
        Rect leftRect;
        bool resizingHorizontalSplitter = false;
        float horizontalSplitterPercent;
        bool rectSplitterInited;
        public AssetDatabaseTab(EditorWindow parentWindow) : base(parentWindow)
        {
        }
        public override void OnEnable()
        {
            rectSplitterInited = false;
            horizontalSplitterPercent = ResourceEditorConstants.MIN_WIDTH;
            bundleLabel.OnEnable();
            bundleDetailLabel.OnEnable();
            objectLabel.OnEnable();
            bundleLabel.OnAllBundleDelete += OnAllBundleDelete;
            bundleLabel.OnBundleDelete += OnBundleDelete;
            bundleLabel.OnSelectionChanged += OnSelectionChanged;
            bundleLabel.OnBundleRenamed += OnRenameBundle;
            bundleLabel.OnBundleSort += OnBundleSort;
            bundleLabel.OnMarkAsSplit += OnMarkAsSplit;
            bundleLabel.OnMarkAsNotSplit += OnMarkAsNotSplit;
            bundleLabel.OnMarkAsExtract += OnMarkAsExtract;
            bundleLabel.OnMarkAsNotExtract += OnMarkAsNotExtract;
            objectLabel.OnObjectInfoSelectionChanged += OnObjectInfoSelectionChanged;
            tabData = EditorUtil.SafeGetData<AssetDatabaseTabData>(ResourceEditorConstants.EDITOR_CACHE_RELATIVE_PATH, AssetDatabaseTabDataName);
            if (ResourceBuilderWindowDataProxy.ResourceDataset != null)
            {
                bundleLabel.Clear();
                objectLabel.Clear();
                var bundleInfoList = ResourceBuilderWindowDataProxy.ResourceDataset.ResourceBundleInfoList;
                var bundleInfoLength = bundleInfoList.Count;
                for (int i = 0; i < bundleInfoLength; i++)
                {
                    var bundleInfo = bundleInfoList[i];
                    bundleLabel.AddBundle(bundleInfo);
                }
                bundleLabel.Reload();
                hasChanged = ResourceBuilderWindowDataProxy.ResourceDataset.IsChanged;
                DisplayBundleDetail();
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
            HandleHorizontalResize();

            if (hasChanged)
                EditorGUILayout.HelpBox("Dataset has been changed, please \"Build Dataset\" !", MessageType.Warning);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical();
                {
                    DrawDragRect();
                    var width = GUILayout.Width((ParentWindow.position.width * horizontalSplitterPercent - 6) / 2);
                    using (var scope = new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
                    {
                        EditorGUILayout.LabelField($"Amount: {bundleLabel.BundleCount}/{GetTotalBundleFormatSize()}", EditorStyles.boldLabel, width);
                        EditorGUILayout.LabelField($"Selected: {selectedBundleCount}/{GetSelectedBundleFormatSize()}", EditorStyles.boldLabel, width);
                    }
                    bundleLabel.OnGUI(leftRect);
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        tabData.LabelTabIndex = EditorGUILayout.Popup(tabData.LabelTabIndex, tabArray, EditorStyles.toolbarPopup, GUILayout.MaxWidth(128));
                        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
                        {
                            if (tabData.LabelTabIndex == 0)
                            {
                                GUILayout.Label($"Amount: {objectLabel.ObjectCount}/{GetTotalObjectFormatSize()}", EditorStyles.boldLabel);
                                GUILayout.Label($"Selected: {objectLabel.SelectedCount}/{GetSelectedObjectFormatSize()}", EditorStyles.boldLabel);
                            }
                            else if (tabData.LabelTabIndex == 1)
                            {
                                GUILayout.Label($"{tabArray[tabData.LabelTabIndex]} count: {bundleDetailLabel.BundleDetailCount}", EditorStyles.boldLabel);
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    if (tabData.LabelTabIndex == 0)
                    {
                        objectLabel.OnGUI(rightRect);
                    }
                    else if (tabData.LabelTabIndex == 1)
                    {
                        bundleDetailLabel.OnGUI(rightRect);
                    }
                }
                EditorGUILayout.EndVertical();
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
                if (GUILayout.Button("Build Dataset", GUILayout.MinWidth(ResourceEditorConstants.BUTTON_WIDTH)))
                {
                    bundleDetailLabel.Clear();
                    objectLabel.Clear();
                    BuildDataset();
                }
                if (GUILayout.Button("Clear Dataset", GUILayout.MinWidth(ResourceEditorConstants.BUTTON_WIDTH)))
                {
                    if (ResourceBuilderWindowDataProxy.ResourceDataset == null)
                        return;
                    ResourceBuilderWindowDataProxy.ResourceDataset.Clear();
                    ClearLabels();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        public override void OnDatasetAssign()
        {
            if (ResourceBuilderWindowDataProxy.ResourceDataset != null)
            {
                bundleLabel.Clear();
                var bundleInfoList = ResourceBuilderWindowDataProxy.ResourceDataset.ResourceBundleInfoList;
                var bundleLength = bundleInfoList.Count;
                totoalBundleSize = 0;
                for (int i = 0; i < bundleLength; i++)
                {
                    var bundleInfo = bundleInfoList[i];
                    bundleLabel.AddBundle(bundleInfo);
                    totoalBundleSize += bundleInfo.BundleSize;
                }
                bundleLabel.Reload();
                objectLabel.Clear();
                hasChanged = ResourceBuilderWindowDataProxy.ResourceDataset.IsChanged;
                bundleDetailLabel.SetSelection(tabData.SelectedBundleIds);
                DisplaySelectedBundle();
            }
        }
        public override void OnDatasetRefresh()
        {
            OnDatasetAssign();
        }
        public override void OnDatasetUnassign()
        {
            ClearLabels();
            tabData.SelectedBundleIds.Clear();
            hasChanged = false;
        }
        public Cosmos.Unity.EditorCoroutines.Editor.EditorCoroutine BuildDataset()
        {
            if (buildDatasetCoroutine != null)
                EditorUtil.Coroutine.StopCoroutine(buildDatasetCoroutine);
            buildDatasetCoroutine = EditorUtil.Coroutine.StartCoroutine(EnumBuildDataset());
            return buildDatasetCoroutine;
        }
        void DrawDragRect()
        {
            if (ResourceBuilderWindowDataProxy.ResourceDataset == null)
                return;
            var mousePositon = UnityEngine.Event.current.mousePosition;
            if (!bundleLabel.LabelTreeViewRect.Contains(mousePositon))
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
                            var bundleInfoList = ResourceBuilderWindowDataProxy.ResourceDataset.ResourceBundleInfoList;
                            var isSceneInSameBundle = ResourceEditorUtility.CheckAssetsAndScenesInOneAssetBundle(path);
                            if (isSceneInSameBundle)
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
                                bundleLabel.AddBundle(bundleInfo);
                                ResourceBuilderWindowDataProxy.ResourceDataset.IsChanged = true;
                                hasChanged = true;
                            }
                        }
                    }
                    bundleLabel.Reload();
                }
            }
        }
        void OnAllBundleDelete()
        {
            ResourceBuilderWindowDataProxy.ResourceDataset.ResourceBundleInfoList.Clear();
            objectLabel.Clear();
            objectLabel.Reload();
            tabData.SelectedBundleIds.Clear();
            bundleDetailLabel.Clear();
            bundleDetailLabel.Reload();

            ResourceBuilderWindowDataProxy.ResourceDataset.IsChanged = true;
            EditorUtil.SaveScriptableObject(ResourceBuilderWindowDataProxy.ResourceDataset);
        }
        void OnBundleDelete(IList<int> bundleIds, IList<int> selectedIds)
        {
            if (ResourceBuilderWindowDataProxy.ResourceDataset == null)
                return;
            if (selectionCoroutine != null)
                EditorUtil.Coroutine.StopCoroutine(selectionCoroutine);
            var bundleInfos = ResourceBuilderWindowDataProxy.ResourceDataset.ResourceBundleInfoList;
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
            ResourceBuilderWindowDataProxy.ResourceDataset.IsChanged = true;
            hasChanged = true;
            OnSelectionChanged(selectedIds);
        }
        void OnSelectionChanged(IList<int> selectedIds)
        {
            if (selectionCoroutine != null)
                EditorUtil.Coroutine.StopCoroutine(selectionCoroutine);
            selectionCoroutine = EditorUtil.Coroutine.StartCoroutine(EnumSelectionChanged(selectedIds));
        }
        void OnRenameBundle(int id, string newName)
        {
            if (ResourceBuilderWindowDataProxy.ResourceDataset == null)
                return;
            var bundles = ResourceBuilderWindowDataProxy.ResourceDataset.ResourceBundleInfoList;
            var dstBundle = bundles[id];
            dstBundle.BundleName = newName;
            dstBundle.BundleKey = newName;
            ResourceBuilderWindowDataProxy.ResourceDataset.IsChanged = true;
            EditorUtil.SaveScriptableObject(ResourceBuilderWindowDataProxy.ResourceDataset);
            hasChanged = true;
        }
        void OnBundleSort(IList<string> sortedNames, IList<int> selectedIds)
        {
            if (ResourceBuilderWindowDataProxy.ResourceDataset == null)
                return;
            var bundleArray = ResourceBuilderWindowDataProxy.ResourceDataset.ResourceBundleInfoList.ToArray();
            ResourceBuilderWindowDataProxy.ResourceDataset.ResourceBundleInfoList.Clear();
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
                        ResourceBuilderWindowDataProxy.ResourceDataset.ResourceBundleInfoList.Add(bundle);
                        continue;
                    }
                }
            }
            EditorUtil.SaveScriptableObject(ResourceBuilderWindowDataProxy.ResourceDataset);
            OnSelectionChanged(selectedIds);
        }
        void OnMarkAsSplit(IList<int> bundleIds)
        {
            MarkChanged();
        }
        void OnMarkAsNotSplit(IList<int> bundleIds)
        {
            MarkChanged();
        }
        void OnMarkAsNotExtract(IList<int> obj)
        {
            MarkChanged();

        }
        void OnMarkAsExtract(IList<int> obj)
        {
            MarkChanged();
        }
        void OnObjectInfoSelectionChanged(List<ResourceObjectInfo> selected)
        {
            var length = selected.Count;
            selectedObjectSize = 0;
            for (int i = 0; i < length; i++)
            {
                selectedObjectSize += selected[i].ObjectSize;
            }
        }
        void SaveTabData()
        {
            EditorUtil.SaveData(ResourceEditorConstants.EDITOR_CACHE_RELATIVE_PATH, AssetDatabaseTabDataName, tabData);
        }
        IEnumerator EnumBuildDataset()
        {
            if (ResourceBuilderWindowDataProxy.ResourceDataset == null)
                yield break;
            bundleDetailLabel.Clear();
            bundleLabel.Clear();
            var bundleInfos = ResourceBuilderWindowDataProxy.ResourceDataset.ResourceBundleInfoList;
            var extensions = ResourceBuilderWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList;
            var lowerExtensions = extensions.Select(s => s.ToLower()).ToArray();
            extensions.Clear();
            extensions.AddRange(lowerExtensions);
            var bundleLength = bundleInfos.Count;
            totoalBundleSize = 0;
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
                bundleInfo.ResourceSubBundleInfoList.Clear();

                if (bundleInfo.Split)
                {
                    BuildSplittableBundleInfo(ref bundleInfo, extensions);
                    bundleLabel.AddBundle(bundleInfo);
                }
                else
                {
                    BuildUnsplittableBundleInfo(ref bundleInfo, extensions);
                    bundleLabel.AddBundle(bundleInfo);
                }
                totoalBundleSize += bundleInfo.BundleSize;
                var bundlePercent = i / (float)bundleLength;
                EditorUtility.DisplayProgressBar("BuildDataset building", $"building bundle : {Mathf.RoundToInt(bundlePercent * 100)}%", bundlePercent);
                yield return null;
            }
            EditorUtility.DisplayProgressBar("BuildDataset building", $"building bundle : {100}%", 1);
            //设置AssetImporter后刷新
            AssetDatabase.Refresh();
            for (int i = 0; i < invalidBundleInfos.Count; i++)
            {
                bundleInfos.Remove(invalidBundleInfos[i]);
            }
            for (int i = 0; i < bundleInfos.Count; i++)
            {
                var bundleInfo = bundleInfos[i];
                BuildBundleInfoDependent(ref bundleInfo);
            }
            for (int i = 0; i < bundleInfos.Count; i++)
            {
                var bundleInfo = bundleInfos[i];
                ResetBundleInfo(ref bundleInfo);
            }
            yield return null;
            EditorUtility.ClearProgressBar();
            bundleLabel.Reload();
            ResourceBuilderWindowDataProxy.ResourceDataset.IsChanged = false;
            ResourceBuilderWindowDataProxy.ResourceDataset.RegenerateBundleInfoDict();
            EditorUtil.SaveScriptableObject(ResourceBuilderWindowDataProxy.ResourceDataset);
            hasChanged = false;
            yield return null;
            AssetDatabase.Refresh();
            SaveTabData();
            DisplaySelectedBundle();
        }
        IEnumerator EnumSelectionChanged(IList<int> selectedIds)
        {
            if (ResourceBuilderWindowDataProxy.ResourceDataset == null)
                yield break;
            loadingMultiSelection = true;
            var bundleInfos = ResourceBuilderWindowDataProxy.ResourceDataset.ResourceBundleInfoList;
            var idlen = selectedIds.Count;
            loadingBundleInfoLength = idlen;
            totalObjectSize = 0;
            selectedBundleCount = selectedIds.Count;
            selectedBundleSize = 0;

            bundleDetailLabel.Clear();
            bundleDetailLabel.Reload();
            objectLabel.Clear();
            objectLabel.Reload();

            loadingObjectInfoProgress = 0;
            currentLoadingBundleInfoIndex = 0;
            for (int i = 0; i < idlen; i++)
            {
                currentLoadingBundleInfoIndex++;
                var id = selectedIds[i];
                if (id >= bundleInfos.Count)
                    continue;
                var bundleInfo = bundleInfos[id];
                bundleDetailLabel.AddBundle(bundleInfo);
                selectedBundleSize += bundleInfo.BundleSize;
                var objectInfos = bundleInfo.ResourceObjectInfoList;
                var objectInfoLength = objectInfos.Count;
                for (int j = 0; j < objectInfoLength; j++)
                {
                    var objectInfo = objectInfos[j];
                    objectLabel.AddObject(objectInfo);
                    var progress = Mathf.RoundToInt((float)j / (objectInfoLength - 1) * 100);
                    loadingObjectInfoProgress = progress > 0 ? progress : 0;
                    totalObjectSize += objectInfo.ObjectSize;
                }
                yield return null;
                objectLabel.Reload();
                bundleDetailLabel.Reload();
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
            bundleLabel.SetSelection(bundleIds);
            OnSelectionChanged(bundleIds);
        }
        void DisplayBundleDetail()
        {
            var bundleIds = tabData.SelectedBundleIds;
            bundleDetailLabel.SetSelection(bundleIds);
        }
        void BuildSplittableBundleInfo(ref ResourceBundleInfo bundleInfo, List<string> extensions)
        {
            var bundlePath = bundleInfo.BundlePath;
            var subBundlePaths = AssetDatabase.GetSubFolders(bundlePath);
            for (int j = 0; j < subBundlePaths.Length; j++)
            {
                var subBundlePath = subBundlePaths[j];
                var isSceneInSameBundle = ResourceEditorUtility.CheckAssetsAndScenesInOneAssetBundle(subBundlePath);
                if (isSceneInSameBundle)
                {
                    var invalidBundleName = ResourceUtility.FilterName(subBundlePath);
                    EditorUtil.Debug.LogError($"Cannot mark assets and scenes in one AssetBundle. AssetBundle name is {invalidBundleName}");
                    continue;
                }
                var subBundleInfo = new ResourceSubBundleInfo()
                {
                    BundleName = subBundlePath,
                    BundlePath = subBundlePath
                };
                var contain = bundleInfo.ResourceSubBundleInfoList.Contains(subBundleInfo);
                if (contain)
                {
                    continue;
                }

                bundleInfo.ResourceSubBundleInfoList.Add(subBundleInfo);
                subBundleInfo.BundleKey = subBundleInfo.BundleName;

                var subImporter = AssetImporter.GetAtPath(subBundleInfo.BundlePath);
                subImporter.assetBundleName = subBundleInfo.BundleName;
                var files = Utility.IO.GetAllFiles(subBundlePath);
                var fileLength = files.Length;
                subBundleInfo.ResourceObjectInfoList.Clear();
                for (int k = 0; k < fileLength; k++)
                {
                    var srcFilePath = files[k].Replace("\\", "/");
                    var srcFileExt = Path.GetExtension(srcFilePath);
                    var lowerFileExt = srcFileExt.ToLower();
                    if (extensions.Contains(lowerFileExt))
                    {
                        //统一使用小写的文件后缀名
                        var lowerExtFilePath = srcFilePath.Replace(srcFileExt, lowerFileExt);

                        var resourceObjectInfo = new ResourceObjectInfo()
                        {
                            BundleName = subBundleInfo.BundleName,
                            Extension = lowerFileExt,
                            ObjectName = Path.GetFileNameWithoutExtension(lowerExtFilePath),
                            ObjectPath = lowerExtFilePath,
                            ObjectSize = EditorUtil.GetAssetFileSizeLength(lowerExtFilePath),
                            ObjectFormatBytes = EditorUtil.GetAssetFileSize(lowerExtFilePath),
                        };
                        resourceObjectInfo.ObjectVaild = AssetDatabase.LoadMainAssetAtPath(resourceObjectInfo.ObjectPath) != null;
                        subBundleInfo.ResourceObjectInfoList.Add(resourceObjectInfo);
                    }
                    long subBundleSize = EditorUtil.GetUnityDirectorySize(subBundlePath, ResourceBuilderWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList);
                    subBundleInfo.BundleSize = subBundleSize;
                    subBundleInfo.BundleKey = subBundleInfo.BundleName;
                    subBundleInfo.BundleFormatBytes = EditorUtility.FormatBytes(subBundleSize);
                }
            }

            long bundleSize = EditorUtil.GetUnityDirectorySize(bundlePath, ResourceBuilderWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList);
            bundleInfo.BundleSize = bundleSize;
            bundleInfo.BundleKey = bundleInfo.BundleName;
            bundleInfo.BundleFormatBytes = EditorUtility.FormatBytes(bundleSize);
        }
        void BuildUnsplittableBundleInfo(ref ResourceBundleInfo bundleInfo, List<string> extensions)
        {
            var bundlePath = bundleInfo.BundlePath;
            var subBundlePaths = AssetDatabase.GetSubFolders(bundlePath);
            var subBundlePathLength = subBundlePaths.Length;
            for (int j = 0; j < subBundlePathLength; j++)
            {
                var subImporter = AssetImporter.GetAtPath(subBundlePaths[j]);
                subImporter.assetBundleName = string.Empty; ;
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
                    resourceObjectInfo.ObjectVaild = AssetDatabase.LoadMainAssetAtPath(resourceObjectInfo.ObjectPath) != null;
                    bundleInfo.ResourceObjectInfoList.Add(resourceObjectInfo);
                }
            }
            long bundleSize = EditorUtil.GetUnityDirectorySize(bundlePath, ResourceBuilderWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList);
            bundleInfo.BundleSize = bundleSize;
            bundleInfo.BundleKey = bundleInfo.BundleName;
            bundleInfo.BundleFormatBytes = EditorUtility.FormatBytes(bundleSize);
        }
        void BuildBundleInfoDependent(ref ResourceBundleInfo bundleInfo)
        {
            if (!bundleInfo.Split)
            {
                var importer = AssetImporter.GetAtPath(bundleInfo.BundlePath);
                bundleInfo.BundleDependencies.Clear();
                var dependencies = AssetDatabase.GetAssetBundleDependencies(importer.assetBundleName, true);
                var dependenciesLength = dependencies.Length;
                for (int j = 0; j < dependenciesLength; j++)
                {
                    var dependency = dependencies[j];
                    var bundleDependency = new ResourceBundleDependency()
                    {
                        BundleKey = dependency,
                        BundleName = dependency
                    };
                    bundleInfo.BundleDependencies.Add(bundleDependency);
                }
            }
            else
            {
                var subBundleInfoList = bundleInfo.ResourceSubBundleInfoList;
                var length = subBundleInfoList.Count;
                for (int i = 0; i < length; i++)
                {
                    var subBundleInfo = subBundleInfoList[i];
                    var subImporter = AssetImporter.GetAtPath(subBundleInfo.BundlePath);
                    subBundleInfo.BundleDependencies.Clear();
                    var subDependencies = AssetDatabase.GetAssetBundleDependencies(subImporter.assetBundleName, true);
                    var subDependenciesLength = subDependencies.Length;
                    for (int j = 0; j < subDependenciesLength; j++)
                    {
                        var subDependency = subDependencies[j];
                        var subBundleDependency = new ResourceBundleDependency()
                        {
                            BundleKey = subDependency,
                            BundleName = subDependency
                        };
                        subBundleInfo.BundleDependencies.Add(subBundleDependency);
                    }
                }
            }
        }
        void ResetBundleInfo(ref ResourceBundleInfo bundleInfo)
        {
            if (!bundleInfo.Split)
            {
                var importer = AssetImporter.GetAtPath(bundleInfo.BundlePath);
                importer.assetBundleName = string.Empty;
            }
            else
            {
                var subBundleInfoList = bundleInfo.ResourceSubBundleInfoList;
                var length = subBundleInfoList.Count;
                for (int i = 0; i < length; i++)
                {
                    var subBundleInfo = subBundleInfoList[i];
                    var subImporter = AssetImporter.GetAtPath(subBundleInfo.BundlePath);
                    subImporter.assetBundleName = string.Empty;
                }
            }
        }
        void HandleHorizontalResize()
        {
            var position = ParentWindow.position;
            EditorGUIUtility.AddCursorRect(horizontalSplitterRect, MouseCursor.ResizeHorizontal);

            if (UnityEngine.Event.current.type == EventType.MouseDown && horizontalSplitterRect.Contains(UnityEngine.Event.current.mousePosition))
                resizingHorizontalSplitter = true;

            if (resizingHorizontalSplitter)
            {
                horizontalSplitterPercent = Mathf.Clamp(UnityEngine.Event.current.mousePosition.x / position.width, 0.1f, 0.9f);
            }
            var leftWidth = position.width * horizontalSplitterPercent;
            var rightWidth = position.width * (1 - horizontalSplitterPercent);

            if (!rectSplitterInited)
            {
                horizontalSplitterRect = new Rect(leftWidth, position.y + 80, 5, 2048);
                rightRect = new Rect(0, 0, rightWidth, position.height);
                leftRect = new Rect(0, 0, leftWidth, position.height);
                ParentWindow.Repaint();
                rectSplitterInited = true;
            }
            horizontalSplitterRect.y = bundleLabel.LabelTreeViewRect.y;
            horizontalSplitterRect.height = bundleLabel.LabelTreeViewRect.height;
            horizontalSplitterRect.x = leftWidth;
            horizontalSplitterRect.width = 5;
            leftRect.width = leftWidth;
            rightRect.width = rightWidth;
            ParentWindow.Repaint();
            if (UnityEngine.Event.current.type == EventType.MouseUp)
                resizingHorizontalSplitter = false;
        }
        void ClearLabels()
        {
            bundleLabel.Clear();
            bundleLabel.Reload();
            bundleDetailLabel.Clear();
            bundleDetailLabel.Reload();
            objectLabel.Clear();
            objectLabel.Reload();

            totoalBundleSize = 0;
            selectedBundleCount = 0;
            selectedBundleSize = 0;
            totalObjectSize = 0;
        }
        string GetTotalBundleFormatSize()
        {
            return Utility.Converter.FormatBytes(totoalBundleSize);
        }
        string GetSelectedBundleFormatSize()
        {
            return Utility.Converter.FormatBytes(selectedBundleSize);
        }
        string GetTotalObjectFormatSize()
        {
            return Utility.Converter.FormatBytes(totalObjectSize);
        }
        string GetSelectedObjectFormatSize()
        {
            return Utility.Converter.FormatBytes(selectedObjectSize);
        }
        void MarkChanged()
        {
            ResourceBuilderWindowDataProxy.ResourceDataset.IsChanged = true;
            EditorUtil.SaveScriptableObject(ResourceBuilderWindowDataProxy.ResourceDataset);
            hasChanged = true;
        }
    }
}
