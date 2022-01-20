using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Quark.Asset;

namespace CosmosEditor.Quark
{
    public class QuarkDirectoriesTreeView : TreeView
    {
        List<string> pathList = new List<string>();
        bool canRender { get { return QuarkEditorDataProxy.QuarkAssetDataset != null; } }
        public void Clear()
        {
            pathList.Clear();
            Reload();
        }
        public void AddPath(string path)
        {
            if (QuarkEditorDataProxy.QuarkAssetDataset == null)
                return;
            var dirHashPairs = QuarkEditorDataProxy.QuarkAssetDataset.DirHashPairs;
            var length = dirHashPairs.Count;
            bool existed = false;
            for (int i = 0; i < length; i++)
            {
                if (dirHashPairs[i].Dir == path)
                {
                    existed = true;
                    break;
                }
            }
            if (!existed)
            {
                var hash = AssetDatabase.AssetPathToGUID(path);
                var pair = new QuarkDirHashPair(hash, path);
                QuarkEditorDataProxy.QuarkAssetDataset.DirHashPairs.Add(pair);
                pathList.Add(path);
            }
            Reload();
        }
        public void RemovePath(string path)
        {
            try
            {
                if (QuarkEditorDataProxy.QuarkAssetDataset == null)
                    return;
                var dirHashPairs = QuarkEditorDataProxy.QuarkAssetDataset.DirHashPairs;
                var length = dirHashPairs.Count;
                int removeindex = -1;
                for (int i = 0; i < length; i++)
                {
                    if (dirHashPairs[i].Dir == path)
                    {
                        removeindex = i;
                        break;
                    }
                }
                if (removeindex != -1)
                {
                    QuarkEditorDataProxy.QuarkAssetDataset.DirHashPairs.RemoveAt(removeindex);
                    pathList.Remove(path);
                }
            }
            catch (Exception e)
            {
                EditorUtil.Debug.LogError(e);
            }
            Reload();
        }
        public QuarkDirectoriesTreeView(TreeViewState treeViewState)
            : base(treeViewState)
        {
            Reload();
        }
        public override void OnGUI(Rect rect)
        {
            if (UnityEngine.Event.current.type == EventType.Repaint)
                DefaultStyles.backgroundOdd.Draw(rect, false, false, false, false);
            try
            {
                if (QuarkEditorDataProxy.QuarkAssetDataset == null)
                    return;
                var dirHashPairs = QuarkEditorDataProxy.QuarkAssetDataset.DirHashPairs;
                var dirHashCount = dirHashPairs.Count;
                int removeCount = 0;
                int[] removeIndexArray = new int[dirHashCount];
                for (int i = 0; i < dirHashCount; i++)
                {
                    var srcHash = dirHashPairs[i].Hash;
                    var srcDir = dirHashPairs[i].Dir;
                    var newPath = AssetDatabase.GUIDToAssetPath(srcHash);
                    var newHash = AssetDatabase.AssetPathToGUID(srcDir);
                    if (newPath != dirHashPairs[i].Dir)
                    {
                        if (!string.IsNullOrEmpty(newPath))
                            dirHashPairs[i] = new QuarkDirHashPair(srcHash, newPath);
                    }
                    else
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), srcDir);
                        if (!File.Exists(filePath) && !Directory.Exists(filePath))
                        {
                            if (!File.Exists(filePath) && !Directory.Exists(filePath))
                            {
                                removeIndexArray[removeCount] = i;
                                removeCount++;
                            }
                        }
                    }
                }
                for (int i = 0; i < removeCount; i++)
                {
                    QuarkEditorDataProxy.QuarkAssetDataset.DirHashPairs.RemoveAt(removeIndexArray[i]);
                }
                var dirs = QuarkEditorDataProxy.QuarkAssetDataset.DirHashPairs.ToArray();
                pathList.Clear();
                pathList.AddRange(dirs.Select(d => d.Dir));
            }
            catch (Exception e)
            {
                EditorUtil.Debug.LogError($"OnGUI :{e}");
            }
            Reload();
            base.OnGUI(rect);
        }
        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            EditorUtil.SelectionActiveObject(pathList[id]);
        }
        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);
            EditorUtil.PingAndActiveObject(pathList[id]);
        }
        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = -1, depth = -1, displayName = "Root" };
            var allItems = new List<TreeViewItem>();
            if (canRender)
            {
                for (int i = 0; i < pathList.Count; i++)
                {
                    var item = new TreeViewItem { id = i, depth = 1, displayName = pathList[i] };
                    allItems.Add(item);
                }
            }
            SetupParentsAndChildrenFromDepths(root, allItems);
            return root;
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
            if (selectedNodes.Count >= 1)
            {
                menu.AddItem(new GUIContent("Delete "), false, Delete, selectedNodes);
                menu.AddItem(new GUIContent("DeleteAll "), false, DeleteAll);
            }
            menu.ShowAsContext();
        }
        void DeleteAll()
        {
            pathList.Clear();
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
                EditorUtil.Debug.LogError(e);
            }
        }
    }
}
