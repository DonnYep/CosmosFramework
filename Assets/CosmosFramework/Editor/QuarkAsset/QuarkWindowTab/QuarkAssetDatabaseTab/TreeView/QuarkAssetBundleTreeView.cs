using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Quark.Asset;

namespace Quark.Editor
{
    public class QuarkAssetBundleTreeView : TreeView
    {
        string originalName;
        public QuarkAssetBundleTreeView(TreeViewState treeViewState)
      : base(treeViewState)
        {
            Reload();
            showAlternatingRowBackgrounds = true;
            showBorder = true;
        }
        public void Clear()
        {
            Reload();
        }
        public void AddPath(string path)
        {
            if (QuarkEditorDataProxy.QuarkAssetDataset == null)
                return;
            var namePathInfoList = QuarkEditorDataProxy.QuarkAssetDataset.QuarkBundleInfoList;
            var length = namePathInfoList.Count;
            bool existed = false;
            for (int i = 0; i < length; i++)
            {
                if (namePathInfoList[i].AssetBundlePath == path)
                {
                    existed = true;
                    break;
                }
            }
            if (!existed)
            {
                var hash = AssetDatabase.AssetPathToGUID(path);
                var pair = new QuarkBundleInfo(path, path);
                QuarkEditorDataProxy.QuarkAssetDataset.QuarkBundleInfoList.Add(pair);
            }
            Reload();
        }
        public void RemovePath(string path)
        {
            try
            {
                if (QuarkEditorDataProxy.QuarkAssetDataset == null)
                    return;
                var dirHashPairs = QuarkEditorDataProxy.QuarkAssetDataset.QuarkBundleInfoList;
                var length = dirHashPairs.Count;
                int removeindex = -1;
                for (int i = 0; i < length; i++)
                {
                    if (dirHashPairs[i].AssetBundlePath == path)
                    {
                        removeindex = i;
                        break;
                    }
                }
                if (removeindex != -1)
                {
                    QuarkEditorDataProxy.QuarkAssetDataset.QuarkBundleInfoList.RemoveAt(removeindex);
                }
            }
            catch (Exception e)
            {
                QuarkUtility.LogError(e);
            }
            Reload();
        }

        //public override void OnGUI(Rect rect)
        //{
        //    if (UnityEngine.Event.current.type == EventType.Repaint)
        //        DefaultStyles.backgroundOdd.Draw(rect, false, false, false, false);
        //    try
        //    {
        //        if (QuarkEditorDataProxy.QuarkAssetDataset == null)
        //            return;
        //        var dirHashPairs = QuarkEditorDataProxy.QuarkAssetDataset.DirHashPairs;
        //        var dirHashCount = dirHashPairs.Count;
        //        int removeCount = 0;
        //        int[] removeIndexArray = new int[dirHashCount];
        //        for (int i = 0; i < dirHashCount; i++)
        //        {
        //            var srcHash = dirHashPairs[i].DirHash;
        //            var srcDir = dirHashPairs[i].Dir;
        //            var newPath = AssetDatabase.GUIDToAssetPath(srcHash);
        //            var newHash = AssetDatabase.AssetPathToGUID(srcDir);
        //            if (newPath != dirHashPairs[i].Dir)
        //            {
        //                if (!string.IsNullOrEmpty(newPath))
        //                    dirHashPairs[i] = new QuarkDirHashPair(srcHash, newPath);
        //            }
        //            else
        //            {
        //                var filePath = Path.Combine(Directory.GetCurrentDirectory(), srcDir);
        //                if (!File.Exists(filePath) && !Directory.Exists(filePath))
        //                {
        //                    if (!File.Exists(filePath) && !Directory.Exists(filePath))
        //                    {
        //                        removeIndexArray[removeCount] = i;
        //                        removeCount++;
        //                    }
        //                }
        //            }
        //        }
        //        for (int i = 0; i < removeCount; i++)
        //        {
        //            QuarkEditorDataProxy.QuarkAssetDataset.DirHashPairs.RemoveAt(removeIndexArray[i]);
        //        }
        //        var dirs = QuarkEditorDataProxy.QuarkAssetDataset.DirHashPairs.ToArray();
        //        pathList.Clear();
        //        pathList.AddRange(dirs.Select(d => d.Dir));
        //    }
        //    catch (Exception e)
        //    {
        //        QuarkUtility.LogError($"OnGUI :{e}");
        //    }
        //    Reload();
        //    base.OnGUI(rect);
        //}
        protected override void SingleClickedItem(int id)
        {
            if (QuarkEditorDataProxy.QuarkAssetDataset == null)
                return;
            var infos = QuarkEditorDataProxy.QuarkAssetDataset.QuarkBundleInfoList;
            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(infos[id].AssetBundlePath);
            EditorGUIUtility.PingObject(obj);
            Selection.activeObject = obj;
            base.SingleClickedItem(id);
        }
        protected override void DoubleClickedItem(int id)
        {
            if (QuarkEditorDataProxy.QuarkAssetDataset == null)
                return;
            var infos = QuarkEditorDataProxy.QuarkAssetDataset.QuarkBundleInfoList;
            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(infos[id].AssetBundlePath);
            EditorGUIUtility.PingObject(obj);
            Selection.activeObject = obj;
            base.DoubleClickedItem(id);
        }
        protected override TreeViewItem BuildRoot()
        {
            int length = 0;
            List<QuarkBundleInfo> namePathInfoList = null;
            if (QuarkEditorDataProxy.QuarkAssetDataset != null)
            {
                namePathInfoList = QuarkEditorDataProxy.QuarkAssetDataset.QuarkBundleInfoList;
                length = namePathInfoList.Count;
            }
            var root = new TreeViewItem { id = -1, depth = -1, displayName = "Root" };
            var allItems = new List<TreeViewItem>();
            var assetIcon = EditorGUIUtility.FindTexture("PreMatCube");
            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    var item = new TreeViewItem { id = i, depth = 1, displayName = namePathInfoList[i].AssetBundleName, icon = assetIcon };
                    allItems.Add(item);
                }
            }
            SetupParentsAndChildrenFromDepths(root, allItems);
            return root;
        }
        protected override void RenameEnded(RenameEndedArgs args)
        {
            if (QuarkEditorDataProxy.QuarkAssetDataset == null)
                return;
            var item = FindItem(args.itemID, rootItem);
            var bundleList = QuarkEditorDataProxy.QuarkAssetDataset.QuarkBundleInfoList;
            if (!string.IsNullOrEmpty(args.newName))
            {
                //防止重名
                var canUse = true;
                var newName = args.newName;
                var length = bundleList.Count;
                for (int i = 0; i < length; i++)
                {
                    if (bundleList[i].AssetBundleName == newName)
                    {
                        canUse = false;
                        break;
                    }
                }
                if (canUse)
                {
                    var bundleInfo = bundleList[args.itemID];
                    bundleList[args.itemID] = new QuarkBundleInfo(bundleInfo.AssetBundlePath, newName);
                    item.displayName = args.newName;
                }
                else
                {
                    item.displayName = originalName;
                }
            }
            else
            {
                item.displayName = originalName;
            }
            EditorUtility.SetDirty(QuarkEditorDataProxy.QuarkAssetDataset);
            base.RenameEnded(args);
        }
        protected override bool CanRename(TreeViewItem item)
        {
            originalName = item.displayName;
            item.displayName = null;
            BeginRename(item);
            return item != null;
        }
        protected override void ContextClickedItem(int id)
        {
            List<string> selectedNodes = new List<string>();

            var selected = GetSelection();
            foreach (var nodeID in selected)
            {
                selectedNodes.Add(FindItem(nodeID, rootItem).displayName);
            }
            GenericMenu menu = new GenericMenu();
            if (selectedNodes.Count > 1)
            {
                menu.AddItem(new GUIContent("Delete "), false, Delete, selectedNodes);
                menu.AddItem(new GUIContent("DeleteAll "), false, DeleteAll);
                menu.AddItem(new GUIContent("ResetAllBundlesName"), false, ResetAllBundlesName);
            }
            if (selectedNodes.Count == 1)
            {
                menu.AddItem(new GUIContent("Delete "), false, Delete, selectedNodes);
                menu.AddItem(new GUIContent("ResetBundleName"), false, ResetBundleName, id);
            }
            menu.ShowAsContext();
        }
        void DeleteAll()
        {
            Reload();
        }
        void ResetAllBundlesName()
        {
            if (QuarkEditorDataProxy.QuarkAssetDataset == null)
                return;
            var infos = QuarkEditorDataProxy.QuarkAssetDataset.QuarkBundleInfoList;
            for (int i = 0; i < infos.Count; i++)
            {
                var info = infos[i];
                infos[i] = new QuarkBundleInfo(info.AssetBundlePath, info.AssetBundlePath);
            }
            EditorUtility.SetDirty(QuarkEditorDataProxy.QuarkAssetDataset);
            Reload();
        }
        void ResetBundleName(object context)
        {
            var id = Convert.ToInt32(context);
            if (QuarkEditorDataProxy.QuarkAssetDataset == null)
                return;
            var info = QuarkEditorDataProxy.QuarkAssetDataset.QuarkBundleInfoList[id];
            QuarkEditorDataProxy.QuarkAssetDataset.QuarkBundleInfoList[id]
                = new QuarkBundleInfo(info.AssetBundlePath, info.AssetBundlePath);
            EditorUtility.SetDirty(QuarkEditorDataProxy.QuarkAssetDataset);
            Reload();
        }
        void Delete(object context)
        {
            try
            {
                var list = context as List<string>;
                for (int i = 0; i < list.Count; i++)
                {
                    RemovePath(list[i]);
                }
                Reload();
            }
            catch (Exception e)
            {
                QuarkUtility.LogError(e);
            }
        }
    }
}
