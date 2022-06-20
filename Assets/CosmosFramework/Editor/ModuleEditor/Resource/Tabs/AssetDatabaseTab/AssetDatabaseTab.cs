using Cosmos.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Cosmos.Editor.Resource
{
    public class AssetDatabaseTab
    {
        ResourceBundleLable resourceBundleLable = new ResourceBundleLable();
        ResourceObjectLable resourceObjectLable = new ResourceObjectLable();
        public void OnUnassign()
        {
            resourceBundleLable.Clear();
            resourceObjectLable.Clear();
        }
        public void OnAssign()
        {
            if (ResourceEditorDataProxy.ResourceDataset != null)
            {
                resourceBundleLable.Clear();
                var bundleList = ResourceEditorDataProxy.ResourceDataset.ResourceBundleList;
                var bundleLen = bundleList.Count;
                for (int i = 0; i < bundleLen; i++)
                {
                    var bundle = bundleList[i];
                    resourceBundleLable.AddBundle(new ResourceBundleInfo() { BundlePath = bundle.BundleName });
                }

                resourceObjectLable.Clear();
                var objectList = ResourceEditorDataProxy.ResourceDataset.ResourceObjectList;
                var objectLen = objectList.Count;
                for (int i = 0; i < objectLen; i++)
                {
                    var obj = objectList[i];
                    resourceObjectLable.AddObject(new ResourceObjectInfo() { AssetPath = obj.AssetPath, ObjectName = obj.AssetName });
                }
            }
        }
        public void OnEnable()
        {
            resourceBundleLable.OnEnable();
            resourceObjectLable.OnEnable();
            resourceBundleLable.OnAllDelete += OnAllBundleDelete;
            resourceBundleLable.OnDelete += OnBundleDelete;
            if (ResourceEditorDataProxy.ResourceDataset != null)
            {
                resourceBundleLable.Clear();
                var bundleList = ResourceEditorDataProxy.ResourceDataset.ResourceBundleList;
                var bundleLen = bundleList.Count;
                for (int i = 0; i < bundleLen; i++)
                {
                    var bundle = bundleList[i];
                    resourceBundleLable.AddBundle(new ResourceBundleInfo() { BundlePath = bundle.BundleName });
                }
            }
        }
        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    DrawDragRect();
                    resourceBundleLable.OnGUI();
                    resourceObjectLable.OnGUI();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Build Dataset", GUILayout.MinWidth(128)))
                    {

                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
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
                    for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                    {
                        Object obj = DragAndDrop.objectReferences[i];
                        string path = DragAndDrop.paths[i];
                        if (!(obj is MonoScript) && (obj is DefaultAsset))
                        {
                            var bundleList = ResourceEditorDataProxy.ResourceDataset.ResourceBundleList;
                            var rb = new ResourceBundle() { BundleName = path };
                            if (!bundleList.Contains(rb))
                            {
                                bundleList.Add(rb);
                            }
                            var bundle = new ResourceBundleInfo() { BundlePath = path, BundleName = obj.name };
                            resourceBundleLable.AddBundle(bundle);
                        }
                    }
                }
            }
        }
        void OnAllBundleDelete()
        {
            ResourceEditorDataProxy.ResourceDataset.ResourceBundleList.Clear();
        }
        void OnBundleDelete(List<ResourceBundleInfo> infos)
        {
            if (ResourceEditorDataProxy.ResourceDataset == null)
                return;
            EditorUtil.Debug.LogInfo("OnBundleDelete");
            var bundles = ResourceEditorDataProxy.ResourceDataset.ResourceBundleList;
            var bundleLen = bundles.Count;
            var indexList = new List<int>();
            for (int i = 0; i < bundleLen; i++)
            {
                foreach (var info in infos)
                {
                    if (bundles[i].BundleName== info.BundlePath)
                        indexList.Add(i);
                }
            }
            for (int i = 0; i < indexList.Count; i++)
            {
                bundles.RemoveAt(indexList[i]);
            }
        }
    }
}
