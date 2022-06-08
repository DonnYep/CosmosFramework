using System;
namespace Cosmos.Data
{
    //================================================
    /*
    *1、数据存储模块。可以使用几种方式记录数据：
    *    Data/SubData/SubDataA
    *    Data\SubData\SubDataA
    *    Data.SubData.SubDataA
    *    此为树状结构，又如有数据为Data/SubData/SubDataB，则表示为：
    *    Data/SubData/ 下的两个数据，分别为SubDataA与SubDataB。
    *    写法上使用何种分隔符并无区别。分隔符为 string[] { ".", "/", "\\" };
    */
    //================================================
    [Module]
    internal sealed partial class DataNodeManager : Module, IDataNodeManager
    {
        #region Properties
        private static readonly string[] EmptyStringArray = new string[] { };
        private static readonly string[] PathSplitSeparator = new string[] { ".", "/", "\\" };
        private const string RootName = "<Root>";
        private DataNode rootNode;
        ///<inheritdoc/>
        public IDataNode Root
        {
            get
            {
                return rootNode;
            }
        }
        #endregion
        #region Methods
        ///<inheritdoc/>
        public T GetData<T>(string path) where T : Variable
        {
            return GetData<T>(path, null);
        }
        ///<inheritdoc/>
        public Variable GetData(string path)
        {
            return GetData(path, null);
        }
        ///<inheritdoc/>
        public T GetData<T>(string path, IDataNode node) where T : Variable
        {
            IDataNode current = GetNode(path, node);
            if (current == null)
            {
                var nodeVar = node != null ? node.FullName : string.Empty;
                throw new ArgumentNullException($"Data node is not exist, path '{path}', node '{nodeVar}'.");
            }
            return current.GetData<T>();
        }
        ///<inheritdoc/>
        public Variable GetData(string path, IDataNode node)
        {
            IDataNode current = GetNode(path, node);
            if (current == null)
            {
                var nodeVar = node != null ? node.FullName : string.Empty;
                throw new ArgumentNullException($"Data node is not exist, path '{path}', node '{nodeVar}'.");
            }
            return current.GetData();
        }
        ///<inheritdoc/>
        public void SetData<T>(string path, T data) where T : Variable
        {
            SetData(path, data, null);
        }
        ///<inheritdoc/>
        public void SetData(string path, Variable data)
        {
            SetData(path, data, null);
        }
        ///<inheritdoc/>
        public void SetData<T>(string path, T data, IDataNode node) where T : Variable
        {
            IDataNode current = GetOrAddNode(path, node);
            current.SetData(data);
        }
        ///<inheritdoc/>
        public void SetData(string path, Variable data, IDataNode node)
        {
            IDataNode current = GetOrAddNode(path, node);
            current.SetData(data);
        }
        ///<inheritdoc/>
        public IDataNode GetNode(string path)
        {
            return GetNode(path, null);
        }
        ///<inheritdoc/>
        public IDataNode GetNode(string path, IDataNode node)
        {
            IDataNode current = node ?? rootNode;
            string[] splitedPath = GetSplitedPath(path);
            foreach (string i in splitedPath)
            {
                current = current.GetChild(i);
                if (current == null)
                {
                    return null;
                }
            }
            return current;
        }
        ///<inheritdoc/>
        public IDataNode GetOrAddNode(string path)
        {
            return GetOrAddNode(path, null);
        }
        ///<inheritdoc/>
        public IDataNode GetOrAddNode(string path, IDataNode node)
        {
            IDataNode current = node ?? rootNode;
            string[] splitedPath = GetSplitedPath(path);
            foreach (string i in splitedPath)
            {
                current = current.GetOrAddChild(i);
            }
            return current;
        }
        ///<inheritdoc/>
        public void RemoveNode(string path)
        {
            RemoveNode(path, null);
        }
        ///<inheritdoc/>
        public void RemoveNode(string path, IDataNode node)
        {
            IDataNode current = node ?? rootNode;
            IDataNode parent = current.Parent;
            string[] splitedPath = GetSplitedPath(path);
            foreach (string i in splitedPath)
            {
                parent = current;
                current = current.GetChild(i);
                if (current == null)
                {
                    return;
                }
            }
            if (parent != null)
            {
                parent.RemoveChild(current.Name);
            }
        }
        ///<inheritdoc/>
        public void ClearNodes()
        {
            rootNode.ClearNode();
        }
        protected override void OnInitialization()
        {
            rootNode = DataNode.Create(RootName, null);
        }
        /// <summary>
        /// 数据结点路径切分工具函数；
        /// </summary>
        /// <param name="path">要切分的数据结点路径</param>
        /// <returns>切分后的字符串数组</returns>
        string[] GetSplitedPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return EmptyStringArray;
            }
            return path.Split(PathSplitSeparator, StringSplitOptions.RemoveEmptyEntries);
        }
        #endregion
    }
}