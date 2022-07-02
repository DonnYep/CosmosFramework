using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Quark.Editor
{
    public class QuarkAssetObjectTreeView : TreeView
    {
        List<string> pathList = new List<string>();
        public QuarkAssetObjectTreeView(TreeViewState treeViewState)
    : base(treeViewState)
        {
            Reload();
            showAlternatingRowBackgrounds = true;
            showBorder = true;
        }
        public void AddPath(string path)
        {
            pathList.Add(path);
        }
        public void AddPaths(IEnumerable<string> paths)
        {
            pathList.Clear();
            pathList.AddRange(paths);
            Reload();
        }
        public void Clear()
        {
            pathList.Clear();
            Reload();
        }
        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            if (pathList.Count < id)
                return;
            var obj = AssetDatabase.LoadAssetAtPath<Object>(pathList[id]);
            EditorGUIUtility.PingObject(obj);
            Selection.activeObject = obj;
        }
        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);
            if (pathList.Count < id)
                return;
            var obj = AssetDatabase.LoadAssetAtPath<Object>(pathList[id]);
            EditorGUIUtility.PingObject(obj);
            Selection.activeObject = obj;
        }
        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = -1, depth = -1, displayName = "Root" };
            Texture2D objectIcon = null;
            var allItems = new List<TreeViewItem>();
            {
                for (int i = 0; i < pathList.Count; i++)
                {
                    var obj = AssetDatabase.LoadAssetAtPath(pathList[i], typeof(UnityEngine.Object));
                    bool isValidAsset = obj != null;
                    if (isValidAsset)
                    {
                        objectIcon = QuarkAssetEditorUtility.ToTexture2D(EditorGUIUtility.ObjectContent(obj, obj.GetType()).image);
                    }
                    else
                    {
                        objectIcon = EditorGUIUtility.FindTexture("console.erroricon");
                    }
                    var item = new TreeViewItem { id = i, depth = 1, displayName = pathList[i], icon = objectIcon };
                    allItems.Add(item);
                }
                SetupParentsAndChildrenFromDepths(root, allItems);
                return root;
            }
        }
    }
}
