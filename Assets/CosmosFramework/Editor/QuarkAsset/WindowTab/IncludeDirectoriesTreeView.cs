using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Cosmos.CosmosEditor
{
    public class IncludeDirectoriesTreeView : TreeView
    {
        List<string> pathList = new List<string>();
        public List<string> PathList { get { return pathList; } set { pathList = value; Reload(); } }
        public void Clear()
        {
            pathList.Clear();
            Reload();
        }
        public void AddPath(string path)
        {
            if (!pathList.Contains(path))
                pathList.Add(path);
            Reload();
        }
        public void RemovePath(string path)
        {
            try
            {
                pathList.Remove(path);
                Reload();
            }
            catch (Exception e)
            {
                EditorUtil.Debug.LogError(e);
            }
        }
        public IncludeDirectoriesTreeView(TreeViewState treeViewState)
            : base(treeViewState)
        {
            Reload();
        }
        //protected override void RowGUI(RowGUIArgs args)
        //{
        //    Color old = GUI.color;
        //    if (args.row % 2 == 0)
        //        GUI.color = Color.cyan;
        //    else
        //        GUI.color = Color.yellow;
        //    base.RowGUI(args);
        //    GUI.color = old;
        //}
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
            for (int i = 0; i < pathList.Count; i++)
            {
                var item = new TreeViewItem { id = i, depth = 1, displayName = pathList[i] };
                allItems.Add(item);
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
