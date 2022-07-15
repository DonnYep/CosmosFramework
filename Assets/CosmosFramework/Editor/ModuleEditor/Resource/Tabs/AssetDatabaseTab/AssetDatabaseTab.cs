using Cosmos.Resource;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections;
using System.IO;
using Unity.EditorCoroutines.Editor;

namespace Cosmos.Editor.Resource
{
    public class AssetDatabaseTab
    {
        ResourceBundleLable resourceBundleLable = new ResourceBundleLable();
        ResourceObjectLable resourceObjectLable = new ResourceObjectLable();
        public const string AssetDatasetTabDataName = "ResourceEditor_AssetDatasetTabData.json";
        AssetDatasetTabData tabData;
        bool hasChanged = false;
        bool loadingMultiSelection = false;
        int loadingProgress;
        EditorCoroutine selectionCoroutine;
        public void OnEnable()
        {
            resourceBundleLable.OnEnable();
            resourceObjectLable.OnEnable();
            resourceBundleLable.OnAllDelete += OnAllBundleDelete;
            resourceBundleLable.OnDelete += OnBundleDelete;
            resourceBundleLable.OnSelectionChanged += OnSelectionChanged;
            resourceBundleLable.OnRenameBundle += OnRenameBundle;
            GetTabData();
            if (ResourceEditorDataProxy.ResourceDataset != null)
            {
                resourceBundleLable.Clear();
                resourceObjectLable.Clear();
                var bundleList = ResourceEditorDataProxy.ResourceDataset.ResourceBundleList;
                var bundleLen = bundleList.Count;
                for (int i = 0; i < bundleLen; i++)
                {
                    var bundle = bundleList[i];
                    long bundleSize = ResourceEditorUtility.GetDirectorySize(bundle.BundlePath, ResourceEditorDataProxy.ResourceDataset.ResourceAvailableExtenisonList);
                    resourceBundleLable.AddBundle(new ResourceBundleInfo(bundle.BundleName, bundle.BundlePath, EditorUtility.FormatBytes(bundleSize)));
                }
                if (ResourceEditorDataProxy.ResourceDataset.ResourceObjectCount == 0 && ResourceEditorDataProxy.ResourceDataset.ResourceBundleCount > 0)
                    hasChanged = true;
                DisplaySelectedBundle();
            }
        }
        public void OnDisable()
        {
            SaveTabData();
        }
        public void OnGUI(Rect rect)
        {
            EditorGUILayout.BeginVertical();
            {
                if (hasChanged)
                    EditorGUILayout.HelpBox("Dataset has been changed, please \"Build Dataset\" !", MessageType.Warning);
                EditorGUILayout.BeginHorizontal();
                {
                    DrawDragRect();
                    resourceBundleLable.OnGUI(rect);
                    resourceObjectLable.OnGUI(rect);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    if (loadingMultiSelection)
                    {
                        EditorGUILayout.LabelField($"Object loading . . .  {loadingProgress}%");
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Resource Editor");
                    }
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Build Dataset", GUILayout.MinWidth(128)))
                    {
                        resourceObjectLable.Clear();
                        BuildDataset();
                    }
                    if (GUILayout.Button("Clear Dataset", GUILayout.MinWidth(128)))
                    {
                        if (ResourceEditorDataProxy.ResourceDataset == null)
                            return;
                        ResourceEditorDataProxy.ResourceDataset.Clear();
                        resourceBundleLable.Clear();
                        resourceObjectLable.Clear();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
        }
        public void OnDatasetAssign()
        {
            if (ResourceEditorDataProxy.ResourceDataset != null)
            {
                resourceBundleLable.Clear();
                var bundleList = ResourceEditorDataProxy.ResourceDataset.ResourceBundleList;
                var bundleLen = bundleList.Count;
                for (int i = 0; i < bundleLen; i++)
                {
                    var bundle = bundleList[i];
                    long bundleSize = ResourceEditorUtility.GetDirectorySize(bundle.BundlePath, ResourceEditorDataProxy.ResourceDataset.ResourceAvailableExtenisonList);
                    resourceBundleLable.AddBundle(new ResourceBundleInfo(bundle.BundleName, bundle.BundlePath, EditorUtility.FormatBytes(bundleSize)));
                }
                resourceObjectLable.Clear();
                if (ResourceEditorDataProxy.ResourceDataset.ResourceObjectCount == 0 && ResourceEditorDataProxy.ResourceDataset.ResourceBundleCount > 0)
                    hasChanged = true;
                DisplaySelectedBundle();
            }
        }
        public void OnDatasetUnassign()
        {
            resourceBundleLable.Clear();
            resourceObjectLable.Clear();
            tabData.SelectedBundleIds.Clear();
            hasChanged = false;
        }
        public EditorCoroutine BuildDataset()
        {
            return EditorUtil.Coroutine.StartCoroutine(EnumBuildDataset());
        }
        void DrawDragRect()
        {
            if (ResourceEditorDataProxy.ResourceDataset == null)
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
                    bool added = false;
                    for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                    {
                        Object obj = DragAndDrop.objectReferences[i];
                        string path = DragAndDrop.paths[i];
                        if (!(obj is MonoScript) && (obj is DefaultAsset))
                        {
                            var bundleList = ResourceEditorDataProxy.ResourceDataset.ResourceBundleList;
                            var bundle = new ResourceBundle()
                            {
                                BundleName = path,
                                BundlePath = path
                            };
                            if (!bundleList.Contains(bundle))
                            {
                                bundleList.Add(bundle);
                            }
                            long bundleSize = ResourceEditorUtility.GetDirectorySize(path, ResourceEditorDataProxy.ResourceDataset.ResourceAvailableExtenisonList);
                            var bundleInfo = new ResourceBundleInfo(bundle.BundleName, bundle.BundlePath, EditorUtility.FormatBytes(bundleSize));
                            added = resourceBundleLable.AddBundle(bundleInfo);
                        }
                    }
                    if (added)
                        hasChanged = true;
                }
            }
        }
        void OnAllBundleDelete()
        {
            ResourceEditorDataProxy.ResourceDataset.ResourceBundleList.Clear();
            resourceObjectLable.Clear();
            tabData.SelectedBundleIds.Clear();
            hasChanged = true;
        }
        void OnBundleDelete(IList<int> bundleIds)
        {
            if (ResourceEditorDataProxy.ResourceDataset == null)
                return;
            if (selectionCoroutine != null)
                EditorUtil.Coroutine.StopCoroutine(selectionCoroutine);
            var bundles = ResourceEditorDataProxy.ResourceDataset.ResourceBundleList;
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
            hasChanged = true;
            resourceObjectLable.Clear();
        }
        void OnSelectionChanged(IList<int> selectedIds)
        {
            selectionCoroutine = EditorUtil.Coroutine.StartCoroutine(EnumSelectionChanged(selectedIds));
        }
        void OnRenameBundle(int id, string newName)
        {
            if (ResourceEditorDataProxy.ResourceDataset == null)
                return;
            var bundles = ResourceEditorDataProxy.ResourceDataset.ResourceBundleList;
            var dstBundle = bundles[id];
            dstBundle.BundleName = newName;
            EditorUtility.SetDirty(ResourceEditorDataProxy.ResourceDataset);
        }
        void GetTabData()
        {
            try
            {
                tabData = EditorUtil.GetData<AssetDatasetTabData>(AssetDatasetTabDataName);
            }
            catch
            {
                tabData = new AssetDatasetTabData();
                EditorUtil.SaveData(AssetDatasetTabDataName, tabData);
            }
        }
        void SaveTabData()
        {
            EditorUtil.SaveData(AssetDatasetTabDataName, tabData);
        }
        IEnumerator EnumBuildDataset()
        {
            if (ResourceEditorDataProxy.ResourceDataset == null)
                yield break;
            var bundles = ResourceEditorDataProxy.ResourceDataset.ResourceBundleList;
            var objects = ResourceEditorDataProxy.ResourceDataset.ResourceObjectList;
            var extensions = ResourceEditorDataProxy.ResourceDataset.ResourceAvailableExtenisonList;
            objects.Clear();
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
                var files = Utility.IO.GetAllFiles(bundlePath);
                var fileLength = files.Length;
                bundle.ResourceObjectList.Clear();
                for (int j = 0; j < fileLength; j++)
                {
                    var srcFilePath = files[j].Replace("\\", "/");
                    var fileExt = Path.GetExtension(srcFilePath);
                    if (extensions.Contains(fileExt))
                    {
                        var resourceObject = new ResourceObject(Path.GetFileNameWithoutExtension(srcFilePath), srcFilePath, bundle.BundleName, Path.GetExtension(srcFilePath));
                        objects.Add(resourceObject);
                        bundle.ResourceObjectList.Add(resourceObject);
                    }
                }
                long bundleSize = ResourceEditorUtility.GetDirectorySize(bundlePath, ResourceEditorDataProxy.ResourceDataset.ResourceAvailableExtenisonList);
                var bundleInfo = new ResourceBundleInfo(bundle.BundleName, bundle.BundlePath, EditorUtility.FormatBytes(bundleSize));
                validBundleInfo.Add(bundleInfo);

                var bundlePercent = i / (float)bundleLength;
                EditorUtility.DisplayProgressBar("BuildDataset building", $"building bundle : {Mathf.RoundToInt(bundlePercent * 100)}%", bundlePercent);
                yield return null;
            }
            EditorUtility.DisplayProgressBar("BuildDataset building", $"building bundle : {100}%", 1);
            yield return null;

            for (int i = 0; i < invalidBundles.Count; i++)
            {
                bundles.Remove(invalidBundles[i]);
            }

            EditorUtility.ClearProgressBar();
            EditorUtility.SetDirty(ResourceEditorDataProxy.ResourceDataset);
            AssetDatabase.SaveAssets();
            {
                //这么处理是为了bundleLable能够在编辑器页面一下刷新，放在协程里逐步更新，使用体验并不是很好。
                resourceBundleLable.Clear();
                for (int i = 0; i < validBundleInfo.Count; i++)
                {
                    resourceBundleLable.AddBundle(validBundleInfo[i]);
                }
            }

            hasChanged = false;
            yield return null;
            SaveTabData();
            DisplaySelectedBundle();
        }
        IEnumerator EnumSelectionChanged(IList<int> selectedIds)
        {
            if (ResourceEditorDataProxy.ResourceDataset == null)
                yield break;
            loadingMultiSelection = true;
            var bundles = ResourceEditorDataProxy.ResourceDataset.ResourceBundleList;
            var idlen = selectedIds.Count;
            resourceObjectLable.Clear();
            for (int i = 0; i < idlen; i++)
            {
                var id = selectedIds[i];
                if (id >= bundles.Count)
                    continue;
                var objects = bundles[id].ResourceObjectList;
                var objectLength = objects.Count;
                for (int j = 0; j < objectLength; j++)
                {
                    var obj = objects[j];
                    var assetPath = obj.AssetPath;
                    var objInfo = new ResourceObjectInfo(obj.AssetName, assetPath, obj.BundleName, EditorUtil.GetAssetFileSize(assetPath), obj.Extension);
                    resourceObjectLable.AddObject(objInfo);
                }
                var progress = Mathf.RoundToInt((float)i / (idlen - 1) * 100); ;
                loadingProgress = progress > 0 ? progress : 0;
                yield return null;
            }
            loadingProgress = 100;

            loadingMultiSelection = false;
            tabData.SelectedBundleIds.Clear();
            tabData.SelectedBundleIds.AddRange(selectedIds);
            SaveTabData();
        }
        void DisplaySelectedBundle()
        {
            var bundleIds = tabData.SelectedBundleIds;
            resourceBundleLable.SetSelection(bundleIds);
            OnSelectionChanged(bundleIds);
        }
    }
}
