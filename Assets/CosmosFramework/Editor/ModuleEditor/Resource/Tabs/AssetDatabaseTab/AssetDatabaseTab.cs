using Cosmos.Resource;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections;
using System.IO;
using Cosmos;
namespace Cosmos.Editor.Resource
{
    public class AssetDatabaseTab
    {
        ResourceBundleLable resourceBundleLable = new ResourceBundleLable();
        ResourceObjectLable resourceObjectLable = new ResourceObjectLable();
        public const string AssetDatasetTabDataName = "ResourceEditor_AssetDatasetTabData.json";
        AssetDatasetTabData tabData;
        bool hasChanged = false;
        public void OnEnable()
        {
            resourceBundleLable.OnEnable();
            resourceObjectLable.OnEnable();
            resourceBundleLable.OnAllDelete += OnAllBundleDelete;
            resourceBundleLable.OnDelete += OnBundleDelete;
            resourceBundleLable.onBundleClick += OnBundleClick;
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
                    resourceBundleLable.AddBundle(new ResourceBundleInfo() { BundleName = bundle.BundleName });
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
                EditorGUILayout.BeginHorizontal("box");
                {
                    DrawDragRect();
                    resourceBundleLable.OnGUI(rect);
                    resourceObjectLable.OnGUI(rect);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Build Dataset", GUILayout.MinWidth(128)))
                    {
                        resourceObjectLable.Clear();
                        EditorUtil.Coroutine.StartCoroutine(BuildDataset());
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
                    resourceBundleLable.AddBundle(new ResourceBundleInfo() { BundleName = bundle.BundleName });
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
            tabData.HighlightBundleIndex = 0;
            hasChanged = false;
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
                            var resourceBundle = new ResourceBundle(path);
                            if (!bundleList.Contains(resourceBundle))
                            {
                                bundleList.Add(resourceBundle);
                            }
                            var bundle = new ResourceBundleInfo() { BundleName = path };
                            added = resourceBundleLable.AddBundle(bundle);
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
            tabData.HighlightBundleIndex = 0;
            hasChanged = true;
        }
        void OnBundleDelete(List<ResourceBundleInfo> infos)
        {
            if (ResourceEditorDataProxy.ResourceDataset == null)
                return;
            var bundles = ResourceEditorDataProxy.ResourceDataset.ResourceBundleList;
            var bundleLen = bundles.Count;
            var indexList = new List<int>();
            for (int i = 0; i < bundleLen; i++)
            {
                foreach (var info in infos)
                {
                    if (bundles[i].BundleName == info.BundleName)
                        indexList.Add(i);
                }
            }
            for (int i = 0; i < indexList.Count; i++)
            {
                bundles.RemoveAt(indexList[i]);
                if (tabData.HighlightBundleIndex == indexList[i])
                {
                    resourceObjectLable.Clear();
                }
            }
            hasChanged = true;
        }
        void OnBundleClick(int bundleIndex)
        {
            if (ResourceEditorDataProxy.ResourceDataset == null)
                return;
            var bundles = ResourceEditorDataProxy.ResourceDataset.ResourceBundleList;
            ResourceBundle resourceBundle = null;
            if (bundleIndex > bundles.Count - 1)
                bundleIndex = bundles.Count - 1;
            if (bundles.Count > 0)
                resourceBundle = bundles[bundleIndex];
            if (resourceBundle == null)
                return;
            resourceObjectLable.Clear();
            var objects = resourceBundle.ResourceObjectList;
            var objectLength = objects.Count;
            for (int i = 0; i < objectLength; i++)
            {
                var objInfo = new ResourceObjectInfo(objects[i].AssetName, objects[i].AssetPath);
                resourceObjectLable.AddObject(objInfo);
            }
            tabData.HighlightBundleIndex = bundleIndex;
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
        IEnumerator BuildDataset()
        {
            if (ResourceEditorDataProxy.ResourceDataset == null)
                yield break;
            var bundles = ResourceEditorDataProxy.ResourceDataset.ResourceBundleList;
            var objects = ResourceEditorDataProxy.ResourceDataset.ResourceObjectList;
            var extensions = ResourceEditorDataProxy.ResourceDataset.ResourceAvailableExtenisonList;
            objects.Clear();
            var bundleLength = bundles.Count;
            for (int i = 0; i < bundleLength; i++)
            {
                var bundlePath = bundles[i].BundleName;
                var files = Utility.IO.GetAllFiles(bundlePath);
                var fileLength = files.Length;
                for (int j = 0; j < fileLength; j++)
                {
                    var srcFilePath = files[j].Replace("\\", "/");
                    var fileExt = Path.GetExtension(srcFilePath);
                    if (extensions.Contains(fileExt))
                    {
                        var resourceObject = new ResourceObject(Path.GetFileName(srcFilePath), srcFilePath, bundlePath, Path.GetExtension(srcFilePath));
                        objects.Add(resourceObject);
                        bundles[i].ResourceObjectList.Add(resourceObject);
                    }
                    var percent = j / (float)fileLength;
                    EditorUtility.DisplayProgressBar("BuildDataset building", $"Checking bundle : {bundlePath} {Mathf.RoundToInt(percent * 100)}%", percent);
                    yield return null;
                }
            }
            EditorUtility.ClearProgressBar();

            EditorUtility.SetDirty(ResourceEditorDataProxy.ResourceDataset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            DisplaySelectedBundle();
            hasChanged = false;
        }
        void DisplaySelectedBundle()
        {
            var index = tabData.HighlightBundleIndex;
            resourceBundleLable.SetSetSelectionBundle(index);
            OnBundleClick(index);
        }
    }
}
