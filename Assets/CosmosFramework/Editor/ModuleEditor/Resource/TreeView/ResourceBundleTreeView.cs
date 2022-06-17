using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using Cosmos.Resource;
using UnityEngine;

namespace Cosmos.Editor
{
    public class ResourceBundleTreeView : TreeView
    {
        public ResourceBundleTreeView(TreeViewState state) : base(state) { }
        List<ResourceBundle> bundleList = new List<ResourceBundle>();
        public override void OnGUI(Rect rect)
        {
            Reload();
            base.OnGUI(rect);
        }
        //protected override void SingleClickedItem(int id)
        //{
        //    base.SingleClickedItem(id);
        //    var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(bundleList[id].BundleName);
        //    EditorGUIUtility.PingObject(obj);
        //    Selection.activeObject = obj;
        //}
        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = -1, depth = -1, displayName = "Root" };
            var allItems = new List<TreeViewItem>();
            {
                for (int i = 0; i < bundleList.Count; i++)
                {
                    var item = new TreeViewItem { id = i, depth = 1, displayName = bundleList[i].BundleName };
                    allItems.Add(item);
                }
                SetupParentsAndChildrenFromDepths(root, allItems);
                return root;
            }
        }
    }
}
